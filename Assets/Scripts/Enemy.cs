using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;

    [SerializeField] Animator animator;

    Vector2 scale = Vector2.one; //������
    bool isFlipedRight = true; //���������� �����Ǿ��°�?

    [SerializeField] Transform player;
    bool toPlayer = true; //�÷��̾ Ÿ������ �ϴ���
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
                targetPos = stairsPos[targetStairs = upStairsPos[enemyFloor]]; //UpStairs�� Ÿ������ ����
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
                targetPos = stairsPos[targetStairs = upStairsPos[enemyFloor - 1]]; //DownStairs�� Ÿ������ ����
                OnStairs(null);
                return;
            }
            
            Player.instance.SetVignetteScale(false);
        }
    }

    float targetPos;

    float[] stairsPos = new float[3]; //�� ���� ��� ���� �� x ��ǥ
    [HideInInspector] public int[] upStairsPos; //������ UpStairs ��ǥ �ε���

    int enemyFloor = 0; //���� �ִ� ��

    [SerializeField] float changeFloorWait; //�� ��ȯ ��� �ð�
    WaitForSeconds changeFloorWfs;

    Vector2 move = Vector2.zero; //������ ����
    [SerializeField] float speed; //������ �ӵ�

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
    /// ������
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
    /// ��������Ʈ �¿� ����
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
        //stairsPos ����
        for (int i = 0; i < 3; i++)
        {
            stairsPos[i] = stairsX[i];
        }

        //UpStairPos ����
        upStairsPos = new int[upStairs.Length];
        for (int i = 0; i < upStairs.Length; i++)
        {
            upStairsPos[i] = upStairs[i];
        }
    }

    int targetStairs = 0;
    public void SetTarget() //Ÿ�� ����
    {
        int playerFloor = GameManager.instance.currentFloor;
        if (playerFloor != enemyFloor)
        {
            //�÷��̾ ���� ������ ������ �Ͽ�
            if (playerFloor > enemyFloor) ToUpStair = true;
            else ToDownStair = true;
            //�÷��̾ ���� ������ �Ʒ����� ���
            
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
        //�÷��̾ Ÿ������ ����
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
            //�� Ʈ���� ���Ͱ� ����� �Ŀ� �÷��̾ �� �̵� ��
            //�ٽ� ����� Ȯ���ϱ� ���� ���������� �ֺ� �繰 ���
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

    IEnumerator changeFloor(int upDown) //�� ��ȯ
    {
        IsMoving = false;

        yield return changeFloorWfs;

        transform.position = targetPos * Vector2.right + Vector2.up * (transform.position.y + GameManager.FLOOR_INTERVAL * upDown);
        enemyFloor += upDown;

        IsMoving = true;

        SetTarget();
    }
}