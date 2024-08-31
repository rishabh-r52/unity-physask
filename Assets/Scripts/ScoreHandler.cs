using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] private int checkpointCount = 5;
    private int checkpointsCollected = 0;
    private bool finished = false;

    private void FixedUpdate()
    {
        if (!finished)
        {
            Debug.Log("Score: " + checkpointsCollected);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Start"))
        {
            checkpointsCollected++;
            Destroy(other.gameObject);
            Debug.Log("Started!");
        }
        if (other.CompareTag("Checkpoints"))
        {
            checkpointsCollected++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Finish") && checkpointsCollected >= checkpointCount)
        {
            finished = true;
            Debug.Log("Finished!");
            Invoke("LoadNextScene", 0.5f);
        }
    }
    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
