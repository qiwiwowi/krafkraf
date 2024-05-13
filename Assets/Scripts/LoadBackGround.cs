using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBackGround : MonoBehaviour
{
    [Tooltip("장면이 전환되는 속도입니다. 최대값은 1 입니다.")]
    [SerializeField] private float strength;

    private SpriteRenderer _spr;

    public static LoadBackGround instance;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    // Start is called before the first frame update
    void Awake()
    {
        _spr = GetComponent<SpriteRenderer>();
        if (instance == null)
        {
            instance = this;
            _spr.enabled = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public IEnumerator FadeIn(bool SceneLoad = false, string sceneName = null)
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

    public IEnumerator FadeOut(bool SceneLoad = false, string sceneName = null)
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

    public IEnumerator DualFade(int a = 1)
    {
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
