using System;
using System.Collections.Generic;
using NonsensicalKit.Core;
using NonsensicalKit.Windows.Hook;
using UnityEngine;

[AggregatorEnum]
public enum Live2DEvent
{
    ChangedLive2DState = 1200,
}

public class Live2DManager : NonsensicalMono
{
    [SerializeField] private GameObject m_model;
    [SerializeField] private GameObject m_live2D;
    [SerializeField] private Camera m_camera;
    [SerializeField] private bool m_initShow;

    private bool _isDown;
    private bool _checkDown;
    private int _state;
    private Camera _runtimeCamera;

    private void Awake()
    {
        Subscribe(Live2DEvent.ChangedLive2DState, ChangedState);
        Subscribe<MouseEvent>(WindowsEvent.MouseEvent, OnMouseEvent);
        IOCC.Register("TrayMenu", GetMenu);
        _state = m_initShow ? 0 : 2;
        _runtimeCamera = m_camera != null ? m_camera : Camera.main;
    }

    private List<(string, int, Action)> GetMenu()
    {
        string stateString = (_state % 3) switch
        {
            0 => "常规显示",
            1 => "一直显示",
            2 => "隐藏",
            _ => "未知状态"
        };
        return new List<(string, int, Action)>() { ($@"Live2D\切换状态({stateString})(F9)", 900, ChangedState) };
    }

    private void Update()
    {
        if (_runtimeCamera == null || m_live2D == null || m_model == null) return;

        if (_checkDown == true)
        {
            _checkDown = false;

            _isDown = IsMouseOnBody();
        }

        switch (_state % 3)
        {
            case 0:
            {
                //状态0，当鼠标放置到live2d对象上时隐藏对象
                m_live2D.gameObject.SetActive(!IsMouseOnBody());

                break;
            }
            case 1:
            {
                //状态1,一直显示，可以用鼠标拖拽live2d对象
                m_live2D.gameObject.SetActive(true);
                if (_isDown == true)
                {
                    m_model.transform.position = Vector3.Scale(_runtimeCamera.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0));
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

    private void ChangedState()
    {
        _state++;
    }

    private bool IsMouseOnBody()
    {
        var worldPosition = _runtimeCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        return hit.transform != null && hit.transform.name == "Body";
    }

    private void OnMouseEvent(MouseEvent @event)
    {
        switch (@event.MouseMessage)
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
}
