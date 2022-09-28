using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(Sway))]
[RequireComponent (typeof(Grab))]
[RequireComponent (typeof(Jump))]
[RequireComponent (typeof(Crouch))]
[RequireComponent (typeof(Life))]
[RequireComponent (typeof(PlayerSound))]
public class Move : MonoBehaviour
{
    // MOVEMENT
    [Header("Air control")]
    [SerializeField] float gravityMultiplyer = 0f;
    [SerializeField] float slideGravity = 800f;
    [SerializeField] float maxSpeed = 35;
    [SerializeField] float airControl = 2f;
    [SerializeField] float maxVelocity = 15f;
    [SerializeField] float swayForce = 2f;
    float gravity = 0;
    [Header("Speed")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float slowMultiplier = 2f;
    [Header("KnockBack")]
    [SerializeField] float knockbackForce = 30f;
    [SerializeField] float knockbackDuration = 1f;
    [Header("Depth control")]
    public float defaultDepth = 5f; // Depth that travel the player when pass through a depth door

    public float VerticalSitckTrigger = 0f; // True if Left joystick inclined up
    public bool isInDepth = false; // Signal to others objects if player hides in depth
    bool hadChangeDepth = false; // Change depth during a depth

    [Header("Other")]
    public float lookAtBottomOffset = 0.5f;
    [Range(0f, 1f)] public float walkSoundDeadZone = 0.1f;

    float currentSpeed = 0f;
    float currentOrientation = 0f;

    Vector3 respawnPoint;

    float moveFactor;

    // OTHERS BEHAVIORS
    Tail tail = null;
    Jump jump = null;
    Crouch crouch = null;
    Grab grab = null;
    Sway sway = null;
    Life life = null;
    PlayerSound sound = null;
    Animator animator = null;

    // PHYSICS && CONTROLS

    Rigidbody rb;
    public PlayerControl controls;

    bool isRunning = false;
    bool canMove = true;

    //CHECKPOINTS AND SPAWN

    GameMaster checkpointMaster = null;
    IEnumerator CoKnockback(Transform source, float kbDuration = 0f, float kbForce = 0f)
    {
        rb.velocity = Vector3.zero;
        canMove = tail.enabled = false;
        Vector3 direction = new Vector3(Mathf.Sign(transform.position.x - source.position.x) * 2, 1, 0);

        rb.velocity = direction * (kbForce == 0f ? knockbackForce : kbForce);

        yield return new WaitForSeconds(kbDuration <= 0f ? knockbackDuration : kbDuration);
        canMove = tail.enabled = true;
    }

    IEnumerator CoKnockback(Vector3 direction, float kbDuration = 0f, float kbForce = 0f)
    {
        rb.velocity = Vector3.zero;
        canMove = tail.enabled = false;

        rb.velocity = direction * (kbForce == 0f ? knockbackForce : kbForce); ;

        yield return new WaitForSeconds(kbDuration <= 0f ? knockbackDuration : kbDuration);
        canMove = tail.enabled = true;
    }

    public bool IsGrounded() { return jump.canMove; }

    private void Awake()
    {
        tail = GetComponent<Tail>();
        sway = GetComponent<Sway>();
        grab = GetComponent<Grab>();
        crouch = GetComponent<Crouch>();
        jump = GetComponent<Jump>();
        life = GetComponent<Life>();
        sound = GetComponent<PlayerSound>();
        animator = GetComponent<Animator>();

        gravity = gravityMultiplyer;

        currentSpeed = moveSpeed;
        respawnPoint = transform.position;

        controls = new PlayerControl();

        // MOVE
        controls.Player.Move.performed += ctx => moveFactor = ctx.ReadValue<float>();
        controls.Player.Move.performed += ctx => {
            if (ctx.ReadValue<float>() <= walkSoundDeadZone) 
                EndWalk();
                };
        controls.Player.Move.canceled += ctx => moveFactor = 0f;
        controls.Player.Move.canceled += ctx => EndWalk();
        // JUMP
        controls.Player.Jump.performed += ctx => { if (gravity == gravityMultiplyer) jump.JumpPerform(moveFactor); };
        controls.Player.Jump.performed += ctx => sway.Disable();
        controls.Player.Jump.performed += ctx => grab.Disable();
        controls.Player.Jump.performed += ctx => crouch.Disable();

        // UP and DOWN DIRECTION
        controls.Player.CrouchAndRetract.performed += ctx => sway.ChangeSize(-ctx.ReadValue<float>());
        controls.Player.CrouchAndRetract.performed += ctx => VerticalSitckTrigger = ctx.ReadValue<float>();
        controls.Player.CrouchAndRetract.canceled += ctx =>
        {
            VerticalSitckTrigger = 0f;
            hadChangeDepth = false;
        };

        //controls.Player.CrouchAndRetract.performed += ctx => MoveCamera(ctx.ReadValue<float>());
        //controls.Player.CrouchAndRetract.canceled += ctx => FindObjectOfType<FollowingCamera>().ResetOffset();

        controls.Player.CrouchAndRetract.performed += ctx => CrouchPerform(ctx.ReadValue<float>());
        controls.Player.CrouchAndRetract.canceled += ctx => crouch.EnableSwitch();

        // OTHER
        //controls.Player.Respawn.performed += ctx => Respawn();

        controls.Player.QuitApplication.performed += ctx => Application.Quit();
        controls.Player.Run.performed += ctx => isRunning = true;
        controls.Player.Run.canceled += ctx => isRunning = false;

        // LIFE EVENTS
        life.OnHurt.AddListener(() => life.InvulFrames());
        life.OnDeath.AddListener(() => controls.Disable());
        life.OnDeath.AddListener(() => Time.timeScale = 0f);
        life.OnDeath.AddListener(() => FindObjectOfType<CanvasManager>().SwitchCanvas("GameOver"));
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        checkpointMaster = FindObjectOfType<GameMaster>();
        if (checkpointMaster != null) transform.position = checkpointMaster.lastCheckpointPos;
    }

    private void FixedUpdate()
    {
        bool grabState = sway.isEnabled || grab.isEnabled;
        jump.SetGeneralGrabState(grabState);
        crouch.SetGeneralGrabState(grabState);
        crouch.SetAirState(!jump.canMove);

        UpdateSpeed();
        if (!sway.isEnabled && tail.CanRotate()) UpdateOrientation();

        if(canMove && gravity == gravityMultiplyer)
        {
            if (grab.isEnabled)
            {
                grab.SelfUpdate();
                EndWalk();
            }
            else if (sway.isEnabled)
            {
                rb.velocity += Vector3.right * Mathf.Sign(moveFactor) * Time.deltaTime * swayForce;
                EndWalk();
            }
            else if (jump.canMove)
            {
                if (Mathf.Abs(moveFactor) >= walkSoundDeadZone) BeginWalk();
                rb.velocity = new Vector3(currentSpeed * moveFactor, rb.velocity.y, 0);
            }
            else if (Mathf.Abs(rb.velocity.x) < maxVelocity || Mathf.Sign(rb.velocity.x) != Mathf.Sign(moveFactor))
            {
                rb.velocity += Vector3.right * airControl * moveFactor * Time.deltaTime;
            }
            else
            {
                EndWalk();
            }
        }

        if (!sway.isEnabled && !grab.isEnabled)
            rb.AddForce(-Vector3.up * 20f * gravity * Time.deltaTime, ForceMode.Acceleration);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        
    }
    private void UpdateSpeed()
    {
        if (crouch.isEnabled) currentSpeed = Mathf.Lerp(currentSpeed, crouch.speed, crouch.slideLerp);
        else if (isRunning) currentSpeed = runSpeed;
        else currentSpeed = moveSpeed;
    }

    private void UpdateOrientation(bool forceToRotate = false)
    {
        if (currentOrientation == 180f && (forceToRotate || moveFactor > 0))
        {
            transform.Rotate(Vector3.up, -180f);
            currentOrientation = 0f;
            tail.SetOrientation(1);
        }
        else if (currentOrientation == 0f && (forceToRotate || moveFactor < 0))
        {
            transform.Rotate(Vector3.up, 180f);
            currentOrientation = 180f;
            tail.SetOrientation(-1);
        }
    }

    private void BeginWalk()
    {
        sound.PlayOnce("Footsteps");
        animator.SetBool("IsWalking", true);
    }
    private void EndWalk()
    {
        sound.Stop("Footsteps");
        animator.SetBool("IsWalking", false);
    }

    public void CrouchPerform(float yInput)
    {
        if (!isInDepth && !hadChangeDepth && yInput <= -0.9f)
        {
            grab.Disable();
            crouch.SwitchEnableState();
        }
    }

    public void Respawn()
    {
        isInDepth = false;
        transform.position = respawnPoint;
    }

    public void Respawn(Vector3 respawnPos, bool isInDepth = false)
    {
        crouch.Disable();
        sway.Disable();
        this.isInDepth = isInDepth;
        transform.position = respawnPos;
    }

    public void ChangeDepth()
    {
        hadChangeDepth = true;
        isInDepth = isInDepth ? false : true;

        crouch.Disable();
    }

    public bool CanChangeDepth() { return !hadChangeDepth && !sway.isEnabled; }

    public void SlowMove()
    {
        moveSpeed /= slowMultiplier;
        runSpeed /= slowMultiplier;

        jump.SlowJump();
    }

    public void UnslowMove()
    {
        moveSpeed *= slowMultiplier;
        runSpeed *= slowMultiplier;

        jump.UnslowJump();
    }

    private void MoveCamera(float stickValue)
    {
        if (crouch.isEnabled && stickValue <= -0.5f)
        {
            FindObjectOfType<FollowingCamera>().AddToOffset(new Vector3(0f, -lookAtBottomOffset, 0f));
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Giraffe"))
        {
            transform.SetParent(other.GetComponent<StuckPoint>().GetStuckTransform(), true);
        }
        else if (other.CompareTag("BigBird"))
        {
            // If bird is motionless, make it moving when touch
            if (transform.position.y > other.transform.position.y)
            {
                transform.SetParent(other.GetComponent<StuckPoint>().GetStuckTransform(), true);
                other.GetComponent<BigBirdBehavior>().StartMove();
            }
        }

        if (!other.CompareTag("Floor"))
        {
            if (grab.isEnabled) grab.Disable();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Bramble"))
        {
            Knockback(other.transform);
            life.Hurt(1);
        }
        else if(gravity != slideGravity && other.CompareTag("Slope"))
        {
            gravity = slideGravity;
        }
        else if(gravity != gravityMultiplyer && !other.CompareTag("Slope"))
        {
            gravity = gravityMultiplyer;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Giraffe") || other.CompareTag("BigBird"))
        {
            transform.SetParent(null, true);
        }
        else if(other.CompareTag("Slope"))
        {
            gravity = gravityMultiplyer;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy_Panthere_Scratch") && other.GetComponent<ScratchPanthereBehavior>().CanTriggerPlayer())
        {
            GetComponentInParent<Move>().Knockback(other.transform, knockbackDuration * 3f, knockbackForce * 0.7f);
            GetComponentInParent<Life>().Hurt(1);

            other.GetComponent<Animator>().Play("SPAttack");
            other.GetComponent<ScratchPanthereBehavior>().PlayAttackSound();
        }
    }

    public void Knockback(Transform source, float kbDuration = 0f, float kbForce = 0f)
    {
        if (!life.isInvincible)
        {
            sound.Play("Hurt");
            StartCoroutine(CoKnockback(source, kbDuration, kbForce));
        }
    }
    public void Knockback(Vector3 direction, float kbDuration = 0f, float kbForce = 0f)
    {
        sound.Play("Hurt");
        if (!life.isInvincible)
            StartCoroutine(CoKnockback(direction, kbDuration, kbForce));
    }

    public void Stun()
    {
        controls.Player.Look.Disable();
        controls.Player.Jump.Disable();
        rb.isKinematic = true;
    }

    public void StunDisable()
    {
        controls.Player.Jump.Enable();
        controls.Player.Look.Enable();
        rb.isKinematic = false;
    }

    public void BigBirdRotate(Transform birdTransform)
    {
        if (transform.parent != null && transform.parent.Equals(birdTransform))
        {
            birdTransform.Rotate(Vector3.up, 180f);
            transform.Rotate(Vector3.up, 180f);
        }
    }
}
