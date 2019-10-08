using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private const float GroundLifetimeDuration = 60f;
    private const float TimeTilCanBePickedUp = 0.25f;
    private const float ItemGlowEffectYPosition = 0.25f;

    [SerializeField]
    private GameObject _itemGlowEffectPrefab = null;

    [SerializeField]
    private string _label = "";
    public string Label
    {
        get { return _label; }
    }

    private bool _canBePickedUp = false;
    public bool CanBePickedUp
    {
        get { return _canBePickedUp; }
    }

    private bool _isHeld = false;
    public bool IsHeld
    {
        get { return _isHeld; }
        set
        {
            _isHeld = value;

            if (!_isHeld)
            {
                _canBePickedUp = false;
                _t = 0f;
            }

            _rb.useGravity = !_isHeld;
            _rb.isKinematic = _isHeld;
            _collider.enabled = !_isHeld;

            _itemGlowEffect.SetActive(!_isHeld);
        }
    }

    private GameObject _itemGlowEffect;
    private Rigidbody _rb = null;
    private Collider _collider = null;

    private float _t = 0f;

    // Use this for initialization
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _itemGlowEffect = Instantiate(_itemGlowEffectPrefab, gameObject.transform) as GameObject;
        _itemGlowEffect.transform.position = new Vector3(transform.position.x, ItemGlowEffectYPosition, transform.position.z);
    }

    private void Update()
    {
        if (!_isHeld)
        {
            if (_t > TimeTilCanBePickedUp && !_canBePickedUp)
            {
                _canBePickedUp = true;

                // reset collider
                _collider.enabled = true;
            }
            if (_t > GroundLifetimeDuration)
            {
                Destroy(gameObject);
            }
            _t += Time.deltaTime;

            _itemGlowEffect.transform.position = new Vector3(transform.position.x, ItemGlowEffectYPosition, transform.position.z);
        }
    }

    public void Throw(Vector3 force)
    {
        IsHeld = false;
        transform.SetParent(null);
        _rb.AddForce(force, ForceMode.VelocityChange);
    }
}
