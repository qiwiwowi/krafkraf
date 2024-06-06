using UnityEngine;

public class Floor : MonoBehaviour
{
    public Background[] backgroundObjs;

    [SerializeField] SpriteRenderer[] backgroundSr;

    private int count = 0, floor = 0;

    public void SetBackgrounds(background[] backgrounds, int _floor) //��������Ʈ ����
    {
        for (int i = 0; i < 7; i++)
        {
            floor = _floor;

            backgroundObjs[i].backgroundType = backgrounds[i];
            backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds[i]];

            SetIgnoreInteraction(i);
        }
    }

    private void SetIgnoreInteraction(int i) //�� ȣ�� ���ϱ� (�迭 �ε���)
    {
        backgroundObjs[i].roomCount = 1; //��ȣ�ۿ� ���� 

        if (backgroundObjs[i].backgroundType == background.Lighted) //���� Ȱ��ȭ �Ǿ��ִٸ� �� ��ȣ ���ϱ�
        {
            count++;
            backgroundObjs[i].roomCount = (floor + 1) * 100 + count;
        }

        switch (backgroundObjs[i].backgroundType) //��ȣ�ۿ� ���� ����
        {
            case background.Milk:
            case background.Unlighted:
                backgroundObjs[i].roomCount = 0;
                break;
            default:
                break;
        }
    }

    public void SetBackground(background backgrounds, int i) //�ϳ��� ��׶��常 ���� (��׶���, �迭 �ε���)
    {
        backgroundObjs[i].backgroundType = backgrounds;
        backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds];

        backgroundObjs[i].roomCount = (floor + 1) * 100 + count;

        SetIgnoreInteraction(i);
    }
}