using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{

    private Buildings buildingsData;
    private List<IBuildingData> listBuildData;

    private Menus menus;
    private List<MenuData> listMenuData;

    private bool isInitialized = false;

    public void InitOnlineData()
    {

    }

    public void Initlize()
    {
        buildingsData = Resources.Load<Buildings>("Buildings");
        listBuildData = new List<IBuildingData>();
        listBuildData.Add(buildingsData.farmData);
        listBuildData.Add(buildingsData.barrackData);

        menus = Resources.Load<Menus>("Menus");
        listMenuData = menus.MenuDatas.ToList();
        isInitialized = true;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public Buildings GetBuildingData()
    {
        return buildingsData;
    }

    public BuidingDataGame GetbuidingDataGames(BuildingType type)
    {
        return buildingsData.buidingDataGames.First(x => x.buildingType == type);
    }

    public IBuildingData GetBuildingDataByID(BuildingType type)
    {
        return buildingsData.farmData;
    }

    public List<IBuildingData> GetBuildingDatas()
    {
        return listBuildData;
    }


    public MenuData GetMenuData(MenuType type)
    {
        return listMenuData.First(x => x.Type == type);
    }



}
