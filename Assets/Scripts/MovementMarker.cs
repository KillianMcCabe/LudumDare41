using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMarker : MonoBehaviour
{
    private const float ArrowSpeed = 1.0f;
    private const float Lifetime = 4.0f;

    [SerializeField]
    private Renderer _arrowRenderer = null;

    private float t = 0;
    private Material _scrollingArrowMaterial;

    private void Start()
    {
        _scrollingArrowMaterial = _arrowRenderer.material;
    }

    private void OnEnable()
    {
        t = 0;
    }

    private void Update()
    {
        t += Time.deltaTime;
        _scrollingArrowMaterial.SetFloat("_ScrollValue", t * ArrowSpeed);

        if (t >= Lifetime)
        {
            Destroy(gameObject);
        }
    }
}
