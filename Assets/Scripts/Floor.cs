using UnityEngine;

public class Floor : MonoBehaviour
{
    public Background[] backgroundObjs;

    [SerializeField] SpriteRenderer[] backgroundSr;

    public void SetBackgrounds(background[] backgrounds) //��������Ʈ ����
    {
        for (int i = 0; i < GameManager.BACKGROUND_CNT; i++)
        {
            SetBackground(backgrounds[i], i, 1);
        }
    }

    public void SetBackground(background backgrounds, int i, int roomCount) //�ϳ��� ��׶��常 ���� (��׶���, �迭 �ε���)
    {
        backgroundObjs[i].backgroundType = backgrounds;
        backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds];

        if (backgroundObjs[i].backgroundType == background.Unlighted) roomCount = 0; 
        backgroundObjs[i].roomCount = roomCount;
    }
}