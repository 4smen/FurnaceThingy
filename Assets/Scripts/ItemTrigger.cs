using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [SerializeField] private int radius = 5;
    [SerializeField] private Item sample;

    public int GetCollidedItems()
    {
        int locatedItems = 0;
        foreach (Collider item in Physics.OverlapSphere(transform.position, radius))
        {
            var obj = item.gameObject.GetComponent<Item>();
            if (obj.itemInfo.id == sample.itemInfo.id) locatedItems++;
        }
        return locatedItems;
    }
}
