using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using static DevelopersHub.RealtimeNetworking.Client.Packet;

public class DataManager : Singleton<DataManager>
{
    private ClanDataReference clanDataReference;
    private BuildingsReference buildingsDataReference;
    private UnitsReference unitsDataReference;
    private List<IBuildingData> listBuildData;
    private List<IUnitData> listUnitData;

    private Menus menus;
    private List<MenuData> listMenuData;

    private bool isInitialized = false;
    private bool isOnlineDataInitialized = false;

    public void InitOnlineData()
    {

    }

    public void Initlize()
    {
        clanDataReference = Resources.Load<ClanDataReference>("ClanDataRef");
        buildingsDataReference = Resources.Load<BuildingsReference>("BuildingsRef");
        unitsDataReference = Resources.Load<UnitsReference>("UnitsRef");

        menus = Resources.Load<Menus>("Menus");
        listMenuData = menus.MenuDatas.ToList();
        isInitialized = true;
    }

    public void InitializeOnlineData()
    {
        listBuildData = new List<IBuildingData>();
        listBuildData.Add(new BarrackData()
        {
            ID = 1,
            Name = "Barrack",
            BuildingType = BuildingType.BARRACKS,
            Level = 1,
            MaxLevel = 10,
            BuildTime = 100,
            UpgradeTime = 100,
            State = BuildingState.IDLE,
            Price = 100,
        }); 
        listBuildData.Add(new FarmData()
        {
            ID = 1,
            Name = "Farm",
            BuildingType = BuildingType.FARM_GOLD,
            Level = 1,
            MaxLevel = 10,
            BuildTime = 100,
            UpgradeTime = 100,
            State = BuildingState.IDLE,
            Price = 100,
        });

        listUnitData = new List<IUnitData>();
        listUnitData.Add(new SwordManData()
        {
            ID = 1,
            Type = UnitType.SWORDMAN,
            State = UnitState.IDLE,
            Level = 1,
            MaxLevel = 10,
            Attack = 10,
            Defense = 5,
            HP = 100,
            Speed = 5

        });

        isOnlineDataInitialized = true;

    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public bool IsOnlineInitialized()
    {
        return isOnlineDataInitialized;
    }

    public BuildingsReference GetBuildingData()
    {
        return buildingsDataReference;
    }

    public BuidingDataGame GetbuidingDataGames(BuildingType type)
    {
        return buildingsDataReference.buidingDataGames.First(x => x.buildingType == type);
    }

    public IBuildingData GetBuildingDataByID(BuildingType type)
    {
        foreach (var item in listBuildData)
        {
            if(item.BuildingType == type)
                return item;
        }
        return null;
    }

    public List<IBuildingData> GetBuildingDatas()
    {
        return listBuildData;
    }

    public MenuData GetMenuData(MenuType type)
    {
        return listMenuData.First(x => x.Type == type);
    }

    public Sprite GetClanFlag(ClanFlagID flagID)
    {
        return clanDataReference.clanFlagDatas[(int)flagID].FlagSprite;
    }

    public UnitDataGame GetUnitsData(UnitType type)
    {
        return unitsDataReference.UnitDataGames.First(x => x.Type == type);
    }
     
    public IUnitData GetUnitDataByID(UnitType type)
    {
        foreach (var item in listUnitData)
        {
            if (item.Type == type)
                return item;
        }
        return null;
    }

}
