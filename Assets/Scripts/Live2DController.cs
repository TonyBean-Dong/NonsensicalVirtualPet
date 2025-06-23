using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using UnityEngine;

public class Live2DController : MonoBehaviour
{
    [SerializeField]
    Transform Anchor = null;

    [SerializeField]
    CubismParameter HeadAngleX = null, HeadAngleY = null, HeadAngleZ = null;

    [SerializeField]
    CubismParameter EyeBallX = null, EyeBallY = null;

    [SerializeField]
    CubismParameter EyeLOpen = null, EyeROpen = null;

    [SerializeField]
    CubismParameter PraramBreath;


    [SerializeField]
    float EaseTime = 0.2f;

    [SerializeField]
    float EyeBallXRate = 0.05f;

    [SerializeField]
    float EyeBallYRate = 0.02f;

    [SerializeField]
    bool ReversedGazing = false;

    Vector3 currentRotateion = Vector3.zero;
    Vector3 eulerVelocity = Vector3.zero;

    private void OnEnable()
    {
        SetParameter(EyeLOpen, 1);
        SetParameter(EyeROpen, 1);
    }

    private void ChangeOpacity()
    {
        if (transform.GetComponent<CubismRenderController>().Opacity == 0)
        {
            transform.GetComponent<CubismRenderController>().Opacity = 1;
        }
        else
        {
            transform.GetComponent<CubismRenderController>().Opacity = 0;
        }
    }

    private void SwitchOpacity(bool isOn)
    {
        if (isOn == true)
        {
            transform.GetComponent<CubismRenderController>().Opacity = 1;
        }
        else
        {
            transform.GetComponent<CubismRenderController>().Opacity = 0;
        }
    }

    void LateUpdate()
    {
        var centerOnScreen = Camera.main.WorldToScreenPoint(Anchor.position);
        var mousePos = Input.mousePosition - centerOnScreen;
        UpdateRotate(new Vector3(mousePos.x, mousePos.y, 0) * 0.2f);

        Breath(Time.frameCount);
        ChangeOpen(Time.frameCount);
    }

    void UpdateRotate(Vector3 targetEulerAngle)
    {
        currentRotateion = Vector3.SmoothDamp(currentRotateion, targetEulerAngle, ref eulerVelocity, EaseTime);
        // 頭の角度
        SetParameter(HeadAngleX, currentRotateion.x);
        SetParameter(HeadAngleY, currentRotateion.y);
        SetParameter(HeadAngleZ, currentRotateion.z);
        // 眼球の向き
        SetParameter(EyeBallX, currentRotateion.x * EyeBallXRate * (ReversedGazing ? -1 : 1));
        SetParameter(EyeBallY, currentRotateion.y * EyeBallYRate * (ReversedGazing ? -1 : 1));
    }

    private void Breath(int value)
    {
        value %= 500;
        float realValue = Mathf.Abs(value - 250) / (float)250;
        SetParameter(PraramBreath, realValue);
    }

    private void ChangeOpen(int value)
    {
        value = value % 1000;
        if (value > 0 && value < 50)
        {
            float realValue = Mathf.Abs(value - 25) / (float)25;
            SetParameter(EyeLOpen, realValue);
            SetParameter(EyeROpen, realValue);
        }
    }

    void SetParameter(CubismParameter parameter, float value)
    {
        if (parameter != null)
        {
            parameter.Value = Mathf.Clamp(value, parameter.MinimumValue, parameter.MaximumValue);
        }
    }
}
