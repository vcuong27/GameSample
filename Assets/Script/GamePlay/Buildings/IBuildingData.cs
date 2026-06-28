using System;


public enum BuildingType
{
    NONE = 0,
    MAINTOWER,
    FARM_GOLD = 10,
    BARRACKS = 50,
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
    // Build data
    public int ID;
    public string Name;
    public BuildingType BuildingType;
    public int Level;
    public int MaxLevel;
    public int BuildTime;
    public int UpgradeTime;
    public BuildingState State;
    public int Price;
    public int[] UpgradePrice;

    //Battle data
    public int MaxHP;
    public int Attack;
    public int DefendHP;
}

