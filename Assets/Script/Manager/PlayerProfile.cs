using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class PlayerBuildingData
{
    public int ID;
    public BuildingType BuildingType;
    public int Level;
    public BuildingState State;
    public Vector2 Position;
    public DateTime EndTime; // thoi gian ket thuc xay dung, nang cap, thu thap = utc time
}

[Serializable]
public class PlayerUnitData
{
    public int ID;
    public UnitType Type;
    public int Level;
    public int Number;
}

[Serializable]
public class PlayerItemData
{
    public int ID;
    public ItemType Type;
    public long Number;
}

[Serializable]
public class PlayerStatData
{
    public int Level;
    public int Experience;
    public int WinNumber;
    public int LoseNumber;
}

[Serializable]
public class PlayerSettingData
{
    public bool IsMusicOn;
    public bool IsSoundOn;
    public bool IsNotificationOn;
}

[Serializable]
public class PlayerProfileData
{
    public int playerID;
    public string playerName;
    public int profileVersion;
    public List<PlayerBuildingData> buildingDatas;
    public List<PlayerUnitData> unitDatas;
    public List<PlayerItemData> itemDatas;
    public PlayerStatData statData;
    public PlayerSettingData settingData;
}


public class PlayerProfile : MonoBehaviour
{

    public static Action OnProfileUpdated;


    private static PlayerProfile _instance;
    public static PlayerProfile Instance => _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurentProfile = new PlayerProfileData
        {
            playerID = 1,
            playerName = "Player1",
            buildingDatas = new List<PlayerBuildingData>(),
            itemDatas = new List<PlayerItemData>(),
            unitDatas = new List<PlayerUnitData>(),
            statData = new PlayerStatData
            {
                Level = 1,
                Experience = 0,
                WinNumber = 0,
                LoseNumber = 0,
            },
            settingData = new PlayerSettingData(),
        };

        Initialize(null);

    }


    private bool IsInitialized = false;
    private PlayerProfileData CurentProfile;

    public void RequestPlayerProfile()
    {
        OnlineManager.Instance.GetPlayerProfile();
    }

    public void Initialize(PlayerProfileData profileData)
    {

        string playerProfile = PlayerPrefs.GetString("PlayerProfile", "");
        if (playerProfile.Length > 0)
        {
            CurentProfile = JsonUtility.FromJson<PlayerProfileData>(playerProfile);
        }

        //if (profileData != null)
        //{
        //    CurentProfile = profileData;
        //}
        OnProfileUpdated.Invoke();
        IsInitialized = true;
    }

    public void SavePlayerProfile()
    {
        PlayerPrefs.SetString("PlayerProfile", JsonUtility.ToJson(CurentProfile));
    }

    public bool IsInitialize()
    {
        return IsInitialized;
    }

    public long GetPlayeID()
    {
        return CurentProfile.playerID;
    }

    public string GetPlayerName()
    {
        return CurentProfile.playerName;
    }

    public PlayerStatData GetPlayerStatData()
    {
        return CurentProfile.statData;
    }

    public void IncreasePlayerLevel(int number)
    {
        CurentProfile.statData.Level += number;
    }
    public void IncreasePlayerExperience(int number)
    {
        CurentProfile.statData.Experience += number;
    }

    public PlayerSettingData GetPlayerSettingData()
    {
        return CurentProfile.settingData;
    }

    public void UpdatePlayerSettingData(PlayerSettingData settingData)
    {
        CurentProfile.settingData = settingData;
    }

    public List<PlayerUnitData> GetPlayerUnitDatas()
    {
        return CurentProfile.unitDatas;
    }

    public void AddPlayerUnitData(PlayerUnitData unitData)
    {
        CurentProfile.unitDatas.Add(unitData);
    }

    public void UpdatePlayerUnitData(PlayerUnitData unitData)
    {
        foreach (PlayerUnitData data in CurentProfile.unitDatas)
        {
            if (data.ID == unitData.ID)
            {
                data.Level = unitData.Level;
                data.Number = unitData.Number;
                break;
            }
        }
    }

    public void RemovePlayerUnitData(int id)
    {
        PlayerUnitData unitData = CurentProfile.unitDatas.Find(unit => unit.ID == id);
        if (unitData != null)
        {
            CurentProfile.unitDatas.Remove(unitData);
        }
    }

    public List<PlayerBuildingData> GetPlayerBuildingDatas()
    {
        return CurentProfile.buildingDatas;
    }

    public void AddPlayerBuildingData(PlayerBuildingData buildingData)
    {
        CurentProfile.buildingDatas.Add(buildingData);
    }

    public void UpdatePlayerBuildingData(PlayerBuildingData buildingData)
    {
        foreach (PlayerBuildingData data in CurentProfile.buildingDatas)
        {
            if (data.ID == buildingData.ID)
            {
                data.Level = buildingData.Level;
                data.State = buildingData.State;
                data.Position = buildingData.Position;
                data.EndTime = buildingData.EndTime;
                break;
            }
        }
    }

    public List<PlayerItemData> GetPlayerItemData()
    {
        return CurentProfile.itemDatas;
    }

    public void AddPlayerItemData(PlayerItemData itemData)
    {
        CurentProfile.itemDatas.Add(itemData);
    }

    public void RemovePlayerItemData(PlayerItemData itemData)
    {
        PlayerItemData data = CurentProfile.itemDatas.Find(item => item.ID == itemData.ID);
        if (data != null)
        {
            CurentProfile.itemDatas.Remove(data);
        }
    }

    public long GetNumberPlayerItemData(ItemType itemType)
    {
        PlayerItemData itemData = CurentProfile.itemDatas.Find(item => item.Type == itemType);
        if (itemData != null) return itemData.Number;
        return 0;
    }

    public void ChangeNumberPlayerItemData(ItemType itemType, long number)
    {
        PlayerItemData itemData = CurentProfile.itemDatas.Find(item => item.Type == itemType);
        if (itemData != null)
        {
            itemData.Number += number;
            if (itemData.Number < 0) itemData.Number = 0;
        }
        else
        {
            CurentProfile.itemDatas.Add(new PlayerItemData
            {
                ID = CurentProfile.itemDatas.Count + 1,
                Type = itemType,
                Number = number > 0 ? number : 0,
            });
        }
    }

    
}
