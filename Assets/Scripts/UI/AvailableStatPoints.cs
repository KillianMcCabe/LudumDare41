using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvailableStatPoints : MonoBehaviour
{
    [SerializeField]
    private Text _text = null;

    [SerializeField]
    private Animator _animator = null;

    private int _count = 0;
    public int Count
    {
        get { return _count; }
        set
        {
            _count = value;
            _text.text = _count.ToString();

            _animator.SetBool("Glow", (_count > 0));
        }
    }
}
