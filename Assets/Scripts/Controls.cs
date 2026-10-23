using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public KeyCode jumpKey;
    public KeyCode crouchKey;
    public KeyCode sprintKey;
    public KeyCode pauseKey;
    public KeyCode cancelKey;
    public KeyCode confirmKey;
    public KeyCode interactKey;
    public KeyCode inventoryKey;
    public KeyCode skipDialogueKey;
    public KeyCode constructionKey;
    public KeyCode[] useKeys;
    public static Controls Singletone;

    void Awake()
    {
        if (Singletone == null)
            Singletone = this;
        else return;
    }
}
