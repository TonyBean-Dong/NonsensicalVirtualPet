// using Live2D.Cubism.Core;
// using UnityEngine;
//
// public class Breath : MonoBehaviour
// {
//     [SerializeField] CubismParameter PraramBreath;
//
//     [SerializeField] private int m_idleTime = 500;
//     [SerializeField] private int m_breathTime = 500;
//
//     private int _fullTime;
//     private int _halfTime;
//
//     private void Awake()
//     {
//         _fullTime = m_idleTime + m_breathTime;
//         _halfTime = m_breathTime / 2;
//     }
//
//     private void OnEnable()
//     {
//         SetParameter(PraramBreath, 1);
//     }
//
//     private void LateUpdate()
//     {
//         var value = Time.frameCount;
//         value = value % _fullTime;
//         if (value > 0 && value < m_breathTime)
//         {
//             float realValue = Mathf.Abs(value - _halfTime) / (float)_halfTime;
//             SetParameter(PraramBreath, realValue);
//         }
//     }
//
//     void SetParameter(CubismParameter parameter, float value)
//     {
//         if (parameter != null)
//         {
//             parameter.Value = Mathf.Clamp(value, parameter.MinimumValue, parameter.MaximumValue);
//         }
//     }
// }
