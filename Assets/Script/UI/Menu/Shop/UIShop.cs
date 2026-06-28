using System.Collections.Generic;
using UnityEngine;

public class UIShop : MonoBehaviour
{

    [SerializeField] private UIShopItem itemPref;

    [SerializeField] private GameObject content;

    bool isInit = false;

    public void InitUI()
    {
        if (isInit)
        {
            return;
        }

        isInit = true;
        List<IBuildingData> buildingDatas = DataManager.Instance.GetBuildingDatas();
        foreach (IBuildingData data in buildingDatas)
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
