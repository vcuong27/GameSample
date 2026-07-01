using UnityEngine;

[System.Serializable]
public class UnitDataGame
{
    public UnitType Type;
    public Sprite Sprite;
    public GameObject Prefab;
    public GameObject BulletPrefab;
}

[CreateAssetMenu(fileName = "UnitsReference", menuName = "GameScriptable/UnitsReference")]
public class UnitsReference : ScriptableObject
{
    public UnitDataGame[] UnitDataGames;
}
