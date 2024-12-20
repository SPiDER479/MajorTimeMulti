using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFood : MonoBehaviour
{
    [SerializeField] private Farm farm;

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Food"))
        {
            farm.setFood(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
