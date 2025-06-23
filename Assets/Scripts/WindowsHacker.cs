using System;
using System.Collections.Generic;
using NonsensicalKit.Core;
using NonsensicalKit.Windows.Hook;
//using NonsensicalKit.Windows.Tray;
using NonsensicalKit.Windows.Window;
using UnityEngine;
using Utils;
using Application = UnityEngine.Application;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WindowsHacker : NonsensicalMono
{
    [SerializeField] private Texture2D m_icon;
    [SerializeField] private string m_iconName;

    //private TrayIconModifier _tray;
    private MouseHooker _mouseHooker;
    private KeyboardHooker _keyboardHooker;
    private bool _closeFlag;

    private readonly HashSet<VirtualKeys> _pressedKeys = new HashSet<VirtualKeys>();

    private void Awake()
    {
        Application.targetFrameRate = 60;

        WindowModifier.TransparentWindow();

        var context = new List<(string, Action)>()
        {
            (TrayIcon.LEFT_CLICK, LeftKey),
            ("钢琴按键\\启用/禁用(F6)", () => ChangedPianoKeyState(null, null)),
            ("钢琴按键\\变调", () => ChangedPianoModifySwitch(null, null)),
            ("钢琴按键\\采样切换", () => ChangedPianoSample(null, null)),
            ("Midi音乐\\启用/禁用(F7)", () => ChangedMidiMusicState(null, null)),
            ("Midi音乐\\采样切换(F8)", () => ChangedMidiMusicSample(null, null)),
            ("Live2D\\启用/禁用(F9)", () => ChangedLive2DState(null, null)),
            ("关闭程序(F10)", () => CloseApplication(null, null))
        };

        TrayIcon.Init("Nonsensical", m_iconName, m_icon, context);

        // _tray = new TrayIconModifier("NonsensicalVirtualPet", Application.streamingAssetsPath + "/icon.png");
        // List<TrayIconMenuInfo> menus = new List<TrayIconMenuInfo>
        // {
        //     new() { Path = "钢琴按键\\启用/禁用(F6)", Callback = ChangedPianoKeyState },
        //     new() { Path = "钢琴按键\\变调", Callback = ChangedPianoModifySwitch },
        //     new() { Path = "钢琴按键\\采样切换", Callback = ChangedPianoSample },
        //     new() { Path = "Midi音乐\\启用/禁用(F7)", Callback = ChangedMidiMusicState },
        //     new() { Path = "Midi音乐\\采样切换(F8)", Callback = ChangedMidiMusicSample },
        //     new() { Path = "Live2D\\启用/禁用(F9)", Callback = ChangedLive2DState },
        //     new() { Path = "关闭程序(F10)", Callback = CloseApplication }
        // };
        // _tray.SetMenu(menus);

        _mouseHooker = new MouseHooker();
        _mouseHooker.MouseEvent += OnMouseEvent;
        _mouseHooker.StartHook();

        _keyboardHooker = new KeyboardHooker();
        _keyboardHooker.KeyboardEvent += OnKeyboardEvent;
        _keyboardHooker.StartHook();
    }

    private void Update()
    {
        if (_closeFlag)
        {
            if (PlatformInfo.IsEditor)
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        //_tray?.Dispose();
        _mouseHooker?.StopHook();
        _keyboardHooker?.StopHook();
    }

    private void LeftKey()
    {
        TrayIcon.ShowBalloonTip("Pop Up!", "You pressed Space!", TrayIcon.ToolTipIcon.Info);
    }

    private void ChangedPianoModifySwitch(object sender, EventArgs e)
    {
        Publish("ChangedPianoModifySwitch");
    }

    private void ChangedMidiMusicState(object sender, EventArgs e)
    {
        Publish("ChangedMidiMusicState");
    }

    private void ChangedMidiMusicSample(object sender, EventArgs e)
    {
        Publish("ChangedMidiMusicSample");
    }

    private void ChangedPianoKeyState(object sender, EventArgs e)
    {
        Publish("ChangedPianoKeyState");
    }

    private void ChangedPianoSample(object sender, EventArgs e)
    {
        Publish("ChangedPianoSample");
    }

    private void ChangedLive2DState(object sender, EventArgs e)
    {
        Publish("ChangedLive2DState");
    }

    private void CloseApplication(object sender, EventArgs e)
    {
        _closeFlag = true;
    }

    private void OnMouseEvent(HookMouseMessage message)
    {
        Publish("MouseEvent", message);
    }

    private void OnKeyboardEvent(HookKeyboardMessage message, VirtualKeys key)
    {
        if (message == HookKeyboardMessage.WM_KEYDOWN)
        {
            switch (key)
            {
                case VirtualKeys.F6:
                    Publish("ChangedPianoKeyState");
                    return;
                case VirtualKeys.F7:
                    Publish("ChangedMidiMusicState");
                    return;
                case VirtualKeys.F8:
                    Publish("ChangedMidiMusicSample");
                    return;
                case VirtualKeys.F9:
                    Publish("ChangedLive2DState");
                    return;
                case VirtualKeys.F10:
                    _closeFlag = true;
                    return;
            }

            if (_pressedKeys.Add(key) == false)
            {
                return;
            }
        }
        else if (message == HookKeyboardMessage.WM_KEYUP)
        {
            _pressedKeys.Remove(key);
        }

        Publish("KeyBoardEvent", message, key);
    }
}
