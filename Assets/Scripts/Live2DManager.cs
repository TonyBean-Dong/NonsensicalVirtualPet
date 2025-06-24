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
    [SerializeField] private Live2DController m_live2D;
    [SerializeField] private bool m_initShow;

    private bool _isDown;
    private bool _checkDown;
    private int _state;

    private void Awake()
    {
        Subscribe(Live2DEvent.ChangedLive2DState, ChangedState);
        Subscribe<MouseEvent>(WindowsEvent.MouseEvent, OnMouseEvent);
        _state = m_initShow ? 0 : 2;
    }

    private void Update()
    {
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

    private void ChangedState()
    {
        _state++;
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
