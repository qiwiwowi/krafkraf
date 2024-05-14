using System.Collections;
using TreeEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;\
    [SerializeField] Animator animator;

    Vector3 scale = Vector3.one; //스케일
    Transform _childTrans = null;

    bool isFlipedRight = true; //오른쪽으로 반전되었는가?
    bool isUpStair = false, isDownStair = false; // 계단인가요
    bool isMoving = false;

    float move = 0; //움직임 실수값
   [SerializeField] float speed; //움직임 속도

    private void Awake()
    {
        _childTrans = animator.gameObject.transform;
        scale *= 0.38f;
        _childTrans.localScale = scale;
    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// 움직임 
    /// </summary>
    private void Move()
    {
        if (!GameManager.instance.isAllMove) return;

        move = Input.GetAxisRaw("Horizontal");

        if (!isMoving && move != 0)
        {
            isMoving = true;
            animator.SetTrigger("isRun");
        }
        else if (isMoving && move == 0)
        {
            isMoving = false;
            animator.SetTrigger("isStop");
        }

        SpriteFlip();

        if (_childTrans.position.x + move > -7 && _childTrans.position.x + move < 60)
        {
            _childTrans.Translate(move * Vector3.right * Time.deltaTime * speed);
        }

        //StairProcess();
    }

    //void StairProcess()
    //{
    //    if (Input.GetKeyDown(KeyCode.F) && isUpStair)
    //    {
    //        animator.SetTrigger("isUpStair");
    //        StartCoroutine(LoadBackGround.instance.DualFade());
    //    }

    //    else if (Input.GetKeyDown(KeyCode.F) && isDownStair)
    //    {
    //        animator.SetTrigger("isDownStair");
    //        StartCoroutine(LoadBackGround.instance.DualFade(-1));
    //    }
    //}

    /// <summary>
    /// 스프라이트 좌우 반전
    /// </summary>
    /// 
    void SpriteFlip()
    {
        if (!isFlipedRight && move > 0)
        {
            isFlipedRight = true;

            //_childTrans.Translate(Vector3.right * flipOffset);
            scale.x *= -1;
            _childTrans.localScale = scale;
        }
        else if (isFlipedRight && move < 0)
        {
            isFlipedRight = false;

            //_childTrans.Translate(Vector3.left * fipOffset);
            scale.x *= -1;
            _childTrans.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: true);
            else if ((other.GetComponent<Background>().backgroundType == background.DownStairs))  ProcessStair(down: true);
        } 
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: false);
            else if((other.GetComponent<Background>().backgroundType == background.DownStairs)) ProcessStair(down: false);
        }
    }

    private void ProcessStair(bool up = false, bool down = false)
    {
        isUpStair = up;
        isDownStair = down;
    }

    //IEnumerator ProCessStairCorutine(bool up = false, bool down = false)
    //{ 
    //    WaitForSeconds Coru = new WaitForSeconds(1f);
    //    if (up == true)
    //    {
    //        isDownStair = false;
    //        yield return Coru;
    //        isUpStair = true;
    //    }
    //    else if(down == true)
    //    {
    //        isUpStair = false;
    //        yield return Coru;
    //        isDownStair = true;
    //    }
    //}
}
