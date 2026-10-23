using EzySlice;
using System.Collections.Generic;
using UnityEngine;

public class SliceTest : MonoBehaviour
{
    [SerializeField] Material crossMat;
    [SerializeField] PhysicsMaterial slippery;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<SlicedRemaining>(out var _) == true || other.gameObject.layer == LayerMask.NameToLayer("Walkable")) return;

        SlicedHull hull = SliceObject(other.gameObject, crossMat);

        if (hull == null) return;

        GameObject lowerHalf = hull.CreateLowerHull(other.gameObject, crossMat);
        GameObject upperHalf = hull.CreateUpperHull(other.gameObject, crossMat);

        ProcessHull(lowerHalf);
        ProcessHull(upperHalf);

        other.gameObject.SetActive(false);
    }

    void ProcessHull(GameObject obj)
    {
        obj.AddComponent<Rigidbody>();
        obj.AddComponent<MeshCollider>().convex = true;
        obj.GetComponent<MeshCollider>().material = slippery;
        //obj.AddComponent<SlicedRemaining>().Decay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            rb.isKinematic = true;
            gameObject.transform.position = new Vector3(-8.294f, 1.69f, -15.32f);
            rb.isKinematic = false;
            rb.AddForce(new Vector3(0, 0, 0.001f));
            //Destroy(gameObject, 5f);
        }
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        return obj.Slice(transform.position, transform.up, crossSectionMaterial);
    }
}
