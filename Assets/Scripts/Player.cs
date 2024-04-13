using UnityEngine;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;

    Vector3 scale = Vector3.one; //스케일
    bool isFlipedRight = true; //오른쪽으로 반전되었는가?

    Vector3 move = Vector3.zero; //움직임 벡터
    [SerializeField] float speed; //움직임 속도

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// 움직임
    /// </summary>
    private void Move()
    {
        move.x = Input.GetAxisRaw("Horizontal");

        SpriteFlip();

        transform.Translate(move * Time.deltaTime * speed);
    }

    /// <summary>
    /// 스프라이트 좌우 반전
    /// </summary>
    void SpriteFlip()
    {
        if (!isFlipedRight && move.x > 0)
        {
            isFlipedRight = true;

            //transform.Translate(Vector3.right * );
            scale.x = 1;
            transform.localScale = scale;
        }
        else if (isFlipedRight && move.x < 0)
        {
            isFlipedRight = false;

            //transform.Translate(Vector3.left * 0.3f);
            scale.x = -1;
            transform.localScale = scale;
        }
    }
}
