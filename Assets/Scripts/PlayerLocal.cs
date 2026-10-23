using UnityEngine;

public class PlayerLocal : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerCam playerCam;
    public BaseMechanics playerBaseMechanics;
    public Drop playerDrop;
    public Animator playerAnimator;

    public bool isConstructing = false;

    public static PlayerLocal Singletone;

    void Awake()
    {
        if (Singletone == null)
        {
            Singletone = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }
}
