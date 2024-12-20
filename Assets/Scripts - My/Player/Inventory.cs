using System;
using MT;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class Inventory : NetworkBehaviour
{
    public UIManager uiManager;
    public GameObject[] item;
    public int currItemSlot;
    //[SerializeField] private Image[] invenSlots;
    //[SerializeField] private Image[] invenIcons;
    [SerializeField] private Sprite defaultSlot;
    [SerializeField] private Sprite selectedSlot;

    private void Start()
    {
        print(Application.persistentDataPath);
        uiManager = FindObjectOfType<UIManager>();
        currItemSlot = 0;
        if (!IsOwner) return;
        
        
        // GameObject inventoryPanel = GameObject.Find("InventoryPanel");
        // Image[] images = inventoryPanel.GetComponentsInChildren<Image>();
        // for (int i = 0; i <= images.Length; i++)
        // {
        //     if (i % 2 == 0)
        //     {
        //         invenIcons[i / 2] = images[i];
        //         invenIcons[i / 2].gameObject.SetActive(false);
        //     }
        //     else
        //         invenSlots[(i + 1) / 2] = images[i];
        // }

        for (int i = 0; i < uiManager.inventoryIcons.Count; i++)
        {
            uiManager.inventoryIcons[i].gameObject.SetActive(false);
            
            if(currItemSlot == i) uiManager.inventoryIcons[i].sprite = uiManager.selectedSlot;
            else uiManager.inventoryIcons[i].sprite = uiManager.defaultSlot;
        }
    }

    public void AddItem(GameObject obj, int pos)
    {
        currItemSlot = pos;
        Debug.Log("Adding to "+pos);
        if (!item[pos])
        {
            item[pos] = obj;
            if (IsLocalPlayer)
            {
                uiManager.inventoryIcons[currItemSlot].gameObject.SetActive(true);
                uiManager.inventoryIcons[currItemSlot].sprite = obj.GetComponent<InvenItem>().icon;
                uiManager.inventoryBackIcons[currItemSlot].sprite = uiManager.selectedSlot;
            }

            //obj.SetActive(false);
            //obj.GetComponent<NetworkObject>().Despawn();
        }
    }
    public bool RemoveItem()
    {
        Debug.Log("Trying Removing from "+currItemSlot);
        if (!item[currItemSlot])
            return false;
        // Slime
        item[currItemSlot].SetActive(true);
        item[currItemSlot].transform.position = transform.position;
        //Inventory
        item[currItemSlot] = null;
        uiManager.inventoryIcons[currItemSlot].gameObject.SetActive(false);
        Debug.Log("Removed from "+currItemSlot);
        //Destroy(GetComponent<Inventory>().item[itemSlot]);1
        
        return true;
    }
    public void ChangeSlots(int pos)
    {
        currItemSlot = pos;
        for (int i = 0; i < uiManager.inventoryBackIcons.Count; i++)
            uiManager.inventoryBackIcons[i].sprite = defaultSlot;
        uiManager.inventoryBackIcons[currItemSlot].sprite = selectedSlot;
    }
}