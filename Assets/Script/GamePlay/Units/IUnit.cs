using UnityEngine;
using UnityEngine.AI;

public class IUnit : MonoBehaviour
{
    [SerializeField]
    protected AnimationClip m_Clip;
    [SerializeField]
    private NavMeshAgent m_NavigationAgent;
    [SerializeField]
    private GameObject Target;

    void Start()
    {

    }

    void Update()
    {
        m_NavigationAgent.SetDestination(Target.transform.position);
        m_NavigationAgent.isStopped = true;
    }
}
