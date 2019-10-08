using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTimer : MonoBehaviour
{
    public System.Action OnTimerEnd = null;

    [SerializeField]
    private Image _filledImage = null;

    [SerializeField]
    private Text _timerText = null;

    private float _time = 0;
    private float _duration = 0;

    public void StartTimer(float duration)
    {
        _duration = duration;
        _time = 0;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (_time < _duration)
        {
            _timerText.text = (_duration - _time).ToString("F0");
            _filledImage.fillAmount = 1 - (_time / _duration);
            _time += Time.deltaTime;
        }
        else
        {
            _filledImage.fillAmount = 0;
            gameObject.SetActive(false);

            if (OnTimerEnd != null)
            {
                OnTimerEnd.Invoke();
            }
        }
    }
}
