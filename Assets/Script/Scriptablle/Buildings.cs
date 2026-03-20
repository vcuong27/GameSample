using System;
using UnityEngine;

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
public class BarrackData : BuildingData
{
    public int Level;
    public float TrainingTime;

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
