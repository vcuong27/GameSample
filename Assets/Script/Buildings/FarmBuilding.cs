using System;
using UnityEngine;

public class FarmBuilding : IBuilding
{
    private FarmData curentFarmData;

    [SerializeField] private GameObject ObjectUI;

    void Start()
    {
        buildingType = BuildingType.Farm;
    }

    private void OnEnable()
    {
        GameManager.ON_UPDATE_FARM_DATA += UpdateFarmData;
    }

    private void OnDisable()
    {
        GameManager.ON_UPDATE_FARM_DATA -= UpdateFarmData;
    }


    public void Initilize(FarmData data)
    {
        curentFarmData = data;
    }

    private void UpdateFarmData(FarmData data)
    {
        if(curentFarmData.id != data.id)
        {
            return;
        }
        curentFarmData = data;
        Initilize(data);
    }

    public void Collect()
    {
        Debug.Log($"Collected {curentFarmData.CurrentCoin} coins from the farm.");
        curentFarmData.CurrentCoin = 0; 
        curentFarmData.StartTime = DateTime.UtcNow;
        ObjectUI.SetActive(false);
        
        OnlineManager.Instance.CollectFarm(curentFarmData.id);

    }

    private void Update()
    {
        UpdateFarm();
        UpdateUI();
    }

    public void UpdateFarm()
    {
        if (curentFarmData.CurrentCoin < curentFarmData.MaxCoin)
        {
            curentFarmData.CurrentCoin = (int)(curentFarmData.CoinPerSecond * (DateTime.UtcNow - curentFarmData.StartTime).TotalSeconds);
            if (curentFarmData.CurrentCoin > curentFarmData.MaxCoin)
            {
                curentFarmData.CurrentCoin = curentFarmData.MaxCoin; 
            }
        }
    }

    public void UpdateUI()
    {
        if(curentFarmData.CurrentCoin > curentFarmData.MaxCoin * 0.5f)
        {
            ObjectUI.SetActive(true);
        }
        else
        {
            ObjectUI.SetActive(false);
        }
    }

}
