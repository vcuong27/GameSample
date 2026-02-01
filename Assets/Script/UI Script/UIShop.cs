using Unity.VisualScripting;
using UnityEngine;

public class UIShop : MonoBehaviour
{

    [SerializeField] private UIShopItem itemPref;

    [SerializeField] private GameObject content;

    public void InitUI()
    {
        BuildingData[] buildingDatas = DataManager.Instance.GetBuildingDatas();
        foreach (BuildingData data in buildingDatas)
        {
            UIShopItem item = Instantiate(itemPref, content.transform);
            item.InitUI(data);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }    
}
