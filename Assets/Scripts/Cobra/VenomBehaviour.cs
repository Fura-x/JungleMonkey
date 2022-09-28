using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenomBehaviour : MonoBehaviour
{
    [SerializeField] float venomSpeed = 2f;
    Rigidbody rb = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = transform.right * venomSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Move>().Knockback(transform);
            collision.gameObject.GetComponent<Life>().Hurt(1);
        }
        gameObject.SetActive(false);
    }
}
