using System.Collections;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;\
    [SerializeField] Animator animator;

    Vector3 scale = Vector3.one; //스케일

    bool isFlipedRight = true; //오른쪽으로 반전되었는가?
    bool isUpStair = false, isDownStair = false; // 계단인가요
    bool isMoving = false;

    float move = 0; //움직임 실수값
    float speed; //움직임 속도
    [SerializeField] float walkSpeed = 5; //걸을 때 속도
    [SerializeField] float runSpeed = 10; //뛸 때 속도
    bool isRunning = false;

    float maxStamina = 100;
    [SerializeField] float staminaDrain = 0.5f; //스테미나 소모량
    [SerializeField] float stamina = 100;
    [SerializeField] Image staminaSlider;

    private void Awake()
    {
        speed = walkSpeed;
        scale *= 0.38f;
        transform.localScale = scale;
    }

    private void Update()
    {
        Move();
        Stamina();
    }


    /*
     * 스테미나가 20 미만이면 이미 사용중인 상태(isRunning이 true인 경우)에서만 사용할 수 있음.
     * 스테미나가 0이 될경우 달리기 풀림
     * 스테미나가 20미만이면 스테미나 바 색깔이 노란색으로 바뀜
     */
    void Stamina() //스테미나 관리
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 20) isRunning = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift)) isRunning = false;

        if (stamina <= 0) isRunning = false;

        if(isRunning)
        {
            stamina -= staminaDrain;
            speed = runSpeed;

        } else if(stamina < maxStamina)
        {
            stamina += staminaDrain;
            speed = walkSpeed;
        }

        if (stamina < 20) staminaSlider.color = Color.yellow;
        else staminaSlider.color = Color.white;

        staminaSlider.fillAmount = stamina / maxStamina;
    }


    /// <summary>
    /// 움직임 
    /// </summary>
    private void Move()
    {
        SpriteFlip();

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
        else if (move == 0) isRunning = false;


        if (!GameManager.instance.isAllMove) return;

        if (transform.position.x + move > -7 && transform.position.x + move < 60)
        {
            transform.Translate(move * Vector3.right * Time.deltaTime * speed);
        }

        StairProcess();
    }

    void StairProcess() //계단 상호작용
    {
        if (Input.GetKeyDown(KeyCode.F) && isUpStair)
        {
            //animator.SetTrigger("isUpStair");
            StartCoroutine(LoadBackGround.instance.DualFade());
            isUpStair = false;
        }

        else if (Input.GetKeyDown(KeyCode.F) && isDownStair)
        {
            //animator.SetTrigger("isDownStair");
            StartCoroutine(LoadBackGround.instance.DualFade(-1));
            isDownStair = false;
        }
    }

    /// <summary>
    /// 스프라이트 좌우 반전
    /// </summary>
    /// 
    void SpriteFlip()
    {
        if (!isFlipedRight && move > 0)
        {
            isFlipedRight = true;

            //transform.Translate(Vector3.right * flipOffset);
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (isFlipedRight && move < 0)
        {
            isFlipedRight = false;

            //transform.Translate(Vector3.left * fipOffset);
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //게임 오브젝트에 접근하였을때
    {
        if(other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: true);
            else if ((other.GetComponent<Background>().backgroundType == background.DownStairs))  ProcessStair(down: true);
        } 
    }
    private void OnTriggerExit2D(Collider2D other) //게임 오브젝트를 떠났을때
    {
        if (other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: false);
            else if((other.GetComponent<Background>().backgroundType == background.DownStairs)) ProcessStair(down: false);
        }
    }

    private void ProcessStair(bool up = false, bool down = false) //첫번째 UpStair 두번째 DownStair. 기본값 false
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
