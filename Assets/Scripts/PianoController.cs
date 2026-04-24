using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using F1yingBanana.SfizzUnity;
using NaughtyAttributes;
using NonsensicalKit.Core;
using NonsensicalKit.Windows.Hook;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AggregatorEnum]
public enum PianoEvent
{
    ChangePianoKeyState = 1300,
    ChangePianoModifySwitch,
    ChangePianoSample,
}

public enum PianoInputMode
{
    TwoSpans, //Y代表C(n),Z代表C(n-1),n默认为3
    ThreeSpans, //T代表C(n),<代表C(n-1),Z代表C(n-2),n默认为3
}

public class PianoController : NonsensicalMono
{
    private const int MiddleCNote = 60;
    private const int MinMidiNote = 0;
    private const int MaxMidiNote = 120;

    [SerializeField] private SfizzPlayer m_player;
    [SerializeField] private bool m_initPlay;

    [SerializeField, Range(0, 127)]
    private int m_velocity = 64;

    [SerializeField] private Object[] m_sfzFiles;
    [SerializeField] private string[] m_sfzFilesPath;
    [SerializeField] private int m_startSample;


    private Dictionary<VirtualKeys, int> KeyMap => _inputMode switch
    {
        PianoInputMode.TwoSpans => _twoSpansKeyMap,
        PianoInputMode.ThreeSpans => _threeSpansKeyMap,
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>
    /// 按键和C3的默认偏移
    /// </summary>
    private readonly Dictionary<VirtualKeys, int> _twoSpansKeyMap = new Dictionary<VirtualKeys, int>()
    {
        { VirtualKeys.OEM_3, -11 },
        { VirtualKeys.KEY_1, -9 },
        { VirtualKeys.KEY_2, -9 },
        { VirtualKeys.KEY_3, -6 },
        { VirtualKeys.KEY_4, -4 },
        { VirtualKeys.KEY_5, -2 },
        { VirtualKeys.KEY_6, -2 },
        { VirtualKeys.KEY_7, 1 },
        { VirtualKeys.KEY_8, 3 },
        { VirtualKeys.KEY_9, 3 },
        { VirtualKeys.KEY_0, 6 },
        { VirtualKeys.OEM_MINUS, 8 },
        { VirtualKeys.OEM_PLUS, 10 },
        { VirtualKeys.BACK, 13 },

        { VirtualKeys.KEY_Q, -8 },
        { VirtualKeys.KEY_W, -7 },
        { VirtualKeys.KEY_E, -5 },
        { VirtualKeys.KEY_R, -3 },
        { VirtualKeys.KEY_T, -1 },
        { VirtualKeys.KEY_Y, 0 },
        { VirtualKeys.KEY_U, 2 },
        { VirtualKeys.KEY_I, 4 },
        { VirtualKeys.KEY_O, 5 },
        { VirtualKeys.KEY_P, 7 },
        { VirtualKeys.OEM_4, 9 },
        { VirtualKeys.OEM_6, 11 },
        { VirtualKeys.OEM_5, 12 },

        { VirtualKeys.KEY_A, -13 },
        { VirtualKeys.KEY_S, -11 },
        { VirtualKeys.KEY_D, -9 },
        { VirtualKeys.KEY_F, -9 },
        { VirtualKeys.KEY_G, -6 },
        { VirtualKeys.KEY_H, -4 },
        { VirtualKeys.KEY_J, -2 },
        { VirtualKeys.KEY_K, -2 },
        { VirtualKeys.KEY_L, 1 },
        { VirtualKeys.OEM_1, 3 },
        { VirtualKeys.OEM_7, 3 },
        { VirtualKeys.RETURN, 6 },

        { VirtualKeys.KEY_Z, -12 },
        { VirtualKeys.KEY_X, -10 },
        { VirtualKeys.KEY_C, -8 },
        { VirtualKeys.KEY_V, -7 },
        { VirtualKeys.KEY_B, -5 },
        { VirtualKeys.KEY_N, -3 },
        { VirtualKeys.KEY_M, -1 },
        { VirtualKeys.OEM_COMMA, 0 },
        { VirtualKeys.OEM_PERIOD, 2 },
        { VirtualKeys.OEM_2, 4 },
    };

    /// <summary>
    /// 按键和C3的默认偏移
    /// </summary>
    private readonly Dictionary<VirtualKeys, int> _threeSpansKeyMap = new Dictionary<VirtualKeys, int>()
    {
        { VirtualKeys.OEM_3, -11 },
        { VirtualKeys.KEY_1, -9 },
        { VirtualKeys.KEY_2, -6 },
        { VirtualKeys.KEY_3, -4 },
        { VirtualKeys.KEY_4, -2 },
        { VirtualKeys.KEY_5, -2 },
        { VirtualKeys.KEY_6, 1 },
        { VirtualKeys.KEY_7, 3 },
        { VirtualKeys.KEY_8, 5 },
        { VirtualKeys.KEY_9, 5 },
        { VirtualKeys.KEY_0, 8 },
        { VirtualKeys.OEM_MINUS, 10 },
        { VirtualKeys.OEM_PLUS, 10 },
        { VirtualKeys.BACK, 13 },

        { VirtualKeys.KEY_Q, -6 },
        { VirtualKeys.KEY_W, -5 },
        { VirtualKeys.KEY_E, -3 },
        { VirtualKeys.KEY_R, -1 },
        { VirtualKeys.KEY_T, 0 },
        { VirtualKeys.KEY_Y, 2 },
        { VirtualKeys.KEY_U, 4 },
        { VirtualKeys.KEY_I, 6 },
        { VirtualKeys.KEY_O, 7 },
        { VirtualKeys.KEY_P, 9 },
        { VirtualKeys.OEM_4, 11 },
        { VirtualKeys.OEM_6, 12 },
        { VirtualKeys.OEM_5, 14 },

        { VirtualKeys.KEY_A, -25 },
        { VirtualKeys.KEY_S, -23 },
        { VirtualKeys.KEY_D, -21 },
        { VirtualKeys.KEY_F, -21 },
        { VirtualKeys.KEY_G, -18 },
        { VirtualKeys.KEY_H, -16 },
        { VirtualKeys.KEY_J, -14 },
        { VirtualKeys.KEY_K, -14 },
        { VirtualKeys.KEY_L, -11 },
        { VirtualKeys.OEM_1, -9 },
        { VirtualKeys.OEM_7, -9 },
        { VirtualKeys.RETURN, -6 },

        { VirtualKeys.KEY_Z, -24 },
        { VirtualKeys.KEY_X, -22 },
        { VirtualKeys.KEY_C, -20 },
        { VirtualKeys.KEY_V, -19 },
        { VirtualKeys.KEY_B, -17 },
        { VirtualKeys.KEY_N, -15 },
        { VirtualKeys.KEY_M, -13 },
        { VirtualKeys.OEM_COMMA, -12 },
        { VirtualKeys.OEM_PERIOD, -10 },
        { VirtualKeys.OEM_2, -8 },
    };

    private readonly List<VirtualKeys> _waitPress = new List<VirtualKeys>();
    private readonly List<VirtualKeys> _waitRelease = new List<VirtualKeys>();

    private bool _sustainPedalState;
    private bool _sustainPedalLastState;

    private int _octaveModifier = 0;

    private bool _canModify;
    private bool _playing;
    private PianoInputMode _inputMode;
    private int _sfzIndex;

    [Button]
    private void UpdatePath()
    {
#if UNITY_EDITOR
        m_sfzFilesPath = new string[m_sfzFiles.Length];
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
#endif
    }

    private void Awake()
    {
        _playing = m_initPlay;
        _sfzIndex = m_startSample;

        if (m_sfzFilesPath != null && m_sfzFilesPath.Length > 0)
        {
            _sfzIndex = Mathf.Clamp(_sfzIndex, 0, m_sfzFilesPath.Length - 1);
            ChangeSample(_sfzIndex).Forget();
        }
        else
        {
            Debug.LogWarning("未配置SFZ采样路径，钢琴将保持静音。");
        }
        Subscribe(PianoEvent.ChangePianoModifySwitch, OnSwitchModifier);
        Subscribe(PianoEvent.ChangePianoKeyState, OnChangedState);
        Subscribe(PianoEvent.ChangePianoSample, OnChangeSample);
        Subscribe<KeyEvent>(WindowsEvent.KeyBoardEvent, this.OnKeyboardEvent);
        IOCC.Register("TrayMenu", GetMenu);
    }

    private List<(string, int, Action)> GetMenu()
    {
        var menu = new List<(string, int, Action)>()
        {
            ($"钢琴按键\\切换状态({(_playing ? "启用中" : "禁用中")})(F6)", 700, OnChangedState),
            ($@"钢琴按键\切换输入模型({_inputMode switch {
                PianoInputMode.TwoSpans => "ZY",
                PianoInputMode.ThreeSpans => "Z<T",
            }})", 701, OnChangedInputMode),
            ($"钢琴按键\\切换变调状态({(_canModify ? "变调启用中" : "变调禁用中")})", 702, OnSwitchModifier),
            ($"钢琴按键\\采样切换({_sfzIndex + 1})", 703, OnChangeSample),
        };
        return menu;
    }

    private void ChangeModifier(int change)
    {
        if (!_canModify) return;
        _octaveModifier += change;
        if (_octaveModifier < -48)
        {
            _octaveModifier = -48;
        }

        if (_octaveModifier > 108)
        {
            _octaveModifier = 108;
        }
    }

    private void OnKeyboardEvent(KeyEvent @event)
    {
        var key = @event.Key;
        switch (@event.KeyboardMessage)
        {
            case HookKeyboardMessage.WM_KEYDOWN:
            {
                switch (key)
                {
                    case VirtualKeys.LSHIFT: ChangeModifier(-12); break;
                    case VirtualKeys.RSHIFT: ChangeModifier(12); break;
                    case VirtualKeys.LCONTROL: ChangeModifier(-12); break;
                    case VirtualKeys.RCONTROL: ChangeModifier(12); break;
                    case VirtualKeys.TAB: _octaveModifier = 0; break;
                    case VirtualKeys.SPACE: _sustainPedalState = true; break;
                }

                if (_playing && KeyMap.ContainsKey(key))
                {
                    _waitPress.Add(key);
                }

                break;
            }
            case HookKeyboardMessage.WM_KEYUP:
            {
                switch (key)
                {
                    case VirtualKeys.LSHIFT: ChangeModifier(12); break;
                    case VirtualKeys.RSHIFT: ChangeModifier(-12); break;
                    case VirtualKeys.SPACE: _sustainPedalState = false; break;
                }

                if (_playing && KeyMap.ContainsKey(key))
                {
                    _waitRelease.Add(key);
                }

                break;
            }
        }
    }

    private int CheckKey(int key)
    {
        return Mathf.Clamp(key, MinMidiNote, MaxMidiNote);
    }

    private void Update()
    {
        if (!_playing) return;
        //60是C3
        if (m_player.Loading) return;
        var currentKeyMap = KeyMap;
        foreach (var key in _waitPress)
        {
            m_player.Sfizz.SendNoteOn(0, CheckKey(MiddleCNote + currentKeyMap[key] + _octaveModifier), m_velocity);
        }

        _waitPress.Clear();

        foreach (var key in _waitRelease)
        {
            m_player.Sfizz.SendNoteOff(0, CheckKey(MiddleCNote + currentKeyMap[key] + _octaveModifier), m_velocity);
        }

        _waitRelease.Clear();

        if (_sustainPedalLastState != _sustainPedalState)
        {
            m_player.Sfizz.SendCC(0, 64, _sustainPedalState ? 127 : 0);

            _sustainPedalLastState = _sustainPedalState;
        }
    }

    private void OnSwitchModifier()
    {
        _canModify = !_canModify;
    }

    private void OnChangedState()
    {
        _playing = !_playing;
    }

    private void OnChangedInputMode()
    {
        _inputMode = (PianoInputMode)(((int)_inputMode + 1) % Enum.GetValues(typeof(PianoInputMode)).Length);
    }

    private void OnChangeSample()
    {
        if (m_sfzFilesPath == null || m_sfzFilesPath.Length == 0)
        {
            Debug.LogWarning("未配置SFZ采样路径，无法切换采样。");
            return;
        }

        _sfzIndex++;
        if (_sfzIndex >= m_sfzFilesPath.Length)
        {
            _sfzIndex = 0;
        }

        ChangeSample(_sfzIndex).Forget();
    }

    private async UniTaskVoid ChangeSample(int index)
    {
        if (m_player.Loading) return;
        if (m_sfzFilesPath == null || m_sfzFilesPath.Length == 0)
        {
            Debug.LogWarning("未配置SFZ采样路径，无法加载采样。");
            return;
        }

        if (index < 0 || index >= m_sfzFilesPath.Length)
        {
            Debug.LogWarning($"采样索引超出范围: {index}");
            return;
        }

        var path = Path.Combine(Application.streamingAssetsPath, m_sfzFilesPath[index]);
        bool result = await m_player.LoadFileAsync(path);
        if (!result)
        {
            Debug.LogWarning($"Sfz not found at the given path: {path}, player will remain silent.");
        }
    }
}
