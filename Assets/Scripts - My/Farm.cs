using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    public GameObject food;
    [SerializeField] private float timer;
    [SerializeField] private Transform[] spawnPoints;
    
    public void setFood(GameObject food)
    {
        Destroy(this.food);
        this.food = food;
        StopCoroutine(SpawnFood());
        StartCoroutine(SpawnFood());
    }

    IEnumerator SpawnFood()
    {
        yield return new WaitForSeconds(timer);
        foreach(Transform spawnPoint in spawnPoints)
            Instantiate(food, spawnPoint.position, Quaternion.identity).SetActive(true);
        Destroy(food);
    }
}
