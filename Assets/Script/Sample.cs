using Lean.Gui;
using System;
using UnityEngine;

public class Sample : MonoBehaviour
{

    [SerializeField]
    private LeanJoystick leanJoystick;

    void Start()
    {
        leanJoystick.OnDown.AddListener(JoyStickSet);
        leanJoystick.OnSet.AddListener(JoyStickSet);
        leanJoystick.OnUp.AddListener(JoyStickUp);

    }

    bool enabler = false;
    private void JoyStickSet()
    {
        enabler = true;
    }
    private void JoyStickUp()
    {
        enabler = false;
    }
    public void JoyStickSet(Vector2 vector)
    {
        if(enabler)
        Debug.Log($"[Sample] JoyStickSet: {vector}");

    }
}
