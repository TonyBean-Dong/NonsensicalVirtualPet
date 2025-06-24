// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace PianoInputLab
// {
//     /// <summary>
//     /// 废弃
//     /// 原始的直接将按键和音频文件绑定的方式
//     /// 配置音频文件和与按键绑定都很麻烦，且一次按键只能原样播放一个音频文件
//     /// 之后将改为读取sfz文件，使用sfizz库
//     /// </summary>
//     public class PianoInput : MonoBehaviour
//     {
//         [SerializeField] private KeySampleConfig[] m_keySamples;
//         [SerializeField] private int m_offset;
//         private PianoKeyCore _core;
//
//         private void Reset()
//         {
//             PianoKeyCore.InitConfig(ref m_keySamples);
//         }
//
//         private void Awake()
//         {
//             _core = new PianoKeyCore(m_keySamples, gameObject);
//             _core.DelayTime = 0.25f;
//             _core.FadeOutTime = 0.5f;
//         }
//
//         private void Update()
//         {
//             _core.UpdateTime(Time.deltaTime);
//
//             #region 左手
//
//             if (Input.GetKeyDown(KeyCode.Z)) _core.Press(PianoKey.C2 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Z)) _core.Release(PianoKey.C2 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.X)) _core.Press(PianoKey.D2 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.X)) _core.Release(PianoKey.D2 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.C)) _core.Press(PianoKey.E2 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.C)) _core.Release(PianoKey.E2 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.V)) _core.Press(PianoKey.F2 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.V)) _core.Release(PianoKey.F2 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.B)) _core.Press(PianoKey.G2 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.B)) _core.Release(PianoKey.G2 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.N)) _core.Press(PianoKey.A2 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.N)) _core.Release(PianoKey.A2 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.M)) _core.Press(PianoKey.B2 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.M)) _core.Release(PianoKey.B2 + m_offset * 12);
//
//             if (Input.GetKeyDown(KeyCode.S)) _core.Press(PianoKey.C2S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.S)) _core.Release(PianoKey.C2S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.D)) _core.Press(PianoKey.D2S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.D)) _core.Release(PianoKey.D2S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.G)) _core.Press(PianoKey.F2S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.G)) _core.Release(PianoKey.F2S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.H)) _core.Press(PianoKey.G2S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.H)) _core.Release(PianoKey.G2S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.J)) _core.Press(PianoKey.A2S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.J)) _core.Release(PianoKey.A2S + m_offset * 12);
//
//             if (Input.GetKeyDown(KeyCode.Comma)) _core.Press(PianoKey.C3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Comma)) _core.Release(PianoKey.C3 + m_offset * 12);
//
//             #endregion
//
//             #region 右手
//
//             if (Input.GetKeyDown(KeyCode.Y)) _core.Press(PianoKey.C3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Y)) _core.Release(PianoKey.C3 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.U)) _core.Press(PianoKey.D3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.U)) _core.Release(PianoKey.D3 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.I)) _core.Press(PianoKey.E3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.I)) _core.Release(PianoKey.E3 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.O)) _core.Press(PianoKey.F3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.O)) _core.Release(PianoKey.F3 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.P)) _core.Press(PianoKey.G3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.P)) _core.Release(PianoKey.G3 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.LeftBracket)) _core.Press(PianoKey.A3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.LeftBracket)) _core.Release(PianoKey.A3 + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.RightBracket)) _core.Press(PianoKey.B3 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.RightBracket)) _core.Release(PianoKey.B3 + m_offset * 12);
//
//             if (Input.GetKeyDown(KeyCode.Alpha7)) _core.Press(PianoKey.C3S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Alpha7)) _core.Release(PianoKey.C3S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.Alpha8)) _core.Press(PianoKey.D3S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Alpha8)) _core.Release(PianoKey.D3S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.Alpha0)) _core.Press(PianoKey.F3S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Alpha0)) _core.Release(PianoKey.F3S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.Minus)) _core.Press(PianoKey.G3S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Minus)) _core.Release(PianoKey.G3S + m_offset * 12);
//             if (Input.GetKeyDown(KeyCode.Equals)) _core.Press(PianoKey.A3S + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Equals)) _core.Release(PianoKey.A3S + m_offset * 12);
//
//             if (Input.GetKeyDown(KeyCode.Backslash)) _core.Press(PianoKey.C4 + m_offset * 12);
//             if (Input.GetKeyUp(KeyCode.Backslash)) _core.Release(PianoKey.C4 + m_offset * 12);
//
//             #endregion
//         }
//     }
//
//     public enum PianoKey
//     {
//         C0,
//         C0S,
//         D0,
//         D0S,
//         E0,
//         F0,
//         F0S,
//         G0,
//         G0S,
//         A0,
//         A0S,
//         B0,
//
//         C1,
//         C1S,
//         D1,
//         D1S,
//         E1,
//         F1,
//         F1S,
//         G1,
//         G1S,
//         A1,
//         A1S,
//         B1,
//
//         C2,
//         C2S,
//         D2,
//         D2S,
//         E2,
//         F2,
//         F2S,
//         G2,
//         G2S,
//         A2,
//         A2S,
//         B2,
//
//         C3,
//         C3S,
//         D3,
//         D3S,
//         E3,
//         F3,
//         F3S,
//         G3,
//         G3S,
//         A3,
//         A3S,
//         B3,
//
//         C4,
//         C4S,
//         D4,
//         D4S,
//         E4,
//         F4,
//         F4S,
//         G4,
//         G4S,
//         A4,
//         A4S,
//         B4,
//
//         C5,
//         C5S,
//         D5,
//         D5S,
//         E5,
//         F5,
//         F5S,
//         G5,
//         G5S,
//         A5,
//         A5S,
//         B5,
//
//         C6,
//         C6S,
//         D6,
//         D6S,
//         E6,
//         F6,
//         F6S,
//         G6,
//         G6S,
//         A6,
//         A6S,
//         B6,
//
//         C7,
//         C7S,
//         D7,
//         D7S,
//         E7,
//         F7,
//         F7S,
//         G7,
//         G7S,
//         A7,
//         A7S,
//         B7,
//
//         C8,
//         C8S,
//         D8,
//         D8S,
//         E8,
//         F8,
//         F8S,
//         G8,
//         G8S,
//         A8,
//         A8S,
//         B8,
//     }
//
//     [Serializable]
//     public class KeySampleConfig
//     {
//         public string Name; //用于在unity检查器的列表中直接显示名称
//         public PianoKey Key;
//         public AudioClip Clip;
//     }
//
//     public class PianoKeyCore
//     {
//         public float DelayTime { get; set; } = 0.25f; //延迟时间，松开按键后仍然继续播放的时间
//         public float FadeOutTime { get; set; } = 0.25f; //淡出时间，延迟之后声音渐渐消失的时间
//
//         private readonly Dictionary<PianoKey, AudioClip> _clips = new();
//         private readonly Dictionary<PianoKey, AudioSource> _sources = new();
//         private readonly Dictionary<PianoKey, float> _releasingTimer = new();
//         private readonly ComponentPool<AudioSource> _sourcePool;
//
//         public PianoKeyCore(KeySampleConfig[] configs, GameObject source)
//         {
//             for (int i = 0; i < 108; i++)
//             {
//                 _clips.Add((PianoKey)i, null);
//                 _sources.Add((PianoKey)i, null);
//             }
//
//             foreach (var config in configs)
//             {
//                 _clips[config.Key] = config.Clip;
//             }
//
//             _sourcePool = new ComponentPool<AudioSource>(source);
//         }
//
//         public void UpdateTime(float deltaTime)
//         {
//             List<PianoKey> waitRemove = new();
//
//             List<PianoKey> releasing = _releasingTimer.Keys.ToList();
//             foreach (var releasingKey in releasing)
//             {
//                 _releasingTimer[releasingKey] -= deltaTime;
//                 if (_releasingTimer[releasingKey] < FadeOutTime)
//                 {
//                     _sources[releasingKey].volume = _releasingTimer[releasingKey] / FadeOutTime;
//                 }
//
//                 if (_releasingTimer[releasingKey] < 0.0f)
//                 {
//                     FullRelease(releasingKey);
//                     waitRemove.Add(releasingKey);
//                 }
//             }
//
//             foreach (var key in waitRemove)
//             {
//                 _releasingTimer.Remove(key);
//             }
//         }
//
//         public void Press(PianoKey pianoKey)
//         {
//             if (_clips[pianoKey] == null) return;
//
//             if (_sources[pianoKey] == null)
//             {
//                 _sources[pianoKey] = _sourcePool.New();
//             }
//
//             _sources[pianoKey].clip = _clips[pianoKey];
//             _sources[pianoKey].volume = 1;
//             _sources[pianoKey].Play();
//
//             _releasingTimer.Remove(pianoKey);
//         }
//
//         public void Release(PianoKey pianoKey)
//         {
//             if (_clips[pianoKey] == null) return;
//
//             _releasingTimer[pianoKey] = DelayTime + FadeOutTime;
//         }
//
//         private void FullRelease(PianoKey pianoKey)
//         {
//             if (_clips[pianoKey] == null) return;
//             if (_sources[pianoKey] == null) return;
//
//             _sources[pianoKey].Stop();
//             _sources[pianoKey].clip = null;
//             _sourcePool.Store(_sources[pianoKey]);
//             _sources[pianoKey] = null;
//         }
//
//         public static void InitConfig(ref KeySampleConfig[] configs)
//         {
//             configs = new KeySampleConfig[108];
//             for (int i = 0; i < 108; i++)
//             {
//                 configs[i] = new KeySampleConfig
//                 {
//                     Key = (PianoKey)i,
//
//                     Name = ((PianoKey)i).ToString()
//                 };
//             }
//         }
//
//
//         public class ComponentPool<TComponent> where TComponent : Component
//         {
//             private readonly GameObject _parent;
//             private readonly Queue<TComponent> _queue; //待使用的对象
//             private readonly Action<TComponent> _resetAction; //返回池中后调用
//             private readonly Action<TComponent> _initAction; //取出时调用
//             private readonly Action<ComponentPool<TComponent>, TComponent> _createAction; //首次生成时调用
//
//             public ComponentPool(GameObject parent, Action<TComponent> resetAction = null, Action<TComponent> initAction = null,
//                 Action<ComponentPool<TComponent>, TComponent> createAction = null)
//             {
//                 _parent = parent;
//                 _queue = new Queue<TComponent>();
//                 _resetAction = resetAction;
//                 _initAction = initAction;
//                 _createAction = createAction;
//             }
//
//             /// <summary>
//             /// 取出对象
//             /// </summary>
//             /// <returns></returns>
//             public TComponent New()
//             {
//                 if (_queue.Count > 0)
//                 {
//                     TComponent t = _queue.Dequeue();
//                     _initAction?.Invoke(t);
//                     return t;
//                 }
//                 else
//                 {
//                     TComponent t = _parent.AddComponent<TComponent>();
//                     _createAction?.Invoke(this, t);
//                     _initAction?.Invoke(t);
//
//                     return t;
//                 }
//             }
//
//             /// <summary>
//             /// 放回对象
//             /// </summary>
//             /// <param name="obj"></param>
//             public void Store(TComponent obj)
//             {
//                 if (_queue.Contains(obj) == false)
//                 {
//                     _resetAction?.Invoke(obj);
//                     _queue.Enqueue(obj);
//                 }
//             }
//         }
//     }
// }
