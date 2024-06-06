using UnityEngine;

public class Floor : MonoBehaviour
{
    public Background[] backgroundObjs;

    [SerializeField] SpriteRenderer[] backgroundSr;

    public void SetBackgrounds(background[] backgrounds) //스프라이트 변경
    {
        for (int i = 0; i < GameManager.BACKGROUND_CNT; i++)
        {
            SetBackground(backgrounds[i], i, 1);
        }
    }

    public void SetBackground(background backgrounds, int i, int roomCount) //하나의 백그라운드만 선언 (백그라운드, 배열 인덱스)
    {
        backgroundObjs[i].backgroundType = backgrounds;
        backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds];

        if (backgroundObjs[i].backgroundType == background.Unlighted) roomCount = 0; 
        backgroundObjs[i].roomCount = roomCount;
    }
}