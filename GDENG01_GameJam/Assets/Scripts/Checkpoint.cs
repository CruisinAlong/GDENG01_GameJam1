using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private CheckpointManager checkpointManager;

    void Start()
    {
        checkpointManager = FindAnyObjectByType<CheckpointManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkpointManager.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint reached!");
        }
    }
}
