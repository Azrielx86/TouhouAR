using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Restart : MonoBehaviour
{
    public Button restartButton;
    
    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
    }
    
    private void RestartGame()
    {
        Debug.Log("Restarting game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
