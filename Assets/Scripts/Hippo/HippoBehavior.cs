using UnityEngine;

public class HippoBehavior : MonoBehaviour
{
    Rigidbody rb = null;
    [SerializeField] Orientation orientation = Orientation.LEFT;

    [SerializeField] float maxMoveTimer = 2f;
    float moveTimer = 0f;

    [SerializeField] float speed = 5;
    bool isMoving = true;

    [Space]
    [SerializeField] AudioSource knockoutSound = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (orientation is Orientation.RIGHT) ResetTimer();

        moveTimer = maxMoveTimer / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) return;

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f) ResetTimer();

        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        rb.velocity += -transform.right * speed;
    }

    void ResetTimer()
    {
        if (orientation is Orientation.LEFT)
        {
            transform.Rotate(Vector3.up, 180f);
            transform.Translate(Vector3.right);
            orientation = Orientation.RIGHT;
        }
        else
        {
            transform.Rotate(Vector3.up, -180f);
            transform.Translate(Vector3.right);
            orientation = Orientation.LEFT;
        }
        moveTimer = maxMoveTimer;
    }

    public void SetMoving(bool isMoving)
    {
        this.isMoving = isMoving;
    }

    public void PlayKOSound()
    {
        if (knockoutSound != null) knockoutSound.Play();
    }
}
