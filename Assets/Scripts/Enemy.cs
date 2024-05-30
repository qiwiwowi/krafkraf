using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;

    [SerializeField] Animator animator;

    Vector2 scale = Vector2.one; //스케일
    bool isFlipedRight = true; //오른쪽으로 반전되었는가?

    [SerializeField] Transform player;
    bool toPlayer = true; //플레이어를 타깃으로 하는지
    bool ToPlayer
    {
        get
        {
            return toPlayer;
        }
        set
        {
            toPlayer = value;
            if (value)
            {
                ToUpStair = false;
                ToDownStair = false;
                Player.instance.SetVignetteScale(true);
            }
        }
    }

    bool toUpStair = false;
    bool ToUpStair
    {
        get
        {
            return toUpStair;
        }
        set
        {
            toUpStair = value;
            
            if (value)
            {
                toPlayer = false;
                targetPos = stairsPos[targetStairs = upStairsPos[enemyFloor]]; //UpStairs를 타깃으로 설정
                OnStairs(null);
                return;
            }
            
            Player.instance.SetVignetteScale(false);
        }
    }
    bool toDownStair = false;
    bool ToDownStair
    {
        get
        {
            return toDownStair;
        }
        set
        {
            toDownStair = value;

            if (value)
            {
                toPlayer = false;
                targetPos = stairsPos[targetStairs = upStairsPos[enemyFloor - 1]]; //DownStairs를 타깃으로 설정
                OnStairs(null);
                return;
            }
            
            Player.instance.SetVignetteScale(false);
        }
    }

    float targetPos;

    float[] stairsPos = new float[3]; //한 층의 계단 가능 문 x 좌표
    [HideInInspector] public int[] upStairsPos; //층들의 UpStairs 좌표 인덱스

    int enemyFloor = 0; //적이 있는 층

    [SerializeField] float changeFloorWait; //층 전환 대기 시간
    WaitForSeconds changeFloorWfs;

    Vector2 move = Vector2.zero; //움직임 벡터
    [SerializeField] float speed; //움직임 속도

    bool isMoving = true;
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
                animator.SetTrigger("isWalk");
                return;
            }
            animator.SetTrigger("isStop");
        }
    }

    private void Awake()
    {
        instance = this;
        scale *= 0.5f;
        targetPos = player.position.x;
        changeFloorWfs = new WaitForSeconds(changeFloorWait);
        IsMoving = true;
    }

    private void Start()
    {
        SetTarget();
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
        if (!isMoving) return;

        if (!toPlayer) move.x = targetPos - transform.position.x;
        else move.x = player.position.x - transform.position.x;

        SpriteFlip();

        transform.Translate(move.normalized * Time.deltaTime * speed);
    }

    /// <summary>
    /// 스프라이트 좌우 반전
    /// </summary>
    void SpriteFlip()
    {
        if (!isFlipedRight && move.x > 0)
        {
            isFlipedRight = true;

            //transform.Translate(Vector3.right * flipOffset);
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (isFlipedRight && move.x < 0)
        {
            isFlipedRight = false;

            //transform.Translate(Vector3.left * fipOffset);
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void SetStairsPos(float[] stairsX, int[] upStairs)
    {
        //stairsPos 설정
        for (int i = 0; i < 3; i++)
        {
            stairsPos[i] = stairsX[i];
        }

        //UpStairPos 설정
        upStairsPos = new int[upStairs.Length];
        for (int i = 0; i < upStairs.Length; i++)
        {
            upStairsPos[i] = upStairs[i];
        }
    }

    int targetStairs = 0;
    public void SetTarget() //타깃 설정
    {
        int playerFloor = GameManager.instance.currentFloor;
        if (playerFloor != enemyFloor)
        {
            //플레이어가 적의 층보다 윗층인 겅우
            if (playerFloor > enemyFloor) ToUpStair = true;
            else ToDownStair = true;
            //플레이어가 적의 층보다 아랫층인 경우
            
            return;
        }
        
        if (Player.instance.isHiding)
        {
            if (enemyFloor == 0) ToUpStair = true;
            else if (enemyFloor == GameManager.instance.floorCnt - 1) ToDownStair = true;
            else
            {
                if (Random.Range(0, 2) == 0) ToUpStair = true;
                else ToDownStair = true;
            }
            return;
        }

        ToPlayer = true;
        //플레이어를 타깃으로 설정
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnStairs(other);
    }

    [SerializeField] float overlapSize;
    [SerializeField] LayerMask lm;
    public void OnStairs(Collider2D other)
    {
        if (other == null)
        {
            //온 트리거 엔터가 실행된 후에 플레이어가 층 이동 시
            //다시 계단을 확인하기 위해 오버랩으로 주변 사물 얻기
            Collider2D overlap = Physics2D.OverlapBox(transform.position, Vector3.one * overlapSize, 0, lm);
            if (overlap == null) return;

            other = overlap;
        }
        else if (!other.CompareTag("InteractionObject")) return;


        background bgType = other.GetComponent<Background>().backgroundType;
        if (bgType == background.UpStairs && toUpStair)
        {
            toUpStair = false;

            StartCoroutine(changeFloor(1));
        }
        else if (bgType == background.DownStairs && toDownStair)
        {
            toDownStair = false;

            StartCoroutine(changeFloor(-1));
        }
    }

    IEnumerator changeFloor(int upDown) //층 전환
    {
        IsMoving = false;

        yield return changeFloorWfs;

        transform.position = targetPos * Vector2.right + Vector2.up * (transform.position.y + GameManager.FLOOR_INTERVAL * upDown);
        enemyFloor += upDown;

        IsMoving = true;

        SetTarget();
    }
}