using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBackGround : MonoBehaviour
{
    [Tooltip("장면이 전환되는 속도입니다. 최대값은 1 입니다.")]
    [SerializeField] private float strength;

    [SerializeField] SpriteRenderer _spr;

    public static LoadBackGround instance;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        _spr.enabled = true;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeIn(bool SceneLoad = false, string sceneName = null) //화면 페이드인
    {
        Color _color = _spr.color;

        for (float i = 0; i < 1; i += strength)
        {
            _color.a = i;
            _spr.color = _color;

            yield return null;
        }

        if (SceneLoad && sceneName != null) SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeOut(bool SceneLoad = false, string sceneName = null) //화면 페이드아웃
    {
        Color _color = _spr.color;

        for (float i = 1; i > 0; i -= strength)
        {
            _color.a = i;
            _spr.color = _color;

            yield return null;
        }

        if (SceneLoad && sceneName != null) SceneManager.LoadScene(sceneName);
    }

    public IEnumerator DualFade(int a = 1) //계단 전환 (-1이면 내려감, 1이면 올라감)
    {
        GameManager.instance.isAllMove = false; //캐릭터 무빙 정지

        Color _color = _spr.color;

        for (float i = 0; i < 1; i += strength)
        {
            _color.a = i;
            _spr.color = _color;

            yield return null;
        }

        GameManager.instance.ChangeCurrentFloor(a);

        for (float i = 1; i > 0; i -= strength)
        {
            _color.a = i;
            _spr.color = _color;

            yield return null;
        }
    }
}
