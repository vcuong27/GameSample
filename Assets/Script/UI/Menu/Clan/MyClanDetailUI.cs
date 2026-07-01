using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyClanDetailUI : MonoBehaviour
{


    public TextMeshProUGUI clanName;
    public TextMeshProUGUI clanDescription;
    public Image clanFlag;
    public GameObject ListMemberRoot;


    public void initialize()
    {
        ClanInfo clanInfo = ClanManager.Instance.GetClanInfo();
        if (clanInfo != null)
        {
            clanName.text = ClanManager.Instance.GetClanName();
            clanDescription.text = clanInfo.message;
            if(clanInfo.flagName == "default_flag")
                clanFlag.sprite = DataManager.Instance.GetClanFlag(ClanFlagID.NONE);
            else if (clanInfo.flagName == "flag_1")
                clanFlag.sprite = DataManager.Instance.GetClanFlag(ClanFlagID.FLAG_1);
            else if (clanInfo.flagName == "flag_2")
                clanFlag.sprite = DataManager.Instance.GetClanFlag(ClanFlagID.FLAG_2);
            else if (clanInfo.flagName == "flag_3")
                clanFlag.sprite = DataManager.Instance.GetClanFlag(ClanFlagID.FLAG_3);
            else if (clanInfo.flagName == "flag_4")
                clanFlag.sprite = DataManager.Instance.GetClanFlag(ClanFlagID.FLAG_4);
        }
    }
}
