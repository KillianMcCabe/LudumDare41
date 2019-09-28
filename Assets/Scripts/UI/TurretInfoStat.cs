using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretInfoStat : MonoBehaviour
{
    public System.Action<StatType, int> OnStatLevelChange;

    [SerializeField]
    private StatType _statType = StatType.Unknown;
    public StatType StatType
    {
        get { return _statType; }
    }

    [SerializeField]
    private Text _levelText = null;

    [SerializeField]
    private Button _increaseStatButton = null;

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
        StatLevel ++;

        if (OnStatLevelChange != null)
        {
            OnStatLevelChange.Invoke(_statType, _statLevel);
        }
    }
}
