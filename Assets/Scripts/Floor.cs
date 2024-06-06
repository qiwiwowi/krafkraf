using UnityEngine;

public class Floor : MonoBehaviour
{
    public Background[] backgroundObjs;

    [SerializeField] SpriteRenderer[] backgroundSr;

    private int count = 0, floor = 0;

    public void SetBackgrounds(background[] backgrounds, int _floor) //스프라이트 변경
    {
        for (int i = 0; i < 7; i++)
        {
            floor = _floor;

            backgroundObjs[i].backgroundType = backgrounds[i];
            backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds[i]];

            SetIgnoreInteraction(i);
        }
    }

    private void SetIgnoreInteraction(int i) //방 호수 정하기 (배열 인덱스)
    {
        backgroundObjs[i].roomCount = 1; //상호작용 가능 

        if (backgroundObjs[i].backgroundType == background.Lighted) //방이 활성화 되어있다면 방 번호 정하기
        {
            count++;
            backgroundObjs[i].roomCount = (floor + 1) * 100 + count;
        }

        switch (backgroundObjs[i].backgroundType) //상호작용 제한 적용
        {
            case background.Milk:
            case background.Unlighted:
                backgroundObjs[i].roomCount = 0;
                break;
            default:
                break;
        }
    }

    public void SetBackground(background backgrounds, int i) //하나의 백그라운드만 선언 (백그라운드, 배열 인덱스)
    {
        backgroundObjs[i].backgroundType = backgrounds;
        backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds];

        backgroundObjs[i].roomCount = (floor + 1) * 100 + count;

        SetIgnoreInteraction(i);
    }
}