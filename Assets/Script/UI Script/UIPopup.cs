using UnityEngine;

public class UIPopup : MonoBehaviour
{


    public void Initilize(string title, string message)
    {
        Debug.Log($"[UIPopup] Initilize with Title: {title}, Message: {message}");
    }

    public void ClosePopup()
    {
        Debug.Log("[UIPopup] ClosePopup called");
        gameObject.SetActive(false);
    }
}
