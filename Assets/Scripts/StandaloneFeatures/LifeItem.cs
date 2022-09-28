using UnityEngine;

public class LifeItem : MonoBehaviour
{
    public int heal = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Life.Heal() return true if the heal is done, so then we destroy the item
            if (other.GetComponentInParent<Life>().Heal(heal))
            {
                other.GetComponentInParent<PlayerSound>().Play("Eat");
                Destroy(gameObject);
            }
        }
    }
}
