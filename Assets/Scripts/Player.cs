using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;
    [SerializeField] Animator animator;

    Vector3 scale = Vector3.one; //������

    bool isFlipedRight = true; //���������� �����Ǿ��°�?
    bool isUpStair = false, isDownStair = false; // ����ΰ���
    bool isMoving = false;
    bool IsMoving
    {
        get
        {
            return isMoving;
        }
        set
        {
            isMoving = value;
            if (value)
            {
                animator.SetTrigger("isRun");
                return;
            }
            animator.SetTrigger("isStop");
        }
    }

    float move = 0; //������ �Ǽ���
    float speed; //������ �ӵ�
    [SerializeField] float walkSpeed = 5; //���� �� �ӵ�
    [SerializeField] float runSpeed = 10; //�� �� �ӵ�
    bool isRunning = false;
    bool IsRunning
    {
        get
        {
            return isRunning;
        }
        set
        {
            isRunning = value;
            if (value)
            {
                speed = runSpeed;
                return;
            }
            speed = walkSpeed;
        }
    }

    const float MAX_STAMINA = 100;
    [SerializeField] float staminaDrain = 50; //���׹̳� �Ҹ�
    [SerializeField] float staminaRegen = 50; //���¹̳� ȸ����
    bool isRegenerating = false;
    float currentStamina = 100;
    float CurrentStamina
    {
        get
        {
            return currentStamina;
        }
        set
        {
            currentStamina = value;
            staminaSlider.fillAmount = currentStamina / MAX_STAMINA;
        }
    }
    [SerializeField] Image staminaSlider;

    private void Awake()
    {
        speed = walkSpeed;
        CurrentStamina = MAX_STAMINA;
        scale *= 0.38f;
        transform.localScale = scale;
    }

    private void Update()
    {
        Move();
        Stamina();
    }


    /*
     * ���׹̳��� 0�� �ɰ�� �޸��� Ǯ���� ȸ�� ����
     * ���׹̳� ȸ�� �߿��� �޸��� �Ұ�
     * ���׹̳��� 20�̸��̸� ���׹̳� �� ������ ��������� �ٲ�
     */
    void Stamina() //���׹̳� ����
    {
        if (!isRegenerating &&Input.GetKeyDown(KeyCode.LeftShift)) IsRunning = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift)) IsRunning = false;

        if (currentStamina <= 0)
        {
            IsRunning = false;
            isRegenerating = true;
        }

        if (isRunning) CurrentStamina -= staminaDrain * Time.deltaTime;

        else if (isRegenerating)
        {
            if (currentStamina < MAX_STAMINA) CurrentStamina += staminaDrain * Time.deltaTime;
            else isRegenerating = false;
        }

        if (currentStamina < 20) staminaSlider.color = Color.red;
        else staminaSlider.color = Color.white;
    }


    /// <summary>
    /// ������ 
    /// </summary>
    private void Move()
    {
        SpriteFlip();

        move = Input.GetAxisRaw("Horizontal");

        if (!isMoving && move != 0) IsMoving = true;
        else if (isMoving && move == 0) IsMoving = false;
        else if (move == 0) IsRunning = false;

        if (!GameManager.instance.isAllMove) return;

        if (transform.position.x + move > -7 && transform.position.x + move < 60)
        {
            transform.Translate(move * Vector3.right * Time.deltaTime * speed);
        }

        StairProcess();
    }

    void StairProcess() //��� ��ȣ�ۿ�
    {
        if (Input.GetKeyDown(KeyCode.F) && isUpStair)
        {
            //animator.SetTrigger("isUpStair");
            StartCoroutine(LoadBackGround.instance.DualFade());
            ProcessStair(down: true);
        }

        else if (Input.GetKeyDown(KeyCode.F) && isDownStair)
        {
            //animator.SetTrigger("isDownStair");
            StartCoroutine(LoadBackGround.instance.DualFade(-1));
            ProcessStair(up: true);
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
        if (other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: true);
            else if ((other.GetComponent<Background>().backgroundType == background.DownStairs)) ProcessStair(down: true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) //���� ������Ʈ�� ��������
    {
        if (!GameManager.instance.isAllMove) return;

        if (other.CompareTag("InteractionObject"))
        {
            if (other.GetComponent<Background>().backgroundType == background.UpStairs) ProcessStair(up: false);
            else if ((other.GetComponent<Background>().backgroundType == background.DownStairs)) ProcessStair(down: false);
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
