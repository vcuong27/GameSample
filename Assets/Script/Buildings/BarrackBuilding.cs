using UnityEngine;
using UnityEngine.UI;

public class BarrackBuilding : IBuilding
{
    enum BarrackState
    {
        Idle,
        Training,
        ReadyToCollect
    }


    [SerializeField]
    private GameObject BarrackUI;
    [SerializeField]
    private Slider BarrackValue;

    private BarrackState barrackState;
    private BarrackData currentData;
    private float percent;
    private float trainingTime;


    void Start()
    {
        buildingType = BuildingType.Barracks;
        currentData = new BarrackData();
        currentData.TrainingTime = 10;
        percent = 0f;
        trainingTime = 0f;
        barrackState = BarrackState.Idle;
    }

    public void Initialize(BarrackData data)
    {
        currentData = data;
        percent = 0f;
        trainingTime = 0f;
    }

    private void Update()
    {
       

        switch (barrackState)
        {
            case BarrackState.Idle:
                break;
            case BarrackState.Training:
                trainingTime += Time.deltaTime;
                if (trainingTime < currentData.TrainingTime)
                {
                    percent = trainingTime / currentData.TrainingTime;
                    BarrackValue.value = percent;
                }
                else
                {
                    BarrackValue.value = 1f;
                    barrackState = BarrackState.ReadyToCollect;
                }
                break;
            case BarrackState.ReadyToCollect:
                break;
            default:
                break;
        }

    }

    public void Confirm()
    {
        Debug.Log("Confirm");
        barrackState = BarrackState.Training;
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        barrackState = BarrackState.Idle;
        percent = 0f;
        trainingTime = 0f;
        BarrackValue.value = 0f;
    }

    public void Collect()
    {
        if (percent >= 1f)
        {
            Debug.Log("Unit trained and ready to deploy!");
            percent = 0f;
            trainingTime = 0f;
            BarrackValue.value = 0f;
        }
        else
        {
            Debug.Log("Training in progress. Please wait until it's complete.");
        }
    }


}
