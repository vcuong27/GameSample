using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;
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
    }
}
