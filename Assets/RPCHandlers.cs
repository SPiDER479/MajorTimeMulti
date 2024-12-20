using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading;

public class RPCHandlers : NetworkBehaviour
{
    public Inventory inventory;
    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddSlimeToInventoryServerRpc(NetworkObjectReference slime, int slot)
    {
        if (!IsServer) return;
        
        if (slime.TryGet(out NetworkObject networkObject))
        {
            if (inventory != null)
            {
                // Add the slime to the player's inventory
                DeactivateSlimeClientRpc(slime,slot);
                //networkObject.GetComponent<GameObject>().SetActive(false);
                // Despawn the slime after adding it to the inventory
                //networkObject.Despawn();

                // Optionally, destroy the slime object (if needed for further cleanup)
                //Destroy(networkObject);
            }
        }
    }
    [ClientRpc]
    private void DeactivateSlimeClientRpc(NetworkObjectReference slime, int slot)
    {
        inventory.AddItem(slime, slot);
        if (slime.TryGet(out NetworkObject networkObject))
        {
            // Deactivate the slime for the client
            networkObject.gameObject.SetActive(false);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void PullSlimeServerRpc(NetworkObjectReference slime, Vector3 force)
    {
        if (!IsServer) return;
        PullSlimeClientRpc(slime, force);
    }
    
    [ClientRpc]
    private void PullSlimeClientRpc(NetworkObjectReference slime, Vector3 force)
    {
        if (slime.TryGet(out NetworkObject networkObject))
        {
            // You can now use the networkObject (e.g., spawn it, manipulate it)
            //networkObject.Spawn();
            networkObject.GetComponent<Pull>().pull(force);
            //print(1);
        }
    }
    
    [ServerRpc(RequireOwnership = false)] public void SpawnObjectServerRpc(string shooterID, string dropObjID)
    {
        // Only the server will handle spawning the object
        if (IsServer) // Ensuring this is only run on the server
        {
            // var instance = Instantiate(GetComponent<Inventory>().item[itemSlot], spawnPosition, spawnRotation);
            // instance.SetActive(true);
            // instance.GetComponent<Rigidbody>().AddForce(10 * transform.forward,ForceMode.Impulse);
            // // Get the NetworkObject from the instantiated prefab
            // var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            //
            // // Spawn the object across the network (this works for all clients)
            // instanceNetworkObject.Spawn();
            
            SpawnObjectClientRpc(shooterID, dropObjID);
        }
    }
    
    [ClientRpc] public void SpawnObjectClientRpc(string shooterID, string dropObjID)
    {
        // Only the server will handle spawning the object
        //var instance = Instantiate(GetComponent<Inventory>().item[itemSlot], spawnPosition, spawnRotation);
        GetComponent<Inventory>().RemoveItem();
        // List<NetworkObject> netObjs = (FindObjectsOfType(typeof(NetworkObject)) as NetworkObject[]).ToList();
        // int index = 0;
        // for (int i = 0; i < netObjs.Count; i++)
        // {
        //     if(netObjs[i].NetworkObjectId.ToString() == dropObjID) index = i;
        // }
        // netObjs[index].gameObject.SetActive(true);
        Debug.Log("Slime pircked "+ dropObjID);
        //netObjs[index].GetComponent<Rigidbody>().AddForce(10 * transform.forward,ForceMode.Impulse);
        // Get the NetworkObject from the instantiated prefab
        //var instanceNetworkObject = netObjs[index].GetComponent<NetworkObject>();

        // Spawn the object across the network (this works for all clients)
            
        //instanceNetworkObject.Spawn();
        //instanceNetworkObject.ChangeOwnership(NetworkObjectId);
    }
    
}
