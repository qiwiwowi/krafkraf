using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public enum background
{
    None = 0,
    UpStairs,
    DownStairs,
    Lighted,
    Unlighted,
    NPCDoor, //주민 나오는 문
    NPCDead, //주민 죽은 문
    MilkPot,
    Milk,
    BoilerRoom,
    FireWall
}

public enum gameItem
{
    None,
    Glass,
    Mlik,
    Flash,
    Key
}

public class GameManager : MonoBehaviour
{
    public Floor[] floors; //층 프리팹들
    [SerializeField] Floor floorOrigin; //층 프리팹

  // [SerializeField] private GameObject[] floorObj; //층 고정한다네요

    public Sprite[] backgroundSprite;

    [SerializeField] Image floorImage;
    [SerializeField] Sprite[] floorSprite;

    [SerializeField] Transform playerTf;

    public bool isAllMove= true ;
    public static GameManager instance;

    public int floorCnt = 3; //층 수 설정
    public const int BACKGROUND_CNT = 7; //한 층당 배경 개수
    public const float FLOOR_INTERVAL = 16; //층 생성 y 간격

    private int doorCount = 0, npcDoorCount = 0; //필수 문 카운트 (tmp)

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

    [SerializeField] Animator gameOverAnimator;

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
        background[] backgrounds = new background[BACKGROUND_CNT]; //층 생성에 쓰이는 한 층의 정보
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
                            int hide = Random.Range(1, BACKGROUND_CNT - 1);
                            if (hide != 3)
                            {
                                if (backgrounds[0] == background.DownStairs && hide < 3) continue;
                                else if (backgrounds[BACKGROUND_CNT - 1] == background.DownStairs && hide > 3) continue;
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

            for (int j = 0; j < Random.Range(0, 2); j++) //우유바구니 설정. 층당 0~1개
            {
                while (true)
                {
                    int milk = Random.Range(0, BACKGROUND_CNT);

                    if (backgrounds[milk] != background.None) continue;

                    background _mlik = (background)Random.Range(7, 9);

                    if (_mlik == background.MilkPot) continue;
                    backgrounds[milk] = _mlik;
                    break;
                }
            }

            for (int j = 0; j < BACKGROUND_CNT; j++) //그 외 문 설정
            {
                if (backgrounds[j] != background.None) continue;

                while (true)
                {
                    background door = (background)Random.Range(3, 6);
                    if (npcDoorCount > 3 && door==background.NPCDoor) continue;

                    if (door == background.NPCDoor) npcDoorCount++;

                    backgrounds[j] = door;
                    break;
                }
            }
            floors[i] = Instantiate(floorOrigin, Vector2.zero + Vector2.up * FLOOR_INTERVAL * i, Quaternion.identity);
            floors[i].SetBackgrounds(backgrounds, i);

            for (int j = 0; j < BACKGROUND_CNT; j++) //backgrounds 초기화
            {
                if (backgrounds[j] == background.Lighted) doorCount++;
                else if (backgrounds[j] == background.NPCDoor) npcDoorCount++;

                backgrounds[j] = background.None;
            }
        }
        

        while(doorCount < 6) //밝은 문 9개 배치
        {
            int _floor = Random.Range(0, floorCnt);
            int _roomNum = Random.Range(0, BACKGROUND_CNT);

            switch(floors[_floor].backgroundObjs[_roomNum].backgroundType)
            {
                case background.Unlighted:
                case background.Milk:
                    doorCount++;
                    floors[_floor].SetBackground(background.Lighted, _roomNum);
                    break;
                default:
                    break;
            }
        }

        while(npcDoorCount < 3) //NPC문 3개 배치
        {
            int _floor = Random.Range(0, floorCnt);
            int _roomNum = Random.Range(0, BACKGROUND_CNT);

            switch (floors[_floor].backgroundObjs[_roomNum].backgroundType)
            {
                case background.Milk:
                case background.Unlighted:
                    npcDoorCount++;
                    floors[_floor].SetBackground(background.NPCDoor, _roomNum);
                    break;
                default:
                    break;
            }
        }

        float[] stairsPos = { floorOrigin.backgroundObjs[0].transform.position.x, floorOrigin.backgroundObjs[3].transform.position.x, floorOrigin.backgroundObjs[BACKGROUND_CNT - 1].transform.position.x };
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
        //if (upDown == -1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f); //계단 위치로 이동
        //else if (upDown == 1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f);
    }

    //void SetTransformScale(Vector2 trans, Vector2 scale) //플레이어 위치 설정
    //{
    //    GameObject _player = GameObject.FindWithTag("Player");

    //    _player.transform.position = trans;
    //    _player.transform.localScale = scale;
    //}

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        isAllMove = false;
        Enemy.instance.GameOver();
        InteractionKeyDown.instance.SetPosition(false, Vector3.one);

        gameOverAnimator.SetTrigger("GameOver");
    }
}
