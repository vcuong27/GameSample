using UnityEngine;

public class IBuilding : MonoBehaviour
{
    public BuildingType buildingType;

    protected BarrackData currentData;

    //Animator animator;
    public void Select()
    {
        //animator.SetTrigger("Click");
    }

    public void Place()
    {
        //animator.SetTrigger("Place");
    }


    //Battle data
    private int curentHP;
    private bool isDestroyed;

    public void InitBatle()
    {
        curentHP = currentData.MaxHP;
        isDestroyed = false;
    }

    public void TakeDamage(int damage)
    {
        curentHP -= damage;
        if (curentHP <= 0)
        {
            isDestroyed = true;
            Debug.Log("Barrack destroyed!");
        }
    }

    public bool IsDestroyed()
    {
        return isDestroyed;
    }

}
