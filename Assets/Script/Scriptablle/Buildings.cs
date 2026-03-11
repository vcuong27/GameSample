using System;
using UnityEngine;

public enum BuildingType
{
    None = 0,
    MainTower,
    Barracks,
    Farm,
    Workshop,
    Tree = 100,
    Rock = 101,

}

[System.Serializable]
public class BuildingData
{
    public int id;
    public BuildingType buildingType;
    public string buildingName;
    public int cost;
    public float buildTime;
    public float buildSpeed;
    public Vector2Int size;
    public Vector2Int pivot;
}

[System.Serializable]
public class FarmData : BuildingData
{
    public int Level;
    public int CoinPerSecond;
    public int MaxCoin;
    public int CurrentCoin;
    public DateTime StartTime;
}

[System.Serializable]
public class BuidingDataGame
{
    public BuildingType buildingType;
    public Sprite buildSprite;
    public GameObject buildingPrefab;
}

[CreateAssetMenu(fileName = "Buildings", menuName = "SCR Objects/My Buildings")]

public class Buildings : ScriptableObject
{
    public BuildingData[] buildingDatas;
    public BuidingDataGame[] buidingDataGames;
}
