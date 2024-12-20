using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
public class SlimeSpawner : NetworkBehaviour
{
    
    [SerializeField] private GameObject[] slimes;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform container;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject slime in slimes)
            {
                int randomIndex = Random.Range(10, 15);
                for (int i = 0; i < randomIndex; i++)
                {
                    //Instantiate(slime, spawnPoints[Random.Range(0,spawnPoints.Length)].position + 
                                       //new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)), Random.rotation, container);
                    
                    RequestSpawnObject(slime);
                }
            }
            GetComponent<BoxCollider>().size *= 5;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            GetComponent<BoxCollider>().size /= 5;
        }
    }
    
    [ServerRpc(RequireOwnership = false)] public void SpawnObjectServerRpc(NetworkObjectReference networkObjectReference)
    {
        // Only the server will handle spawning the object
        if (IsServer) // Ensuring this is only run on the server
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                // You can now use the networkObject (e.g., spawn it, manipulate it)
                //networkObject.Spawn();
                print(1);
            }
            
            
            // var instance = Instantiate(obj, spawnPoints[Random.Range(0, spawnPoints.Length)].position + 
            //                                 new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)), 
            //     Random.rotation, container);
            //
            // // Get the NetworkObject from the instantiated prefab
            // var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            //
            // // Spawn the object across the network (this works for all clients)
            // instanceNetworkObject.Spawn();
        }
    }
    // Client-side method to request spawning the object
    public void RequestSpawnObject(GameObject obj)
    {
        if (IsClient) // Ensure the request is coming from a client
        {
            SpawnObjectServerRpc(obj); // Call the server RPC
        }
    }
}
