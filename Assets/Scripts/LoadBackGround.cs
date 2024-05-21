using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBackGround : MonoBehaviour
{
    [Tooltip("����� ��ȯ�Ǵ� �ӵ��Դϴ�. �ִ밪�� 1 �Դϴ�.")]
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

    public IEnumerator FadeIn(bool SceneLoad = false, string sceneName = null) //ȭ�� ���̵���
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

    public IEnumerator FadeOut(bool SceneLoad = false, string sceneName = null) //ȭ�� ���̵�ƿ�
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

    public IEnumerator DualFade(int a = 1) //��� ��ȯ (-1�̸� ������, 1�̸� �ö�)
    {
        GameManager.instance.isAllMove = false; //ĳ���� ���� ����

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
