using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;

    //const float flipOffset = 0.3f;
    [SerializeField] Animator animator;

    Vector2 scale = Vector2.one; //������

    [SerializeField] Image vignetteImage;
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] Collider2D playerCollider;
    //[SerializeField] private GameObject interactionKeyImage;

    Color vignetteColor;
    [SerializeField] float vignetteLightSpeed;
    Coroutine vignetteLightCorou;

    Vector2 vignetteBigScale;
    Vector2 vignetteSmallScale;
    const float VIGNETTE_SCALE_RATE = 1.36f;
    [SerializeField] float vignetteScaleSpeed;
    Coroutine vignetteScaleCorou;

    bool isFlipedRight = true; //���������� �����Ǿ��°�?

    public gameItem[] playerInventory = new gameItem[2]; //�÷��̾� �κ��丮
    [SerializeField] private Image[] itemInvenUI = new Image[2]; //�÷��̾� �κ��丮 ����
    [SerializeField] private Sprite[] itemImage;

    //public int roomCount = 0;
    background backgroundType = background.None; //�ڵ尡 ���������� ���� ������ bool�� �����ϱ� ���� ���⿡�� ���˵� ������Ʈ�� ��׶��� ������ ����.
    Background backgroundClass = null; //�ڵ尡 ���������� ���� ������ bool�� �����ϱ� ���� ���⿡�� ���˵� ������Ʈ�� ��׶��� Ŭ�������� ����.
    public bool isHiding = false; //�÷��̾ ��ȭ��/���Ϸ��ǿ� ��������?
    bool IsHiding
    {
        get
        {
            return isHiding;
        }
        set
        {
            GameManager.instance.isAllMove = !value; //������ ����
            isHiding = value;
            playerSprite.enabled = !value;
            playerCollider.enabled = !value;

            if (value) isRegenerating = true;
            Enemy.instance.SetTarget();
            StartCoroutine(LoadBackGround.instance.ButtonCorutine());
        }
    }

    // bool isUpStair = false, isDownStair = false; // ����ΰ���
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
        instance = this;
        isHiding = false;
        speed = walkSpeed;
        CurrentStamina = MAX_STAMINA;
        vignetteColor = Color.black;
        vignetteSmallScale = vignetteImage.transform.localScale;
        vignetteBigScale = vignetteSmallScale * VIGNETTE_SCALE_RATE;
        vignetteImage.transform.localScale = vignetteBigScale;
        scale *= 0.38f;
        transform.localScale = scale;
    }

    private void Update()
    {
        Move();
        HideProcess();
        Stamina();

        PlayerItemSet(0, gameItem.None);
        PlayerItemSet(1, gameItem.None);
    }

    /*
     * ���׹̳��� 0�� �ɰ�� �޸��� Ǯ���� ȸ�� ����
     * ���׹̳� ȸ�� �߿��� �޸��� �Ұ�
     * ���׹̳��� 20�̸��̸� ���׹̳� �� ������ ��������� �ٲ�
     */
    void Stamina() //���׹̳� ����
    {
        if (currentStamina <= 0)
        {
            IsRunning = false;
            isRegenerating = true;
        }

        if (!isRegenerating && !isHiding)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) IsRunning = true;
            else if (Input.GetKeyUp(KeyCode.LeftShift)) IsRunning = false;

            if (isRunning) CurrentStamina -= staminaDrain * Time.deltaTime;
        }

        else if (isRegenerating)
        {
            if (currentStamina < MAX_STAMINA) CurrentStamina += staminaRegen * Time.deltaTime;
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
        if (!GameManager.instance.isAllMove) return;

        move = Input.GetAxisRaw("Horizontal");

        if (!isMoving && move != 0) IsMoving = true;
        else if (isMoving && move == 0) IsMoving = false;
        else if (move == 0) IsRunning = false;


        if (transform.position.x + move > -11 && transform.position.x + move < 64)
        {
            transform.Translate(move * Vector2.right * Time.deltaTime * speed);
        }

        SpriteFlip();
        StairProcess();
        MlikProcess();
    }

    void StairProcess() //��� ��ȣ�ۿ�
    {
        if (Input.GetKeyDown(KeyCode.F) && backgroundType == background.UpStairs)
        {
            if (GameManager.instance.currentFloor == GameManager.instance.floorCnt - 1) return;
            //animator.SetTrigger("isUpStair");
            StartCoroutine(LoadBackGround.instance.DualFade());

            backgroundType = background.DownStairs;
        }

        else if (Input.GetKeyDown(KeyCode.F) && backgroundType == background.DownStairs)
        {
            //animator.SetTrigger("isDownStair");
            StartCoroutine(LoadBackGround.instance.DualFade(-1));

            backgroundType = background.UpStairs;
        }
    }

    void HideProcess() //��ȭ��/���Ϸ���/ȣ�� ��ȣ�ۿ�
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (backgroundType == background.FireWall || backgroundType == background.BoilerRoom) IsHiding = !isHiding;
            /*else if (backgroundType == background.Lighted || backgroundType == background.NPCDoor)
            {
                IsHiding = !isHiding;
                GameManager.instance.InRoom(backgroundClass.roomCount);
            }*/
        }
    }

    void MlikProcess()
    {
        if (Input.GetKeyDown(KeyCode.F) && (backgroundType == background.Milk) && !IsInventoryFull())
        {

            GameManager.instance.floors[GameManager.instance.currentFloor].SetBackground(background.Unlighted, backgroundClass.roomCount);

            if (playerInventory[0] != gameItem.None) PlayerItemSet(0, gameItem.Mlik);
            else PlayerItemSet(1, gameItem.Mlik);
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

    //isEnemySameFlr ���� ���� ���� �ִ°�
    public void SetVignetteScale(bool isEnemySameFlr)
    {
        if (isEnemySameFlr)
        {
            if (vignetteScaleCorou != null) StopCoroutine(vignetteScaleCorou);
            vignetteScaleCorou = StartCoroutine(SetVignetteSmall());
            return;
        }
        if (vignetteScaleCorou != null) StopCoroutine(vignetteScaleCorou);
        vignetteScaleCorou = StartCoroutine(SetVignetteBig());
    }

    IEnumerator SetVignetteBig()
    {
        while (vignetteImage.transform.localScale.x < vignetteBigScale.x)
        {
            vignetteImage.transform.localScale = Vector2.Lerp(vignetteImage.transform.localScale, vignetteBigScale, Time.deltaTime * vignetteScaleSpeed);
            yield return null;
        }
    }

    IEnumerator SetVignetteSmall()
    {
        while (vignetteImage.transform.localScale.x > vignetteSmallScale.x)
        {
            vignetteImage.transform.localScale = Vector2.Lerp(vignetteImage.transform.localScale, vignetteSmallScale, Time.deltaTime * vignetteScaleSpeed);
            yield return null;
        }
    }

    //�Һ����� ���Ʈ ȿ�� ��¦ �����
    IEnumerator SetVignetteLight()
    {
        while (vignetteImage.color.a > vignetteColor.a)
        {
            vignetteImage.color = Color.Lerp(vignetteImage.color, vignetteColor, Time.deltaTime * vignetteLightSpeed);
            yield return null;
        }
    }

    //�Һ����� ���ͼ� ���Ʈ ȿ�� �� ��ο���
    IEnumerator SetVignetteDark()
    {
        while (vignetteImage.color.a < vignetteColor.a)
        {
            vignetteImage.color = Color.Lerp(vignetteImage.color, vignetteColor, Time.deltaTime * vignetteLightSpeed);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //���� ������Ʈ�� �����Ͽ�����
    {
        if (other.CompareTag("InteractionObject"))
        {
            backgroundClass = other.GetComponent<Background>();
            backgroundType = backgroundClass.backgroundType;
            //���
            //if (bgType == background.UpStairs) isUpStair = true;
            //else if (bgType == background.DownStairs) isDownStair = true;

            if (backgroundType == background.Lighted || backgroundType == background.NPCDoor)
            {
                if (vignetteLightCorou != null) StopCoroutine(vignetteLightCorou);
                vignetteColor.a = 0.9f;
                vignetteLightCorou = StartCoroutine(SetVignetteLight());
            }
        }
        else if (other.CompareTag("Enemy")) //���� ���� ����
        {
            if (IsAvilableGameItem(gameItem.Glass, true)) Enemy.instance.PlayerUseGlass();
            else GameManager.instance.GameOver();
        }
    }
    private void OnTriggerExit2D(Collider2D other) //���� ������Ʈ�� ��������
    {
        if (!GameManager.instance.isAllMove) return;

        if (other.CompareTag("InteractionObject"))
        {
            background _backgroundType = other.GetComponent<Background>().backgroundType;

            if (_backgroundType != backgroundType) return;
            else backgroundType = background.None;

            backgroundClass = null;

            if (_backgroundType == background.Lighted || _backgroundType == background.NPCDoor)
            {
                if (vignetteLightCorou != null) StopCoroutine(vignetteLightCorou);

                vignetteColor.a = 1;
                vignetteLightCorou = StartCoroutine(SetVignetteDark());
            }
        }
    }

    private bool IsAvilableGameItem(gameItem _gameItem, bool isDelete = false) //�κ��丮�� Ư�� ���� �������� ã�� �Լ�
    {
        bool isAvilable = false;

        for (int i = 0; i < playerInventory.Length; i++)
        {
            if (playerInventory[i] == _gameItem)
            {
                isAvilable = true;
                if (isDelete) playerInventory[i] = gameItem.None;
            }
        }

        return isAvilable;
    }

    private bool IsInventoryFull()
    {
        int isFullNum = 0;

        for (int i = 0; i < playerInventory.Length; i++)
        {
            if (playerInventory[i] != gameItem.None) isFullNum++;
        }

        return (isFullNum == 2);
    }

    public void PlayerItemSet(int inventoryNumber, gameItem itemType)
    {
        playerInventory[inventoryNumber] = itemType;
        itemInvenUI[inventoryNumber].enabled = (itemType != gameItem.None);
        itemInvenUI[inventoryNumber].sprite = itemImage[(int)itemType];
    }

    //private void ProcessStair(bool up = false, bool down = false) //ù��° UpStair �ι�° DownStair. �⺻�� false
    //{
    //    isUpStair = up;
    //    isDownStair = down;
    //}

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
