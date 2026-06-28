using TMPro;
using UnityEngine;

public class MyClanDetailUI : MonoBehaviour
{


    public TextMeshProUGUI clanName;
    public TextMeshProUGUI clanDescription;
    public Sprite clanFlag;
    public GameObject ListMemberRoot;


    public void initialize()
    {
        ClanInfo clanInfo = ClanManager.Instance.GetClanInfo();
        if (clanInfo != null)
        {
            clanName.text = ClanManager.Instance.GetClanName();
            clanDescription.text = clanInfo.message;
        }
    }
}
