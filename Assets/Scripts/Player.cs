using UnityEngine;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;

    Vector3 scale = Vector3.one; //������
    bool isFlipedRight = true; //���������� �����Ǿ��°�?

    float move = 0; //������ ����
    [SerializeField] float speed; //������ �ӵ�

    private void Awake()
    {
        scale *= 0.38f;
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
        move = Input.GetAxisRaw("Horizontal");

        SpriteFlip();

        transform.Translate(move * Vector3.right * Time.deltaTime * speed);
    }

    /// <summary>
    /// ��������Ʈ �¿� ����
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
