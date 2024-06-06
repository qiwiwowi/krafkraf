using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadBackGround : MonoBehaviour
{
    [Tooltip("장면이 전환되는 속도입니다. 최대값은 10입니다.")]
    [Range(0, 10)] [SerializeField] private float strength;

    [SerializeField] SpriteRenderer _spr;
    [SerializeField] Image _keyOutLine;

    public bool isAnim; //키보드 애니메이션이 활성화 되었는가?

    public static LoadBackGround instance;

    private void Start()
    {
        isAnim = false;
        StartCoroutine(FadeOut());
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            _spr.enabled = true;
        }
    }  

    public IEnumerator FadeIn(bool SceneLoad = false, string sceneName = null) //화면 페이드인
    {
        Color _color = _spr.color;

        for (float i = 0; i < 1; i += Time.deltaTime * strength)
        {
            _color.a = i;
            _spr.color = _color;

            yield return  null;
        }

        if (SceneLoad && sceneName != null) SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeOut(bool SceneLoad = false, string sceneName = null) //화면 페이드아웃
    {
        Color _color = _spr.color;

        for (float i = 1; i > 0; i -= Time.deltaTime * strength)
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

        StartCoroutine(ButtonCorutine());

        for (float i = 0; i < 1; i += Time.deltaTime * strength)
        {
            _color.a = i;
            _spr.color = _color;

            yield return null;
        }

        GameManager.instance.ChangeCurrentFloor(a);

        for (float i = 1; i > 0; i -= Time.deltaTime * strength)
        {
            _color.a = i;
            _spr.color = _color;

            yield return null;
        }
        GameManager.instance.isAllMove = true;
    }

    public IEnumerator ButtonCorutine() //키보드 채워지는 애니메이션
    {
        _keyOutLine = GameObject.FindWithTag("KeyOutLine").GetComponent<Image>();

        for (float i = 0; i < 1; i += Time.deltaTime * strength)
        {
            _keyOutLine.fillAmount = i;

            yield return null;
        }
        _keyOutLine.fillAmount = 0;
    }
}
