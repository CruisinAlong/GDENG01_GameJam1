using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private int maxLives = 5;

    private Vector3 respawnPosition;
    private int currentLives;

    [SerializeField] private TextMeshProUGUI livesText;

    void Start()
    {
        currentLives = maxLives;
        respawnPosition = player.position;
        UpdateLivesUI();
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPosition = newCheckpoint;
    }

    public void PlayerDied()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives > 0)
        {
            RespawnPlayer();
        }
        else
        {
            GameOver();
        }
    }

    private void RespawnPlayer()
    {
        player.position = respawnPosition;
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("Game Over"); 
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "" + currentLives;
        }
        else
        {
            Debug.LogWarning("No livesText assigned to CheckpointManager.");
        }
    }
}
