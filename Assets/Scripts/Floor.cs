using UnityEngine;

public class Floor : MonoBehaviour
{
    public Background[] backgroundObjs;

    [SerializeField] SpriteRenderer[] backgroundSr;

    private int count = 0, floor = 0;

    public void SetBackgrounds(background[] backgrounds, int _floor) //��������Ʈ ����
    {
        floor = _floor;

        for (int i = 0; i < GameManager.BACKGROUND_CNT; i++)
        {
            backgroundObjs[i].backgroundType = backgrounds[i];
            backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds[i]];

            SetIgnoreInteraction(i);
        }
    }

    private void SetIgnoreInteraction(int i) //�� ȣ�� ���ϱ� (�迭 �ε���)
    {
        if (backgroundObjs[i].backgroundType == background.Lighted) //���� Ȱ��ȭ �Ǿ��ִٸ� �� ��ȣ ���ϱ�
        {
            count++;
            backgroundObjs[i].roomCount = (floor + 1) * 100 + count;
        }

        switch (backgroundObjs[i].backgroundType)
        {
            
        }
    }

    public void SetBackground(background backgrounds, int i) //�ϳ��� ��׶��常 ���� (��׶���, �迭 �ε���)
    {
        backgroundObjs[i].backgroundType = backgrounds;
        backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds];

        SetIgnoreInteraction(i);
    }
}