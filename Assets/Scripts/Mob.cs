using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Mob : MonoBehaviour
{
    private const float OutlineWidth = 0.15f;

    public System.Action OnDeath;

    [SerializeField]
    protected bool _canBeControlled = false;

    private Renderer[] _renderers = null;

    private NavMeshAgent _agent = null;

    public virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    public void MoveTo(Vector3 position)
    {
        if (_agent != null)
        {
            _agent.SetDestination(position);
        }
    }

    private void OnMouseEnter()
    {
        if (_canBeControlled)
        {
            // show outline
            foreach (Renderer renderer in _renderers)
            {
                renderer.material.SetFloat("_FirstOutlineWidth", OutlineWidth);
            }
        }
    }

    private void OnMouseExit()
    {
        if (_canBeControlled)
        {
            // hide outline
            foreach (Renderer renderer in _renderers)
            {
                renderer.material.SetFloat("_FirstOutlineWidth", 0);
            }
        }
    }
}
