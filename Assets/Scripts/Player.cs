using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;

    //const float flipOffset = 0.3f;
    [SerializeField] Animator animator;

    Vector2 scale = Vector2.one; //스케일

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

    bool isFlipedRight = true; //오른쪽으로 반전되었는가?

    public gameItem[] playerInventory = new gameItem[2]; //플레이어 인벤토리
    [SerializeField] private Image[] itemInvenUI = new Image[2]; //플레이어 인벤토리 사진
    [SerializeField] private Sprite[] itemImage;

    //public int roomCount = 0;
    background backgroundType = background.None; //코드가 복잡해짐에 따라 무지성 bool을 방지하기 위해 여기에다 접촉된 오브젝트의 백그라운드 값들을 넣음.
    Background backgroundClass = null; //코드가 복잡해짐에 따라 무지성 bool을 방지하기 위해 여기에다 접촉된 오브젝트의 백그라운드 클래스들을 넣음.
    public bool isHiding = false; //플레이어가 소화전/보일러실에 숨었나요?
    bool IsHiding
    {
        get
        {
            return isHiding;
        }
        set
        {
            GameManager.instance.isAllMove = !value; //움직임 정지
            isHiding = value;
            playerSprite.enabled = !value;
            playerCollider.enabled = !value;

            if (value) isRegenerating = true;
            Enemy.instance.SetTarget();
            StartCoroutine(LoadBackGround.instance.ButtonCorutine());
        }
    }

    // bool isUpStair = false, isDownStair = false; // 계단인가요
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

    float move = 0; //움직임 실수값
    float speed; //움직임 속도
    [SerializeField] float walkSpeed = 5; //걸을 때 속도
    [SerializeField] float runSpeed = 10; //뛸 때 속도
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
    [SerializeField] float staminaDrain = 50; //스테미나 소모량
    [SerializeField] float staminaRegen = 50; //스태미나 회복량
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
     * 스테미나가 0이 될경우 달리기 풀리고 회복 시작
     * 스테미나 회복 중에는 달리기 불가
     * 스테미나가 20미만이면 스테미나 바 색깔이 노란색으로 바뀜
     */
    void Stamina() //스테미나 관리
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
    /// 움직임 
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

    void StairProcess() //계단 상호작용
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

    void HideProcess() //소화전/보일러실/호실 상호작용
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

    //isEnemySameFlr 적과 같은 층에 있는가
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

    //불빛으로 비네트 효과 살짝 밝아짐
    IEnumerator SetVignetteLight()
    {
        while (vignetteImage.color.a > vignetteColor.a)
        {
            vignetteImage.color = Color.Lerp(vignetteImage.color, vignetteColor, Time.deltaTime * vignetteLightSpeed);
            yield return null;
        }
    }

    //불빛에서 나와서 비네트 효과 더 어두워짐
    IEnumerator SetVignetteDark()
    {
        while (vignetteImage.color.a < vignetteColor.a)
        {
            vignetteImage.color = Color.Lerp(vignetteImage.color, vignetteColor, Time.deltaTime * vignetteLightSpeed);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //게임 오브젝트에 접근하였을때
    {
        if (other.CompareTag("InteractionObject"))
        {
            backgroundClass = other.GetComponent<Background>();
            backgroundType = backgroundClass.backgroundType;
            //계단
            //if (bgType == background.UpStairs) isUpStair = true;
            //else if (bgType == background.DownStairs) isDownStair = true;

            if (backgroundType == background.Lighted || backgroundType == background.NPCDoor)
            {
                if (vignetteLightCorou != null) StopCoroutine(vignetteLightCorou);
                vignetteColor.a = 0.9f;
                vignetteLightCorou = StartCoroutine(SetVignetteLight());
            }
        }
        else if (other.CompareTag("Enemy")) //게임 오버 구현
        {
            if (IsAvilableGameItem(gameItem.Glass, true)) Enemy.instance.PlayerUseGlass();
            else GameManager.instance.GameOver();
        }
    }
    private void OnTriggerExit2D(Collider2D other) //게임 오브젝트를 떠났을때
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

    private bool IsAvilableGameItem(gameItem _gameItem, bool isDelete = false) //인벤토리에 특정 게임 아이템을 찾는 함수
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

    //private void ProcessStair(bool up = false, bool down = false) //첫번째 UpStair 두번째 DownStair. 기본값 false
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
