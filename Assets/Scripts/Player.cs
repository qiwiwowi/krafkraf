using System.Collections;
using TreeEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;\
    [SerializeField] Animator animator;

    Vector3 scale = Vector3.one; //스케일

    bool isFlipedRight = true; //오른쪽으로 반전되었는가?
    bool isUpStair = false, isDownStair = false; // 계단인가요
    bool isMoving = false;

    float moveX = 0, moveY = 0;  //움직임 실수 x, y값
   [SerializeField] float speed; //움직임 속도

    private void Awake()
    {
        scale *= 0.38f;
        transform.localScale = scale;
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
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if(isUpStair)
        {
            if (transform.position.y < 4f) transform.localScale = Mathf.Clamp(transform.localScale.y - (moveY * Time.deltaTime * 0.25f), 0.15f, 0.4f) * Vector3.one;
            else
            {
                StartCoroutine(LoadBackGround.instance.DualFade());
                isUpStair = false;
                return;
            }
             
            transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y + (Time.deltaTime * moveY * speed), -3.1f, 4f));

            if (transform.localScale.y < scale.y)
            {
                moveX = 1;
                animator.SetTrigger("isStop");
                return;
            }
            else transform.localScale = scale;
        }

        if(isDownStair)
        {
            if (transform.localScale.y > 0.3f) transform.localScale = Mathf.Clamp(transform.localScale.y - (moveY * Time.deltaTime * 0.25f), 0.15f, 0.4f) * Vector3.one;
            else
            {
                StartCoroutine(LoadBackGround.instance.DualFade(-1));
                isDownStair = false;
                return;
            }

            transform.position += (-Vector3.right) * Time.deltaTime * moveY * speed;

            if (transform.localScale.y < scale.y)
            {
                return;
            }
            else
            {
                transform.localScale = scale;
            }
        }

        if (!isMoving && moveX != 0)
        {
            isMoving = true;
            animator.SetTrigger("isRun");
        }
        else if (isMoving && moveX == 0)
        {
            isMoving = false;
            animator.SetTrigger("isStop");
        }

        SpriteFlip();

        if (transform.position.x + moveX > -7 && transform.position.x + moveX < 60)
        {
            transform.Translate(moveX * Vector3.right * Time.deltaTime * speed);
        }
    }


    /// <summary>
    /// 스프라이트 좌우 반전
    /// </summary>
    /// 
    void SpriteFlip()
    {
        if (!isFlipedRight && moveX > 0)
        {
            isFlipedRight = true;

            //transform.Translate(Vector3.right * flipOffset);
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (isFlipedRight && moveX < 0)
        {
            isFlipedRight = false;

            //transform.Translate(Vector3.left * fipOffset);
            scale.x *= -1;
            transform.localScale = scale;
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
