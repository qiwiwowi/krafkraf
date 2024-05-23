using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    
    Vector2 scale = Vector2.one; //스케일
    bool isFlipedRight = true; //오른쪽으로 반전되었는가?

    [SerializeField] Transform player;
    bool toPlayer = true; //플레이어를 타깃으로 하는지
    Vector2 targetPos;

    float floorInterval = 0;
    Vector2[] stairsPos = { new Vector2(-3.5f, 1.7f), new Vector2(27.1f, 1.7f), new Vector2(57.7f, 1.7f)}; //1층일 때 계단 위치들
    public int[] upStairsPos;

    int enemyFloor = 0; //적이 있는 층

    Vector2 move = Vector2.zero; //움직임 벡터
    [SerializeField] float speed; //움직임 속도

    private void Awake()
    {
        instance = this;
        scale *= 0.5f;
        targetPos = player.position;
    }

    private void Start()
    {
        floorInterval = GameManager.FLOOR_INTERVAL;
        upStairsPos = new int[GameManager.instance.floorCnt];
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
        if (!toPlayer) move.x = targetPos.x - transform.position.x;
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

    int targetStairs = 0;
    public void SetTarget()
    {
        int playerFloor = GameManager.instance.currentFloor;
        if (playerFloor != enemyFloor)
        {
            toPlayer = false;
            if (playerFloor > enemyFloor) targetStairs = upStairsPos[enemyFloor];
            else targetStairs = upStairsPos[enemyFloor - 1];
            targetPos = stairsPos[targetStairs] * Vector2.up * floorInterval * enemyFloor;
        }
        else toPlayer = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }
    //얘도 층 전환 시간 적용 <- 거의 달라붙었을 때 둘 다 계단 타면 적이 더 빨리 층에 도착할 수 있음
}
