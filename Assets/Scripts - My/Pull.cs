using System;
using UnityEngine;
public class Pull : MonoBehaviour
{
    [SerializeField] private float pullSpeed, upDraft;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void pull(Vector3 direction)
    {
        rb.AddForce (pullSpeed * Time.deltaTime * (direction + upDraft * Vector3.up), ForceMode.Impulse);
    }
}