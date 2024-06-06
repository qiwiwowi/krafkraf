using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButtonClick : MonoBehaviour
{
    public void GameStart()
    {
        StartCoroutine(LoadBackGround.instance.FadeIn(true,"Story"));
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Title()
    {
        StartCoroutine(LoadBackGround.instance.FadeIn(true, "Game"));
    }
}
