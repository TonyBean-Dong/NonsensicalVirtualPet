using System.IO;
using Cysharp.Threading.Tasks;
using F1yingBanana.SfizzUnity;
using Melanchall.DryWetMidi.Core;
using NaughtyAttributes;
using NonsensicalKit.Core;
using NonsensicalKit.Core.Log;
using NonsensicalKit.Tools.ObjectPool;
using NonsensicalKit.Windows.Hook;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public enum MidiMusicEvent
{
    ChangeMidiMusicState = 1300,
    ChangeMidiMusicSample,
}

public sealed class MidiPlayerController : NonsensicalMono
{
    [SerializeField] private Object[] m_midiFiles;
    [SerializeField] private string[] m_midiFilesPath;

    [SerializeField] private Object[] m_sfzFiles;
    [SerializeField] private string[] m_sfzFilesPath;

    [SerializeField, Range(1, Sfizz.MaxSampleRate)]
    private int m_sampleRate = 44100;

    [SerializeField] private SfizzMidiRenderer m_sfizzMidiRenderer;

    private ComponentPool<AudioSource> _sourcesPool;
    private AudioSource[][] _players;

    private int _crtIndex = -1;
    private bool _isPlaying = true;
    private bool _loading = false;
    private int _crtSample = 0;

    [Button]
    private void UpdatePath()
    {
#if UNITY_EDITOR
        m_sfzFilesPath = new string[m_sfzFiles.Length];
        m_midiFilesPath = new string[m_midiFiles.Length];
        for (int i = 0; i < m_sfzFiles.Length; i++)
        {
            var path = AssetDatabase.GetAssetPath(m_sfzFiles[i]);
            var prefix = "Assets/StreamingAssets/";
            if (path.StartsWith(prefix))
            {
                m_sfzFilesPath[i] = path.Substring(prefix.Length);
            }
            else
            {
                Debug.LogError("SFZ文件应放入StreamingAsset文件夹中");
            }
        }

        for (int i = 0; i < m_midiFiles.Length; i++)
        {
            var path = AssetDatabase.GetAssetPath(m_midiFiles[i]);
            var prefix = "Assets/StreamingAssets/";
            if (path.StartsWith(prefix))
            {
                m_midiFilesPath[i] = path.Substring(prefix.Length);
            }
            else
            {
                Debug.LogError("Midi文件应放入StreamingAsset文件夹中");
            }
        }
#endif
    }

    private void Awake()
    {
        _players = new AudioSource[m_midiFiles.Length][];
        Subscribe(MidiMusicEvent.ChangeMidiMusicState, OnSwitchMidiMusic);
        Subscribe(MidiMusicEvent.ChangeMidiMusicSample, OnChangeMidiMusicSample);
        Subscribe<KeyEvent>(WindowsEvent.KeyBoardEvent, this.OnKeyboardEvent);

        var go = new GameObject("Source");
        var source = go.AddComponent<AudioSource>();
        source.loop = true;
        _sourcesPool = new ComponentPool<AudioSource>(source, OnNew, OnStore);
    }

    private void OnNew(AudioSource source)
    {
    }

    private void OnStore(AudioSource source)
    {
    }

    private void OnChangeMidiMusicSample()
    {
        _crtSample++;
        if (_crtSample >= m_sfzFilesPath.Length)
        {
            _crtSample = 0;
        }

        if (_crtIndex >= 0)
        {
            StopMidi(_crtIndex);
            _ = RenderMidiAsync(_crtIndex);
        }
    }

    private void OnSwitchMidiMusic()
    {
        _isPlaying = !_isPlaying;
        if (!_isPlaying)
        {
            if (_crtIndex >= 0)
            {
                StopMidi(_crtIndex);
                _crtIndex = -1;
            }
        }
    }

    private void OnKeyboardEvent(KeyEvent @event)
    {
        if (@event.KeyboardMessage == HookKeyboardMessage.WM_KEYDOWN)
        {
            switch (@event.Key)
            {
                case VirtualKeys.NUMPAD0: Switch(0); break;
                case VirtualKeys.NUMPAD1: Switch(1); break;
                case VirtualKeys.NUMPAD2: Switch(2); break;
                case VirtualKeys.NUMPAD3: Switch(3); break;
                case VirtualKeys.NUMPAD4: Switch(4); break;
                case VirtualKeys.NUMPAD5: Switch(5); break;
                case VirtualKeys.NUMPAD6: Switch(6); break;
                case VirtualKeys.NUMPAD7: Switch(7); break;
                case VirtualKeys.NUMPAD8: Switch(8); break;
                case VirtualKeys.NUMPAD9: Switch(9); break;
                case VirtualKeys.ADD: Switch(10); break;
                case VirtualKeys.SUBTRACT: Switch(11); break;
                case VirtualKeys.MULTIPLY: Switch(12); break;
                case VirtualKeys.DIVIDE: Switch(13); break;
                case VirtualKeys.DECIMAL: Switch(14); break;
            }
        }
    }

    private void Switch(int index)
    {
        if (index < 0 || index >= m_midiFilesPath.Length) return;
        if (_loading) return;

        if (_crtIndex != index)
        {
            if (_crtIndex >= 0)
            {
                StopMidi(_crtIndex);
            }

            _ = RenderMidiAsync(index);
            _crtIndex = index;
        }
        else
        {
            StopMidi(_crtIndex);
            _crtIndex = -1;
        }
    }

    private async UniTask RenderMidiAsync(int index)
    {
        if (_players[index] != null)
        {
            return;
        }

        LogCore.Debug($"播放{m_midiFilesPath[index]}");

        _loading = true;
        var midiPath = Path.Combine(Application.streamingAssetsPath, m_midiFilesPath[index]);

        MidiFile midiFile = MidiFile.Read(midiPath);

        var clips = await m_sfizzMidiRenderer.RenderAsync(midiFile,
            Path.Combine(Application.streamingAssetsPath, m_sfzFilesPath[_crtSample]), m_sampleRate);

        await UniTask.SwitchToMainThread();

        _loading = false;
        LogCore.Debug($"加载完成{m_midiFilesPath[index]}");
        if (_crtIndex != index)
        {
            return;
        }

        _players[index] = new AudioSource[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            AudioClip clip = clips[i];

            if (clip != null)
            {
                var newSource = _sourcesPool.New();
                newSource.clip = clip;
                newSource.Play();
                _players[index][i] = newSource;
            }
        }
    }

    private void StopMidi(int index)
    {
        if (_players[index] == null)
        {
            return;
        }

        foreach (var source in _players[index])
        {
            source.Stop();
            source.clip = null;
            _sourcesPool.Store(source);
        }

        _players[index] = null;
    }
}
