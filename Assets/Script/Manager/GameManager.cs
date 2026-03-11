using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject Object3D;
    [SerializeField] private GridInput  gridInput;

    public static Action<FarmData> ON_UPDATE_FARM_DATA;


    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BuyBuilding(int buildingId)
    {
        GameController.Instance.CloseShop();

        Debug.Log($"GameManager: Buy Building {buildingId}");
        BuildingData buildingData = DataManager.Instance.GetBuildingDataByID(buildingId);
        if (buildingData != null)
        {
            Vector2Int pos = GridManager.Instance.FindPlaceForBuilding(buildingData.size);
            if (pos.x != -1 && pos.y != -1)
            {
               bool result =  GridManager.Instance.PlaceBuilding(pos, buildingData.size, buildingData.buildingType);
                if (result)
                {
                    GameObject buildingObj = Instantiate(DataManager.Instance.GetbuidingDataGames(buildingData.buildingType).buildingPrefab, Object3D.transform);
                    buildingObj.transform.position = GridManager.Instance.CellToWorldCenter(pos, buildingData.size);
                    Debug.Log("Building placed successfully.");
                }
                else
                {
                    Debug.Log("Failed to place the building.");
                }
            }
            else
            {
                Debug.Log("No space available for the building.");
            }
        }
    }


    public void UpdateFarmData(FarmData farmData)
    {
        PlayerProfile.Instance.UpdateFarmData(farmData);
        ON_UPDATE_FARM_DATA?.Invoke(farmData);
    }
}
