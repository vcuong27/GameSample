using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject Object3D;
    [SerializeField] private GridInput gridInput;

    public static Action<FarmData> ON_UPDATE_FARM_DATA;

    public void BuyBuilding(BuildingType type)
    {
        if (PlayerProfile.Instance.GetNumberPlayerItemData(ItemType.COIN) < DataManager.Instance.GetBuildingDataByID(type).Price)
        {
            Debug.Log("Not enough coins to buy the building.");
            return;
        }

        GameController.Instance.CloseShop();

        Debug.Log($"GameManager: Buy Building {type}");
        BuidingDataGame buildingData = DataManager.Instance.GetbuidingDataGames(type);
        if (buildingData != null)
        {
            Vector2Int pos = GridManager.Instance.FindPlaceForBuilding(buildingData.size);
            if (pos.x != -1 && pos.y != -1)
            {
                bool result = GridManager.Instance.PlaceBuilding(pos, buildingData.size, buildingData.buildingType);
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
        //PlayerProfile.Instance.UpdateFarmData(farmData);
        ON_UPDATE_FARM_DATA?.Invoke(farmData);
    }
}
