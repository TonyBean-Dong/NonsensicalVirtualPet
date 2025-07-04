using System;
using System.Collections.Generic;
using NonsensicalKit.Core;
using NonsensicalKit.Windows.Hook;
using NonsensicalKit.Windows.Window;
using UnityEngine;
using Utils;
using Application = UnityEngine.Application;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AggregatorEnum]
public enum WindowsEvent
{
    MouseEvent = 1100,
    KeyBoardEvent,
}

public class MouseEvent
{
    public readonly HookMouseMessage MouseMessage;
    public MouseEvent(HookMouseMessage mouseMessage) { MouseMessage = mouseMessage; }
}

public class KeyEvent
{
    public readonly HookKeyboardMessage KeyboardMessage;
    public readonly VirtualKeys Key;

    public KeyEvent(HookKeyboardMessage keyboardMessage, VirtualKeys key)
    {
        KeyboardMessage = keyboardMessage;
        Key = key;
    }
}

public class WindowsHacker : NonsensicalMono
{
    [SerializeField] private Texture2D m_icon;
    [SerializeField] private string m_iconName;

    private MouseHooker _mouseHooker;
    private KeyboardHooker _keyboardHooker;
    private bool _closeFlag;

    private readonly HashSet<VirtualKeys> _pressedKeys = new HashSet<VirtualKeys>();
    private readonly Queue<MouseEvent> _mouseEvents = new();
    private readonly Queue<KeyEvent> _keyEvents = new();

    private void Awake()
    {
        Application.targetFrameRate = 60;

        WindowModifier.TransparentWindow();

        TrayIcon.Init("Nonsensical", m_iconName, m_icon);
        TrayIcon.GetMenuActions = GetMenu;

        _mouseHooker = new MouseHooker();
        _mouseHooker.MouseEvent += OnMouseEvent;
        _mouseHooker.StartHook();

        _keyboardHooker = new KeyboardHooker();
        _keyboardHooker.KeyboardEvent += OnKeyboardEvent;
        _keyboardHooker.StartHook();
    }

    private List<(string,int, Action)> GetMenu()
    {
        var v = IOCC.GetAll<List<(string,int, Action)>>("TrayMenu");
        var result = new List<(string,int, Action)>();
        foreach (var item in v)
        {
            result.AddRange(item);
        }

        result.Add(("关闭程序(F10)", 1000,() => _closeFlag = true));
        return result;
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

            return;
        }

        while (_mouseEvents.Count > 0)
        {
            Publish(WindowsEvent.MouseEvent, _mouseEvents.Dequeue());
        }

        while (_keyEvents.Count > 0)
        {
            var keyEvent = _keyEvents.Dequeue();
            if (keyEvent.KeyboardMessage == HookKeyboardMessage.WM_KEYDOWN)
            {
                switch (keyEvent.Key)
                {
                    case VirtualKeys.F6:
                        Publish(PianoEvent.ChangePianoKeyState);
                        return;
                    case VirtualKeys.F7:
                        Publish(MidiMusicEvent.ChangeMidiMusicState);
                        return;
                    case VirtualKeys.F8:
                        Publish(MidiMusicEvent.ChangeMidiMusicSample);
                        return;
                    case VirtualKeys.F9:
                        Publish(Live2DEvent.ChangedLive2DState);
                        return;
                    case VirtualKeys.F10:
                        _closeFlag = true;
                        return;
                }

                if (_pressedKeys.Add(keyEvent.Key) == false)
                {
                    return;
                }
            }
            else if (keyEvent.KeyboardMessage == HookKeyboardMessage.WM_KEYUP)
            {
                _pressedKeys.Remove(keyEvent.Key);
            }

            Publish(WindowsEvent.KeyBoardEvent, keyEvent);
        }
    }

    private void OnMouseEvent(HookMouseMessage message)
    {
        _mouseEvents.Enqueue(new MouseEvent(message));
    }

    private void OnKeyboardEvent(HookKeyboardMessage message, VirtualKeys key)
    {
        _keyEvents.Enqueue(new KeyEvent(message, key));
    }
}
