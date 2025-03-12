using System;
using NonsensicalKit.Core;
using NonsensicalKit.Windows.Hook;
using NonsensicalKit.Windows.Tray;
using NonsensicalKit.Windows.Window;
using UnityEngine;

public class Live2DManager : NonsensicalMono
{
    [SerializeField] private Live2DController m_live2D;

    private bool _isDown;
    private bool _checkDown;
    private int _state;
    private MouseHooker _mouseHooker;
    private KeyboardHooker _keyboardHooker;
    private bool _closeFlag;

    private TrayIconModifier _tray;

    private void Awake()
    {
        Application.targetFrameRate = 30;

        WindowModifier.TransparentWindow();

        _tray = new TrayIconModifier("NonsensicalVirtualPet", Application.streamingAssetsPath + "/icon.png");
        TrayIconMenuInfo[] menus = new TrayIconMenuInfo[2];
        menus[0] = new TrayIconMenuInfo() { Path = "显示-隐藏", Callback = ChangedState };
        menus[1] = new TrayIconMenuInfo() { Path = "关闭", Callback = CloseApplication };
        _tray.SetMenu(menus);

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
            Application.Quit();
        }

        if (_checkDown == true)
        {
            _checkDown = false;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.transform == null)
            {
                _isDown = false;
            }
            else
            {
                _isDown = hit.transform.name == "Body";
            }
        }

        switch (_state % 3)
        {
            case 0:
            {
                //状态0，当鼠标放置到live2d对象上时隐藏对象
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.transform == null)
                {
                    m_live2D.gameObject.SetActive(true);
                }
                else if (hit.transform.name == "Body")
                {
                    m_live2D.gameObject.SetActive(false);
                }
                else
                {
                    m_live2D.gameObject.SetActive(true);
                }

                break;
            }
            case 1:
            {
                //状态1,一直显示，可以用鼠标拖拽live2d对象
                m_live2D.gameObject.SetActive(true);
                if (_isDown == true)
                {
                    gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }

                break;
            }
            case 2:
            {
                //状态2，隐藏live2d对象
                m_live2D.gameObject.SetActive(false);
                break;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _tray.Dispose();
        _mouseHooker.StopHook();
        _keyboardHooker.StopHook();
    }


    private void ChangedState(object sender, EventArgs e)
    {
        _state++;
    }

    private void CloseApplication(object sender, EventArgs e)
    {
        _closeFlag = true;
    }


    private void OnMouseEvent(HookMouseMessage message)
    {
        switch (message)
        {
            case HookMouseMessage.WM_RBUTTONDOWN:
            {
                if (_state % 3 == 1)
                {
                    _checkDown = true;
                }

                break;
            }
            case HookMouseMessage.WM_RBUTTONUP:
            {
                _isDown = false;
                break;
            }
        }
    }

    private void OnKeyboardEvent(HookKeyboardMessage message, VirtualKeys key)
    {
        if (message == HookKeyboardMessage.WM_KEYDOWN)
        {
            switch (key)
            {
                case VirtualKeys.F9:
                {
                    _state++;
                    break;
                }
                case VirtualKeys.F10:
                {
                    _closeFlag = true;
                    break;
                }
            }
        }
    }
}
