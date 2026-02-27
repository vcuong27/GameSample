using UnityEngine;

public class IBuilding : MonoBehaviour
{
    public BuildingType buildingType;

    public Animator animator;

    public void Select()
    {
        animator.SetTrigger("Click");
    }

    public void Place()
    {
        animator.SetTrigger("Place");
    }

}
