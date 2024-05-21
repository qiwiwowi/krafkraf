using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;\
    [SerializeField] Animator animator;

    Vector3 scale = Vector3.one; //������

    bool isFlipedRight = true; //���������� �����Ǿ��°�?
    bool isUpStair = false, isDownStair = false; // ����ΰ���
    bool isMoving = false;

    float move = 0; //������ �Ǽ���
    float speed; //������ �ӵ�
    [SerializeField] float walkSpeed = 5; //���� �� �ӵ�
    [SerializeField] float runSpeed = 10; //�� �� �ӵ�
    bool isRunning = false;

    float maxStamina = 100;
    [SerializeField] float staminaDrain = 0.5f; //���׹̳� �Ҹ�
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
    }

    /// <summary>
    /// ������ 
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


        if (!GameManager.instance.isAllMove) return;

        if (transform.position.x + move > -7 && transform.position.x + move < 60)
        {
            transform.Translate(move * Vector3.right * Time.deltaTime * speed);
        }

        StairProcess();
    }

    void StairProcess()
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
    /// ��������Ʈ �¿� ����
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

    private void OnTriggerEnter2D(Collider2D other) //���� ������Ʈ�� �����Ͽ�����
    {
        if(other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: true);
            else if ((other.GetComponent<Background>().backgroundType == background.DownStairs))  ProcessStair(down: true);
        } 
    }
    private void OnTriggerExit2D(Collider2D other) //���� ������Ʈ�� ��������
    {
        if (other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: false);
            else if((other.GetComponent<Background>().backgroundType == background.DownStairs)) ProcessStair(down: false);
        }
    }

    private void ProcessStair(bool up = false, bool down = false) //ù��° UpStair �ι�° DownStair. �⺻�� false
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
