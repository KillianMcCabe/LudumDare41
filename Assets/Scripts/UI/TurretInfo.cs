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

    private Turret _turret = null;

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
            _turret.OnChange -= UpdateTurretInfo;
        }

        // subscribe to new turret
        _turret = turret;
        _turret.OnChange += UpdateTurretInfo;

        UpdateTurretInfo();
    }

    private void UpdateTurretInfo()
    {
        _name.text = _turret.Name;
        _level.text = _turret.Level.ToString();

        _likes.text = _turret.FoundLike ? _turret.Likes : "???";
        _dislikes.text = _turret.FoundDislike ? _turret.Dislikes : "???";

        _statSpeed.text = _turret.StatTurnSpeed.ToString();
        _statDamage.text = _turret.StatDps.ToString();
        _statFortitude.text = _turret.StatFortitude.ToString();
        _statRange.text = _turret.StatRange.ToString();
        _statRomance.text = _turret.StatRomance.ToString();
    }
}
