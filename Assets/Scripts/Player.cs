using UnityEngine;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;\
    [SerializeField] Animator animator;

    Vector3 scale = Vector3.one; //스케일
    bool isFlipedRight = true; //오른쪽으로 반전되었는가?

    bool isMoving = false;

    float move = 0; //움직임 실수
    [SerializeField] float speed; //움직임 속도

    private void Awake()
    {
        scale *= 0.38f;
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

        SpriteFlip();

        transform.Translate(move * Vector3.right * Time.deltaTime * speed);
    }

    /// <summary>
    /// 스프라이트 좌우 반전
    /// </summary>
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
}
