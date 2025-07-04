using Live2D.Cubism.Framework.Expression;
using Live2dControl;
using UnityEngine;

public class BangbooController : MonoBehaviour
{
    enum Expression
    {
        默认 = -1,
        左折耳 = 0,
        闭眼,
        紧张,
        难过,
        微笑,
        生气,
        右折耳,
        待机,
    }

    [SerializeField] private CubismExpressionController m_expressionController;
    [SerializeField] private Transform m_headPoint;
    [SerializeField] private float m_touchDistance = 1;
    [SerializeField] private FollowMouse m_follow;
    [SerializeField] private Breath m_breath;
    [SerializeField] private Blink m_blink;

    private bool _touching;

    private void Update()
    {
        var touching = Vector3.Distance(m_headPoint.position, m_follow.transform.position) <= m_touchDistance;
        if (touching != _touching)
        {
            m_follow.Follow = !touching;
            m_breath.Breathing = !touching;
            m_blink.Blinking = !touching;
            ChangeExpression(touching ? Expression.闭眼 : Expression.默认);

            _touching = touching;
        }
    }

    private void ChangeExpression(Expression exp)
    {
        m_expressionController.CurrentExpressionIndex = (int)exp;
    }
}
