using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogScript : MonoBehaviour
{
    Rigidbody rb = null;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Fall()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
    }
}
