using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGlobal : MonoBehaviour
{
    public Inventory inventory;
    public UniversalRendererData URPData;

    public static PlayerGlobal Singletone;

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
