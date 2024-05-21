using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Vector3 scale = Vector3.one; //스케일
    bool isFlipedRight = true; //오른쪽으로 반전되었는가?

    [SerializeField] Transform player;
    Vector2 move = Vector2.zero; //움직임 벡터
    [SerializeField] float speed; //움직임 속도

    private void Awake()
    {
        scale *= 0.5f;
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
        move.x = player.position.x - transform.position.x;

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
}
