using System;
using Live2D.Cubism.Framework.LookAt;
using NonsensicalKit.Tools;
using UnityEngine;

namespace Live2dControl
{
    public class FollowMouse : MonoBehaviour, ICubismLookTarget
    {
        [SerializeField] private Camera m_camera; //非透视相机
        [SerializeField] private Transform m_plane;

        public bool Follow
        {
            get => _follow;
            set => _follow = value;
        }
        private bool _follow;
        
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
        }

        private void Update()
        {
            var mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);

            var pos = VectorTool.GetLinePlaneCrossPoint(mousePos, mousePos + m_camera.transform.forward,
                new Plane(m_plane.forward, m_plane.position));
            if (pos != null)
            {
                transform.position = (Vector3)pos;
            }
        }

        public Vector3 GetPosition()
        {
            return _follow ? transform.position : _startPos;
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}
