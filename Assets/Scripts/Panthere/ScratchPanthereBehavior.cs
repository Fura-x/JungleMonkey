using UnityEngine;

public class ScratchPanthereBehavior : MonoBehaviour
{
    public float range = 1f;
    [Space]
    [SerializeField] AudioSource attackSound = null;
    [SerializeField] AudioSource deathSound = null;

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = range;
    }

    public void PlayAttackSound()
    {
        if (attackSound != null) attackSound.Play();
    }

    public void PlayDeathSound()
    {
        if (deathSound != null) deathSound.Play();
    }

    public void Grabbed()
    {
        PlayDeathSound();
        GetComponent<Animator>().Play("SPFall");
    }

    public bool CanTriggerPlayer()
    {
        return GetComponents<SphereCollider>()[0].enabled is true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponents<SphereCollider>()[0].enabled is false) return;
    }
}
