using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2dControl
{
    public class Breath : MonoBehaviour
    {
        [SerializeField] private CubismParameter m_paramBreath;

        [SerializeField, Min(1)] private int m_idleTime = 500;
        [SerializeField, Min(1)] private int m_breathTime = 500;

        private int _count;

        public bool Breathing
        {
            get => _breathing;
            set
            {
                if (value != _breathing)
                {
                    if (value)
                    {
                        _count = 0;
                    }
                    else
                    {
                        m_paramBreath.Value = Mathf.Clamp(1, m_paramBreath.MinimumValue, m_paramBreath.MaximumValue);
                    }

                    _breathing = value;
                }
            }
        }

        private bool _breathing = true;
        private int _fullTime;
        private int _halfTime;

        private void Awake()
        {
            m_idleTime = Mathf.Max(1, m_idleTime);
            m_breathTime = Mathf.Max(1, m_breathTime);
            _fullTime = m_idleTime + m_breathTime;
            _halfTime = Mathf.Max(1, m_breathTime / 2);
        }

        private void OnEnable()
        {
            if (m_paramBreath == null) return;
            m_paramBreath.Value = Mathf.Clamp(1, m_paramBreath.MinimumValue, m_paramBreath.MaximumValue);
        }

        private void LateUpdate()
        {
            if (!_breathing || m_paramBreath == null) return;

            _count++;
            var value = _count % _fullTime;
            if (value > 0 && value < m_breathTime)
            {
                float realValue = Mathf.Abs(value - _halfTime) / (float)_halfTime;
                m_paramBreath.Value = Mathf.Clamp(realValue, m_paramBreath.MinimumValue, m_paramBreath.MaximumValue);
            }
        }
    }
}
