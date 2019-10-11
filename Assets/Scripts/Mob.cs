using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Mob : MonoBehaviour
{
    private const float OutlineWidth = 0.15f;

    public System.Action OnDeath;

    [SerializeField]
    protected bool _canBeControlled = false;

    [SerializeField]
    protected GameObject _dottedUnitCircle = null;

    [SerializeField]
    protected GameObject _unitCircle = null;

    private Renderer[] _renderers = null;

    private NavMeshAgent _agent = null;

    private bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;

            UpdateLines();
        }
    }

    private bool _hovered = false;
    public bool Hovered
    {
        get { return _hovered; }
        set
        {
            _hovered = value;

            UpdateLines();
        }
    }

    public virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _renderers = GetComponentsInChildren<Renderer>();

        Selected = false;
        Hovered = false;
    }

    public void MoveTo(Vector3 position)
    {
        if (_agent != null)
        {
            _agent.SetDestination(position);
        }
    }

    private void UpdateLines()
    {
        if (_unitCircle != null)
            _unitCircle.SetActive(_selected);

        if (_dottedUnitCircle != null)
            _dottedUnitCircle.SetActive(!_selected && _hovered);
    }

    private void OnMouseEnter()
    {
        if (_canBeControlled)
        {
            Hovered = true;
            // show outline
            // foreach (Renderer renderer in _renderers)
            // {
            //     renderer.material.SetFloat("_FirstOutlineWidth", OutlineWidth);
            // }
        }
    }

    private void OnMouseExit()
    {
        if (_canBeControlled)
        {
            Hovered = false;
        }
    }
}
