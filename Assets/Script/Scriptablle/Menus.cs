using System;
using UnityEngine;

public enum MenuType
{
    MainMenu = 10,
    InGameMenu,
    PauseMenu,
    GameOverMenu,
    Training = 100,
    Shop,
}

[Serializable]
public class MenuData
{
    public MenuType Type;
    public GameObject MenuPrefab;
}

[CreateAssetMenu(fileName = "Menus", menuName = "GameScriptable/Menus")]
public class Menus : ScriptableObject
{
    public MenuData[] MenuDatas;
}
