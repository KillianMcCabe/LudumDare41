using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurretInfoStat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public System.Action<StatData, int> OnStatLevelIncrease;

    private StatData _statData = null;
    public StatData StatData
    {
        get { return _statData; }
    }

    [SerializeField]
    private Text _statNameText = null;

    [SerializeField]
    private Text _levelText = null;

    [SerializeField]
    private Button _increaseStatButton = null;

    [SerializeField]
    private UIHint _hint = null;

    public void Init(StatData statData)
    {
        _statData = statData;
        _statNameText.text = statData.Title;
        _hint.Init(statData.Title, statData.Description);
        _hint.gameObject.SetActive(false);
    }

    public void ShowEditButtons(bool show)
    {
        _increaseStatButton.gameObject.SetActive(show);
    }

    private int _statLevel = 0;
    public int StatLevel
    {
        get { return _statLevel; }
        set
        {
            if (value < 0)
                return;

            _statLevel = value;
            _levelText.text = _statLevel.ToString();
        }
    }

    private void Awake()
    {
        _increaseStatButton.onClick.AddListener(HandleIncreaseStatButtonClick);
    }

    private void HandleIncreaseStatButtonClick()
    {
        if (OnStatLevelIncrease != null)
        {
            OnStatLevelIncrease.Invoke(_statData, _statLevel);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hint.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hint.gameObject.SetActive(false);
    }
}
