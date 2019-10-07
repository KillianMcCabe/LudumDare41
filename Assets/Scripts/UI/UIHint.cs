using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHint : MonoBehaviour
{
    [SerializeField]
    private Text _title = null;

    [SerializeField]
    private Text _description = null;

    public void Init(string title, string description)
    {
        _title.text = title;
        _description.text = description;
    }
}
