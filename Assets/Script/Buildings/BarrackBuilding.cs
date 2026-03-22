using System;
using UnityEngine;
using UnityEngine.UI;

public class BarrackBuilding : IBuilding
{
    public enum BarrackState
    {
        Idle,
        Constructing,
        Training,
        ReadyToCollect
    }


    [SerializeField]
    private BarrackBuildingUI BarrackUI;

    private BarrackState barrackState;
    private BarrackData currentData;
    private float percent;
    private float trainingTime;

    private DateTime ConstructingTime;

    void Start()
    {
        buildingType = BuildingType.BARRACKS;
        currentData = new BarrackData();
        percent = 0f;
        trainingTime = 0f;
        barrackState = BarrackState.Idle;
        BarrackUI.Initialize(this);
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
            case BarrackState.Constructing:
                DateTime CurentTime = DateTime.UtcNow;
                TimeSpan Remaintime = CurentTime - ConstructingTime;
                //float percent = (Remaintime.Seconds / currentData.BuildTime)*100;
                BarrackUI.UpdateConstructTime(100, Remaintime);
                break;
            case BarrackState.Training:
                break;
            case BarrackState.ReadyToCollect:
                break;
            default:
                break;
        }
    }

    public BarrackState GetBarrackState()
    {
        return barrackState;
    }

    public void Confirm()
    {
        Debug.Log("Confirm");
        barrackState = BarrackState.Constructing;
        ConstructingTime =  DateTime.UtcNow.AddSeconds(currentData.BuildTime);
    }

    public void Cancel()
    {
        Debug.Log("Cancel");
        barrackState = BarrackState.Idle;
        percent = 0f;
        trainingTime = 0f;
        //BarrackValue.value = 0f;
    }

    public void Collect()
    {
        if (percent >= 1f)
        {
            Debug.Log("Unit trained and ready to deploy!");
            percent = 0f;
            trainingTime = 0f;
            //BarrackValue.value = 0f;
        }
        else
        {
            Debug.Log("Training in progress. Please wait until it's complete.");
        }
    }


}
