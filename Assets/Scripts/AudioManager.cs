using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Base soundLibrary;
    public static AudioManager Singletone;

    void Awake()
    {
        if (Singletone == null)
        {
            Singletone = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string name, AudioSource source)
    {
        source.PlayOneShot(soundLibrary.GetClipFromName(name));
    }
}
