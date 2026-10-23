using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class Base : MonoBehaviour
{
    [Header("Main Info")]
    public List<GameObject> prefabs = new ();
    public List<GameObject> contraptions = new();
    public List<GameObject> npc = new();
    public SoundType[] sounds;

    [InspectorOrder] public enum CraftingLevels { None, Table, Anvil, }
    [InspectorOrder] public enum ItemTypes { None, Material, Tool, Usable, }
    [InspectorOrder] public enum InteractTypes { None, PickUp, OpenMenu, Door,  }
    [InspectorOrder] public enum AIState { None, Patrol, Chase }

    public static Base Singletone;

    void Awake()
    {
        if (Singletone == null)
        {
            Singletone = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    public AudioClip GetClipFromName(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == name)
            {
                return sound.clips[Random.Range(0, sound.clips.Length)];
            }
        }

        Debug.LogError("No sound presented in the Base script with the same name.");
        return null;
    }
}

[System.Serializable]
public struct SoundType
{
    public string name;
    public AudioClip[] clips;
    //переписать чтоб хранило не список клипов, а AudioResource(эт типо рандомные звуки)
}

public interface IDamageable
{
    void TakeDamage(int amount);
}

public interface IHealable
{
    void TakeHeal(int amount);
}

public static class GizmosExtensions
{
    public static void DrawWireSkinnyCircle(Vector3 position, float radius)
    {
        int segments = 32;
        float angle = 0f;
        Vector3 lastPoint = position + new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius);

        for (int i = 1; i <= segments; i++)
        {
            angle = (i / (float)segments) * Mathf.PI * 2;
            Vector3 nextPoint = position + new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}
