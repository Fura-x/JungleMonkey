using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    Rigidbody rb = null;

    public bool isEnabled = false;
    bool isSwitched = false;
    bool isGrab = false, isInAir = false;

    bool stopCrouchForce = false;

    [SerializeField] float crouchFallForce = 300f;
    //[SerializeField] float bodyScale = 0.5f;
    public float speed = 3f;
    [Range(0f, 0.1f)] public float slideLerp = 0.1f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetGeneralGrabState(bool isGrabOrSway)
    {
        isGrab = isGrabOrSway;
    }

    public void SetAirState(bool isInAir)
    {
        this.isInAir = isInAir;
    }

    public void SwitchEnableState()
    {
        if (isSwitched) return;

        isSwitched = true;
        if (isEnabled) Disable();
        else Enable();
    }
    public void EnableSwitch()
    {
        isSwitched = false;
    }

    public void Enable()
    {
        // Enable crouch during grab is impossible
        if (isGrab) return;
        else if (isInAir)
        {
            // Accelerate fall movement
            rb.velocity = new Vector3(rb.velocity.x, 0f, 0f);
            rb.AddForce(Vector3.down * crouchFallForce);
            return;
        }

        isEnabled = true;
        Animator animator = GetComponent<Animator>();
        animator.SetBool("IsCrounching", true);
        animator.SetBool("IsWalking", false);

        CapsuleCollider capsule = GetComponentInChildren<CapsuleCollider>();
        capsule.height = 1.5f;
        capsule.center = new Vector3(0f, -0.25f, 0f);
    }

    public void Disable()
    {
        if (!isEnabled) return;

        Animator animator = GetComponent<Animator>();
        animator.SetBool("IsCrounching", false);

        CapsuleCollider capsule = GetComponentInChildren<CapsuleCollider>();
        capsule.height = 2f;
        capsule.center = new Vector3(0f, 0f, 0f);

        isEnabled = false;
    }

    public void CancelCrouchFallforce()
    {
        if (stopCrouchForce) rb.AddForce(Vector3.up * crouchFallForce);
    }
}
