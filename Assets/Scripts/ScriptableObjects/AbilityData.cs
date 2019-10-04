using UnityEngine;

public enum Requirement
{
    None,
    HasTurretInRange,
    HoldingItem
}

[CreateAssetMenu(fileName = "Ability Data", menuName = "TDDS/Ability", order = 0)]
public class AbilityData : ScriptableObject
{
    public System.Action OnAbilityActivated;

    public string InputKey = null;
    public string InputCharacter = null;

    public float CooldownDuration = 0;
    public Sprite Icon = null;

    public Requirement[] AbilityDataRequirements;

    public string Title = null;
    [TextArea]
    public string Description = null;

    public void Init(string InputKey)
    {
        this.InputKey = InputKey;
    }

    public void ActivateAbility()
    {
        if (OnAbilityActivated != null)
        {
            OnAbilityActivated.Invoke();
        }
    }
}