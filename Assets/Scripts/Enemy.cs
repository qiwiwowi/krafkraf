using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    
    Vector2 scale = Vector2.one; //������
    bool isFlipedRight = true; //���������� �����Ǿ��°�?

    [SerializeField] Transform player;
    bool toPlayer = true; //�÷��̾ Ÿ������ �ϴ���
    Vector2 targetPos;

    float floorInterval = 0;
    Vector2[] stairsPos = { new Vector2(-3.5f, 1.7f), new Vector2(27.1f, 1.7f), new Vector2(57.7f, 1.7f)}; //1���� �� ��� ��ġ��
    public int[] upStairsPos;

    int enemyFloor = 0; //���� �ִ� ��

    Vector2 move = Vector2.zero; //������ ����
    [SerializeField] float speed; //������ �ӵ�

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
    /// ������
    /// </summary>
    private void Move()
    {
        if (!toPlayer) move.x = targetPos.x - transform.position.x;
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
    //�굵 �� ��ȯ �ð� ���� <- ���� �޶�پ��� �� �� �� ��� Ÿ�� ���� �� ���� ���� ������ �� ����
}
