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

[CreateAssetMenu(fileName = "BuildingsReference", menuName = "GameScriptable/BuildingsReference")]

public class BuildingsReference : ScriptableObject
{
    public BuidingDataGame[] buidingDataGames;
}
