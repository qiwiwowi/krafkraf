using UnityEngine;

public class Player : MonoBehaviour
{
    //const float flipOffset = 0.3f;

    Vector3 scale = Vector3.one; //������
    bool isFlipedRight = true; //���������� �����Ǿ��°�?

    Vector3 move = Vector3.zero; //������ ����
    [SerializeField] float speed; //������ �ӵ�

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Move()
    {
        move.x = Input.GetAxisRaw("Horizontal");

        SpriteFlip();

        transform.Translate(move * Time.deltaTime * speed);
    }

    /// <summary>
    /// ��������Ʈ �¿� ����
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
