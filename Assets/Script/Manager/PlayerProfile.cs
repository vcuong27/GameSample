using Unity.VisualScripting;
using UnityEngine;
using System;


[Serializable]
class PlayerProfileData
{
    public int playerID;
    public string playerName;
    public int level;
    public int experience;
    public int coins;
}


public class PlayerProfile : MonoBehaviour
{

    private static PlayerProfile _instance;
    public static PlayerProfile Instance => _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private bool IsInitialized = false;
    private PlayerProfileData CurentProfile;

    public void Initialize()
    {
        CurentProfile = new PlayerProfileData
        {
            playerID = 1,
            playerName = "Player1",
            level = 1,
            experience = 0,
            coins = 1000
        };


        IsInitialized = true;
    }

    public bool IsInitialize()
    {
        return IsInitialized;
    }


    public string GetPlayerName()
    {
        return CurentProfile.playerName;
    }

    public int GetLevel()
    {
        return CurentProfile.level;
    }

    public int GetCoins()
    {
        return CurentProfile.coins;
    }   


}
