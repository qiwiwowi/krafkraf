using UnityEngine;

public class Floor : MonoBehaviour
{
    public Background[] backgroundObjs;

    [SerializeField] SpriteRenderer[] backgroundSr;

    private int count = 0, floor = 0;

    public void SetBackgrounds(background[] backgrounds, int _floor) //스프라이트 변경
    {
        floor = _floor;

        for (int i = 0; i < GameManager.BACKGROUND_CNT; i++)
        {
            backgroundObjs[i].backgroundType = backgrounds[i];
            backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds[i]];

            SetIgnoreInteraction(i);
        }
    }

    private void SetIgnoreInteraction(int i) //방 호수 정하기 (배열 인덱스)
    {
        if (backgroundObjs[i].backgroundType == background.Lighted) //방이 활성화 되어있다면 방 번호 정하기
        {
            count++;
            backgroundObjs[i].roomCount = (floor + 1) * 100 + count;
        }

        switch (backgroundObjs[i].backgroundType)
        {
            
        }
    }

    public void SetBackground(background backgrounds, int i) //하나의 백그라운드만 선언 (백그라운드, 배열 인덱스)
    {
        backgroundObjs[i].backgroundType = backgrounds;
        backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds];

        SetIgnoreInteraction(i);
    }
}