using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject endGameObject; 
    [SerializeField] private KeyCode interactKey = KeyCode.E; 

    private bool isPlayerInRange = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            InteractWithEndGameObject();
        }
    }

    private void InteractWithEndGameObject()
    {

        Debug.Log("Interacted with end game object");


        SceneManager.LoadScene("Game End"); 
    }
}
