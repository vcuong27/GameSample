using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class IUnit : MonoBehaviour
{
    protected AnimationClip m_Clip;
    private NavMeshAgent m_NavigationAgent;

    void Start()
    {
        
    }

    void Update()
    {
        m_NavigationAgent.Move(Vector3.zero);   
    }
}
