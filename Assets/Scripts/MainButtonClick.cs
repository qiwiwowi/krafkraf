using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButtonClick : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("Game");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
