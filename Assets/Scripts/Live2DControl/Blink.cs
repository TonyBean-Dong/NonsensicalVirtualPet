using Live2D.Cubism.Framework;
using UnityEngine;

namespace Live2dControl
{
    public class Blink : MonoBehaviour
    {
        [SerializeField] private CubismEyeBlinkController m_eyeBlinkController;
        [SerializeField, Min(1)] private int m_idleTime = 950;
        [SerializeField, Min(1)] private int m_blinkTime = 50;

        private int _fullTime;
        private int _halfTime;
        private int _count;

        public bool Blinking
        {
            get => _blinking;
            set
            {
                if (value != _blinking)
                {
                    if (value)
                    {
                        _count = 0;
                    }
                    else
                    {
                        m_eyeBlinkController.EyeOpening = 1;
                    }

                    _blinking = value;
                }
            }
        }

        private bool _blinking = true;
        
        private void Awake()
        {
            m_idleTime = Mathf.Max(1, m_idleTime);
            m_blinkTime = Mathf.Max(1, m_blinkTime);
            _fullTime = m_idleTime + m_blinkTime;
            _halfTime = Mathf.Max(1, m_blinkTime / 2);
        }

        private void OnEnable()
        {
            if (m_eyeBlinkController == null) return;
            m_eyeBlinkController.EyeOpening = 1;
        }

        private void LateUpdate()
        {
            if (!_blinking || m_eyeBlinkController == null) return;

            _count++;
            var value = _count % _fullTime;
            if (value > 0 && value < m_blinkTime)
            {
                float realValue = Mathf.Abs(value - _halfTime) / (float)_halfTime;
                m_eyeBlinkController.EyeOpening = realValue;
            }
        }
    }
}
