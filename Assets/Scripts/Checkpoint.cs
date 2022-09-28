using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameMaster checkpointMaster;
    private void Start()
    {
        checkpointMaster = GameObject.FindGameObjectWithTag("CheckpointMaster").GetComponent<GameMaster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            checkpointMaster.lastCheckpointPos = transform.position;
        }
    }
}
