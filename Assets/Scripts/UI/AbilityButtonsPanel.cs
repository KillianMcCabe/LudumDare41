using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButtonsPanel : MonoBehaviour
{
    [SerializeField]
    private AbilityData[] abilities;

    [SerializeField]
    private AbilityButton _abilityButtonPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (AbilityData abilityData in abilities)
        {
            AbilityButton abilityButton = Instantiate(_abilityButtonPrefab, transform);
            abilityButton.Setup(abilityData);
        }
    }
}
