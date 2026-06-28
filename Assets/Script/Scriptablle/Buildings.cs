using UnityEngine;


[System.Serializable]
public class BuidingDataGame
{
    public Vector2Int size;
    public Vector2Int pivot;
    public BuildingType buildingType;
    public Sprite buildSprite;
    public GameObject buildingPrefab;
}

[CreateAssetMenu(fileName = "Buildings", menuName = "SCR Objects/My Buildings")]

public class Buildings : ScriptableObject
{
    public FarmData farmData;
    public BarrackData barrackData;

    public BuidingDataGame[] buidingDataGames;
}
