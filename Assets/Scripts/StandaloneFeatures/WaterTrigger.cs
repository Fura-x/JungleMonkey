using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField] Transform respawnTransform = null;
    [SerializeField] bool useCheckPoint = true;
    GameMaster checkpointMaster;

    private void Start()
    {
        checkpointMaster = GameObject.FindGameObjectWithTag("CheckpointMaster").GetComponent<GameMaster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Life>().Hurt(1);
            if (useCheckPoint)
            {
                other.GetComponentInParent<Move>().Respawn(checkpointMaster.lastCheckpointPos);
            }
            else
            {
                other.GetComponentInParent<Move>().Respawn(respawnTransform.position);
            }
        }
    }
}
