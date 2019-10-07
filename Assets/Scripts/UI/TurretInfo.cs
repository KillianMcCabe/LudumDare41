using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretInfo : MonoBehaviour
{
    [SerializeField]
    private Text _name = null;

    [SerializeField]
    private Text _level = null;

    [SerializeField]
    private Text _hp = null;

    [SerializeField]
    private Image _hpImage = null;

    [SerializeField]
    private Text _likes = null;

    [SerializeField]
    private Text _dislikes = null;

    [SerializeField]
    private Transform _statsInfoHolderTransform = null;

    [SerializeField]
    private AvailableStatPoints _availableStatPoints = null;

    [Header("Prefabs")]

    [SerializeField]
    private TurretInfoStat _statsInfoPrefab = null;

    [SerializeField]
    private StatData[] _statDatas = null;

    private Dictionary<StatData, TurretInfoStat> _statInteractables = new Dictionary<StatData, TurretInfoStat>();
    private Turret _turret = null;

    private void Awake()
    {
        foreach (StatData stat in _statDatas)
        {
            TurretInfoStat statInteractable = Instantiate(_statsInfoPrefab, _statsInfoHolderTransform);
            statInteractable.Init(stat);
            statInteractable.OnStatLevelIncrease += HandleStatLevelIncrease;

            _statInteractables.Add(stat, statInteractable);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void DisplayInfoForTurret(Turret turret)
    {
        if (_turret == turret)
        {
            return;
        }

        // first unsubscribe previous turret
        if (_turret != null)
        {
            _turret.OnChange -= UpdateView;
        }

        // subscribe to new turret
        _turret = turret;
        _turret.OnChange += UpdateView;

        UpdateView();
    }

    private void UpdateView()
    {
        if (_turret == null)
        {
            Debug.LogWarning("Turret should not be null");
            return;
        }

        _name.text = _turret.Name;
        _level.text = "Lv. " + _turret.Level.ToString();
        _hp.text = $"Hp: {Mathf.Ceil(_turret.Health)} / {Mathf.Ceil(_turret.MaxHealth)}";
        _hpImage.fillAmount = Mathf.Ceil(_turret.Health) / Mathf.Ceil(_turret.MaxHealth);

        _likes.text = _turret.FoundLike ? _turret.Likes : "???";
        _dislikes.text = _turret.FoundDislike ? _turret.Dislikes : "???";

        _availableStatPoints.Count = _turret.AvailableStatPoints;

        foreach (StatData stat in _statInteractables.Keys)
        {
            TurretInfoStat statInteractable = _statInteractables[stat];

            // set stat level
            statInteractable.StatLevel = _turret.GetStatLevel(stat);

            bool editingStats = (_turret.AvailableStatPoints > 0);
            statInteractable.ShowEditButtons(_turret.AvailableStatPoints > 0);
        }
    }

    private void HandleStatLevelIncrease(StatData statData, int level)
    {
        // descreas available stat points
        _turret.AvailableStatPoints --;

        // increase stat level
        int statLevel = _turret.GetStatLevel(statData);
        _turret.SetStatLevel(statData, statLevel + 1);

        UpdateView();
    }
}
