using System;
using System.Collections.Generic;
using System.Text;


public enum BuildingType
{
    NONE = 0,
    MAINTOWER,
    FARM_GOLD = 10,
    STORAGE_GOLD,
    BARRACKS = 50,
    WORKSHOP = 100,
    TREE = 1000,
    ROCK
}

public enum BuildingState
{
    NONE = 0,
    BUILDING,
    IDLE,
    UPGRADING,
    USE
}


[Serializable]
public class IBuildingData
{
    public int ID;
    public BuildingType BuildingType;
    public int Level;
    public int MaxLevel;
    public DateTime BuildFinishTime;
    public DateTime UpgradeFinishTime;
    public BuildingState State;
    public int Price;
    public int[] UpgradePrice;
}
