using UnityEngine;

public class Canvas3D : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCam != null)
        {
            transform.forward = mainCam.transform.forward;
        }
    }
}
