using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySlider : MonoBehaviour
{
    [SerializeField]
    Slider _sensititySlider = null;

    private const float MinMouseSensitivity = 0.1f;
    private const float MaxMouseSensitivity = 2f;

    private void Awake()
    {
        _sensititySlider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void OnEnable()
    {
        _sensititySlider.normalizedValue = (Settings.MouseSpeed - MinMouseSensitivity) / (MaxMouseSensitivity - MinMouseSensitivity);
    }

    private void HandleSliderValueChanged(float value)
    {
        Settings.MouseSpeed = MinMouseSensitivity + ((MaxMouseSensitivity - MinMouseSensitivity) * _sensititySlider.normalizedValue);

        // TODO: save to playerprefs
    }
}
