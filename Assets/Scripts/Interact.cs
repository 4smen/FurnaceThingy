using System;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Interact : MonoBehaviour
{
    [SerializeField] private GameObject craftingMenu;
    [SerializeField] private float collectDistance;

    [SerializeField] private PlayerLocal playerLocal;

    public bool canClick = true;

    GameObject interactTarget;
    public GameObject InteractTarget { get { return interactTarget; } }

    Animator animator;
    Inventory inventory;

    void Start()
    {
        animator = playerLocal.playerAnimator;
        inventory = PlayerGlobal.Singletone.inventory;
    }

    void Update()
    {
        if (Input.GetKeyDown(Controls.Singletone.interactKey) && canClick)
        {
            StartCoroutine(ClickTimer());

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, 10f))
            {
                print(hit.collider.gameObject.name);
                InteractHandler(hit.collider.gameObject);
            }
        }
    }

    public void InteractHandler(GameObject hit)
    {
        print("аж до взаимодействия дошли");
        interactTarget = hit;

        switch (hit.tag)
        {
            case "Item":
                var newInf = hit.GetComponent<ItemHandler>().item;
                if (inventory.AddItem(newInf))
                {
                    Destroy(hit);
                    print("нашёлся предмет до конца");
                }
                break;
            case "CraftingStation":
                craftingMenu.SetActive(true); //добавить анимацию
                print("крафтинг");
                break;
        }
    }

    IEnumerator ClickTimer()
    {
        canClick = false;
        yield return new WaitForSeconds(0.1f);
        canClick = true;
    }
}
