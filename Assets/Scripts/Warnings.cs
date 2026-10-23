using System;
using System.Collections.Generic;
using UnityEngine;

public class Warnings : MonoBehaviour
{
    public WarningType[] warnings;

    public static Warnings Singletone;

    void Awake()
    {
        if (Singletone == null)
        {
            Singletone = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RaiseWarning(string code)
    {
        foreach (var warning in warnings)
        {
            if (warning.code == code)
            {
                warning.animator.SetTrigger($"activate_{code}");
            }
        }
        Debug.LogError("No warning presented in the Warnings script with the same code.");
    }
}

[System.Serializable]
public struct WarningType
{
    public string code;
    public Animator animator;
}
