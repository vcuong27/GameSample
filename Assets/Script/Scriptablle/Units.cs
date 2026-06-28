using UnityEngine;

[System.Serializable]
public class UnitDataGame
{
    public UnitType Type;
    public Sprite Sprite;
    public GameObject Prefab;
    public GameObject BulletPrefab;
}

[CreateAssetMenu(fileName = "Units", menuName = "Scriptable Objects/Units")]
public class Units : ScriptableObject
{
    public SwordManData SwordMan;

    public UnitDataGame[] UnitDataGames;

}
