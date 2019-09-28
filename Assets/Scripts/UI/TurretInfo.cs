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
    private Text _likes = null;

    [SerializeField]
    private Text _dislikes = null;

    [SerializeField]
    private Text _statSpeed = null;

    [SerializeField]
    private Text _statDamage = null;

    [SerializeField]
    private Text _statFortitude = null;

    [SerializeField]
    private Text _statRange = null;

    [SerializeField]
    private Text _statRomance = null;

    [SerializeField]
    private TurretInfoStat[] _statInteractables = null;

    [SerializeField]
    private AvailableStatPoints _availableStatPoints = null;

    [SerializeField]
    private Button _confirmStatButton = null;

    [SerializeField]
    private Text _statsPromtText = null;

    private Turret _turret = null;

    private bool _editingStats = false;

    private void Awake()
    {
        foreach (TurretInfoStat statInteractable in _statInteractables)
        {
            statInteractable.OnStatLevelChange += HandleStatLevelChange;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("ExamineTurret"))
        {
            if (_availableStatPoints.Count <= 0)
            {
                EditStats(false);
            }
            else
            {
                EditStats(!_editingStats);
            }
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

    public void EditStats(bool edit)
    {
        _editingStats = edit;

        UpdateView();
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

        _likes.text = _turret.FoundLike ? _turret.Likes : "???";
        _dislikes.text = _turret.FoundDislike ? _turret.Dislikes : "???";

        _availableStatPoints.Count = _turret.AvailableStatPoints;

        _statSpeed.text = _turret.StatTurnSpeed.ToString();
        _statDamage.text = _turret.StatDps.ToString();
        _statFortitude.text = _turret.StatFortitude.ToString();
        _statRange.text = _turret.StatRange.ToString();
        _statRomance.text = _turret.StatRomance.ToString();

        if (_turret.AvailableStatPoints <= 0)
        {
            // disable stat buttons
            _editingStats = false;
        }

        // _confirmStatButton.gameObject.SetActive(_editingStats);
        foreach (TurretInfoStat statInteractable in _statInteractables)
        {
            statInteractable.ShowEditButtons(_editingStats);

            switch (statInteractable.StatType)
            {
                case StatType.Range:
                    statInteractable.StatLevel = _turret.StatRange;
                    break;
                case StatType.Fortitude:
                    statInteractable.StatLevel = _turret.StatFortitude;
                    break;
                case StatType.Speed:
                    statInteractable.StatLevel = _turret.StatTurnSpeed;
                    break;
                case StatType.Damage:
                    statInteractable.StatLevel = _turret.StatDps;
                    break;
                case StatType.Romance:
                    statInteractable.StatLevel = _turret.StatRomance;
                    break;
                default:
                    Debug.LogError("Unhandled StatType \"" + statInteractable.StatType + "\"");
                    break;
            }
        }

        // update promt text
        _statsPromtText.gameObject.SetActive(_turret.AvailableStatPoints > 0 && !_editingStats);

        // update cursor
        if (_editingStats)
        {
            CursorManager.SetCursorState(CursorLockMode.None);
        }
        else
        {
            CursorManager.SetCursorState(CursorLockMode.Locked);
        }
    }

    private void HandleStatLevelChange(StatType statType, int level)
    {
        _turret.AvailableStatPoints --;

        // apply new stat points
        _turret.ChangeStat(statType, level);

        UpdateView();
    }
}
