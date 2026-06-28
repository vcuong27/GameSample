using TMPro;
using UnityEngine;

public class LoginPopup : MonoBehaviour
{

    [SerializeField] private TMP_InputField InputFieldUsername;
    [SerializeField] private TMP_InputField InputFieldPassword;

    private LoadingController loadingController;

    public void Initilize(LoadingController controller)
    {
        loadingController = controller;
        InputFieldUsername.text = "Player01";
        InputFieldPassword.text = "123456";
    }

    public string GetUsername()
    {
        return InputFieldUsername.text;
    }

    public string GetPassword()
    {
        return InputFieldPassword.text;
    }

    public void OnClickLogin()
    {
        loadingController.Login();
    }

}
