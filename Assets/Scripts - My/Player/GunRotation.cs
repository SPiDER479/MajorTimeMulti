using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotation : MonoBehaviour
{
    [SerializeField]private GameObject cam;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z);
    }
}
