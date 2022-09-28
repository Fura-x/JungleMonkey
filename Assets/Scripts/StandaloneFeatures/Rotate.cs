using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 90;
    Vector3 origin;
    private void Start()
    {
        origin = transform.position;
    }
    void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        transform.position = origin + Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad)/2;
    }
}
