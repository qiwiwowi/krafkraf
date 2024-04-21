using UnityEngine;
using UnityEngine.UI;
public enum background
{
    None,
    UpStairs,
    DownStairs,
    Lighted,
    Unlighted,
    LightedPot,
    UnlightedPot,
    MilkPot,
    Milk,
    BoilerRoom,
    FireWall
}

public class GameManager : MonoBehaviour
{
    background[,] backgrounds = new background[10, 7];

    [SerializeField] Background[] backgroundObjs;
    [SerializeField] SpriteRenderer[] backgroundSr;
    [SerializeField] Sprite[] backgroundSprite;

    [SerializeField] Image floorImage;
    [SerializeField] Sprite[] floorSprite;

    int currentFloor = 0;
    int CurrentFloor
    {
        get
        {
            return currentFloor;
        }
        set
        {
            currentFloor = value;
            floorImage.sprite = floorSprite[currentFloor];
        }
    }

    private void Awake()
    {
        SetBackgrounds();
        SetCurrentFloorBgs();
    }

    void SetBackgrounds()
    {
        for (int i = 0; i < 10; i++)
        {
            while (true) //계단 설정
            {
                int stairs = Random.Range(0, 3);
                if (backgrounds[i, stairs * 3] == background.None)
                {
                    backgrounds[i, stairs * 3] = background.UpStairs;
                    if (i < 9) backgrounds[i + 1, stairs * 3] = background.DownStairs;

                    switch (stairs) //보일러실/소화전 설정
                    {
                        case 0:
                            backgrounds[i, Random.Range(4, 6)] = (background)Random.Range(9, 11);
                            break;

                        case 1:
                            while (true)
                            {
                                int hide = Random.Range(1, 6);
                                if (hide != 3)
                                {
                                    backgrounds[i, hide] = (background)Random.Range(9, 11);
                                    break;
                                }
                            }
                            break;

                        case 2:
                            backgrounds[i, Random.Range(1, 3)] = (background)Random.Range(9, 11);
                            break;
                    }
                    break;
                }
            }

            bool hasPot = false;

            for (int j = 0; j < Random.Range(1, 3); j++) //우유바구니 설정
            {
                while (true)
                {
                    int milk = Random.Range(0, 7);

                    if (backgrounds[i, milk] == background.None)
                    {
                        backgrounds[i, milk] = (background)Random.Range(7, 9);
                        if (backgrounds[i, milk] == background.MilkPot) hasPot = true;
                        break;
                    }
                }
            }

            for (int j = 0; j < 7; j++) //그 외 문 설정
            {
                if (backgrounds[i, j] != background.None) continue;

                while (true)
                {
                    background door = (background)Random.Range(3, 7);

                    if (door < background.LightedPot)
                    {
                        backgrounds[i, j] = door;
                        break;
                    }

                    if (door >= background.LightedPot && !hasPot)
                    {
                        backgrounds[i, j] = door;
                        hasPot = true;
                        break;
                    }
                }
            }
        }
    }

    void SetCurrentFloorBgs()
    {
        for (int i = 0; i < 7; i++)
        {
            backgroundObjs[i].backgroundType = backgrounds[currentFloor, i];
            backgroundSr[i].sprite = backgroundSprite[(int)backgrounds[currentFloor, i]];
        }
    }
}
