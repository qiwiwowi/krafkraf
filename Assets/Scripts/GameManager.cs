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
    Floor[] floors; //층 프리팹들
    [SerializeField] Floor floorOrigin; //층 프리팹

    public Sprite[] backgroundSprite;

    [SerializeField] Image floorImage;
    [SerializeField] Sprite[] floorSprite;

    [SerializeField] Transform playerTf;

    public bool isAllMove= true ;
    public static GameManager instance;

    public int floorCnt = 3; //층 수 설정
    public const float FLOOR_INTERVAL = 16; //층 생성 y 간격

    //Vector3 StairPos;
    public int currentFloor = 0;
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
        instance = this;
        isAllMove = true;

        Cursor.lockState = CursorLockMode.Locked;

        floors = new Floor[floorCnt];

        InstantitateFloors();
        //SetCurrentFloorBgs();
    }

    void InstantitateFloors()
    {
        background[] backgrounds = new background[7]; //층 생성에 쓰이는 한 층의 정보
        //특정 층의 정보를 알고 싶으면 floors[i].backgroundObjs[j]로

        int[] upStairsPos = new int[floorCnt]; //층들의 UpStairs 위치 인덱스

        for (int i = 0; i < floorCnt; i++)
        {
            while (true) //계단 설정
            {
                if (i != 0) backgrounds[upStairsPos[i - 1] * 3] = background.DownStairs; //이전 층의 UpStairs가 있던 자리에 DownStairs 넣기

                upStairsPos[i] = Random.Range(0, 3);
                if (backgrounds[upStairsPos[i] * 3] != background.None) continue;

                backgrounds[upStairsPos[i] * 3] = background.UpStairs;

                switch (upStairsPos[i]) //보일러실/소화전 설정
                {
                    case 0: //계단이 맨왼쪽인 경우 오른쪽에 넣기
                        backgrounds[Random.Range(4, 6)] = (background)Random.Range(9, 11);
                        break;

                    case 1: //계단이 중간인 경우 DownStairs가 없는 쪽에 넣기
                        while (true)
                        {
                            int hide = Random.Range(1, 6);
                            if (hide != 3)
                            {
                                if (backgrounds[0] == background.DownStairs && hide < 3) continue;
                                else if (backgrounds[6] == background.DownStairs && hide > 3) continue;
                                backgrounds[hide] = (background)Random.Range(9, 11);
                                break;
                            }
                        }
                        break;

                    case 2: //계단이 맨오른쪽인 경우 왼쪽에 넣기
                        backgrounds[Random.Range(1, 3)] = (background)Random.Range(9, 11);
                        break;
                }
                break;
            }

            bool hasPot = false; //화분이 있는 문이 나왔는가? 층당 화분 개수: 0~1개

            for (int j = 0; j < Random.Range(1, 3); j++) //우유바구니 설정. 층당 1~3개
            {
                while (true)
                {
                    int milk = Random.Range(0, 7);

                    if (backgrounds[milk] != background.None) continue;

                    backgrounds[milk] = (background)Random.Range(7, 9);
                    if (backgrounds[milk] == background.MilkPot) hasPot = true;
                    break;
                }
            }

            for (int j = 0; j < 7; j++) //그 외 문 설정
            {
                if (backgrounds[j] != background.None) continue;

                while (true)
                {
                    background door = (background)Random.Range(3, 7);

                    if (door < background.LightedPot)
                    {
                        backgrounds[j] = door;
                        break;
                    }
                    else if (!hasPot)
                    {
                        backgrounds[j] = door;
                        hasPot = true;
                        break;
                    }
                }
            }

            floors[i] = Instantiate(floorOrigin, Vector2.zero + Vector2.up * FLOOR_INTERVAL * i, Quaternion.identity);
            floors[i].SetBackgrounds(backgrounds);

            for (int j = 0; j < 7; j++) //backgrounds 초기화
            {
                backgrounds[j] = background.None;
            }
        }

        float[] stairsPos = { floorOrigin.backgroundObjs[0].transform.position.x, floorOrigin.backgroundObjs[3].transform.position.x, floorOrigin.backgroundObjs[6].transform.position.x };
        Enemy.instance.SetStairsPos(stairsPos, upStairsPos);
    }

    //void SetCurrentFloorBgs(background type = background.DownStairs)
    //{
    //    for (int i = 0; i < 7; i++)
    //    {
    //        //backgroundObjs[i].backgroundType = backgrounds[currentFloor, i];
    //        //backgroundSr[i].sprite = backgroundSprite[(int)backgrounds[currentFloor, i]];

    //        //if (backgroundObjs[i].backgroundType == type) StairPos = backgroundSr[i].transform.position;
    //    }
    //}

    public void ChangeCurrentFloor(int upDown = 1) //기본값 UPs
    {
        CurrentFloor += upDown;
        //SetCurrentFloorBgs((background) ((upDown == 1) ? 2: 1));

        playerTf.position += Vector3.up * upDown * FLOOR_INTERVAL;

        Enemy.instance.SetTarget();
        Enemy.instance.OnStairs(null);
        //if (upDown == -1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f); //계단 위치로 이동
        //else if (upDown == 1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f);
    }

    //void SetTransformScale(Vector2 trans, Vector2 scale) //플레이어 위치 설정
    //{
    //    GameObject _player = GameObject.FindWithTag("Player");

    //    _player.transform.position = trans;
    //    _player.transform.localScale = scale;
    //}
}
