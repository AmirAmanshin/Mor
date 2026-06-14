//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class WinLose : MonoBehaviour
//{
//    [Header("UI Panels")]
//    [SerializeField] private GameObject winPanel;
//    [SerializeField] private GameObject losePanel;

//    [SerializeField] private PlayerStats _playerStats;
//    [SerializeField] private PlayerUI _playerUI;

//    private void Start()
//    {
//        if (winPanel != null) winPanel.SetActive(false);
//        if (losePanel != null) losePanel.SetActive(false);

//        Time.timeScale = 1f;
//    }

//    private void Update()
//    {
//        if (_playerStats.health <= 0f && !losePanel.activeSelf)
//        {
//            GameOver();
//        }
//    }

//    private void GameOver()
//    {
//        Time.timeScale = 0f;

//        if (_playerUI.killAmount >= 100)
//        {
//            if (winPanel != null) winPanel.SetActive(true);
//        }
        
//        if (_playerStats.health <= 0f)
//        {
//            if (losePanel != null) losePanel.SetActive(true);
//        }
//    }

//    public void RestartGame()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    public void ExitGame()
//    {
//        Application.Quit();
//    }
//}
