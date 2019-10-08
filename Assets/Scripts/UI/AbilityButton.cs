using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ability Data")]

    [SerializeField]
    private AbilityData _abilityData = null;

    [Header("Sub-components")]

    [SerializeField]
    private AbilityTimer _abilityCooldownTimer = null;

    [SerializeField]
    private GameObject _hint = null;

    [SerializeField]
    private Image _iconImage = null;

    [SerializeField]
    private Text _inputButtonText = null;

    [SerializeField]
    private Text _titleText = null;

    [SerializeField]
    private Text _descriptionText = null;

    [SerializeField]
    private Button _button = null;

    private bool _onCooldown = false;

    // Update is called once per frame
    void Update()
    {
        if (!_onCooldown)
        {
            bool requirementsFulfilled = true;
            // check requirement conditions e.g. TurretInRange, HoldingGift
            foreach (Requirement req in _abilityData.AbilityDataRequirements)
            {
                if (!CheckRequirement(req))
                {
                    requirementsFulfilled = false;
                }
            }

            _button.interactable = requirementsFulfilled;

            // check if input pressed
            if (requirementsFulfilled && Input.GetButtonDown(_abilityData.InputKey))
            {
                ActivateAbility();
            }
        }
        else
        {
            _button.interactable = false;
        }
    }

    public void Setup(AbilityData abilityData)
    {
        _abilityData = abilityData;

        _button.onClick.AddListener(ActivateAbility);

        _onCooldown = false;

        _hint.SetActive(false);
        _iconImage.sprite = _abilityData.Icon;

        _inputButtonText.text = _abilityData.InputCharacter;

        _titleText.text = _abilityData.Title;
        _descriptionText.text = _abilityData.Description;

        _abilityCooldownTimer.OnTimerEnd += HandleTimerEnd;
    }

    private bool CheckRequirement(Requirement req)
    {
        switch (req)
        {
            case Requirement.HasTurretInRange:
                return (GameController.Instance.Robot.TurretInInteractionRange != null);
            case Requirement.HoldingItem:
                return (GameController.Instance.Robot.HoldingItem);
            default:
                Debug.LogError("Unhandled ability requirement " + req);
                break;
        }
        return false;
    }

    private void ActivateAbility()
    {
        _abilityData.ActivateAbility();

        _onCooldown = true;
        _abilityCooldownTimer.StartTimer(_abilityData.CooldownDuration);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hint.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hint.SetActive(false);
    }

    private void HandleTimerEnd()
    {
        _onCooldown = false;
        // TODO: show refreshed ability animation
    }
}
