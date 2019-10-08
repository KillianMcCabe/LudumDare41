using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Mob : MonoBehaviour
{
    public System.Action OnDeath;

    private NavMeshAgent _agent = null;

    public virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 position)
    {
        if (_agent != null)
        {
            _agent.SetDestination(position);
        }
    }
}
