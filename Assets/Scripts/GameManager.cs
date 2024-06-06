using UnityEngine;
using UnityEngine.UI;
public enum background
{
    None = 0,
    UpStairs,
    DownStairs,
    Lighted,
    Unlighted,
    LightedPot, //�ֹ� ������ ��
    UnlightedPot, //�ֹ� ���� ��
    MilkPot,
    Milk,
    BoilerRoom,
    FireWall
}

public enum gameItem
{
    None,
    Glass,
    Mlik
}

public class GameManager : MonoBehaviour
{
    Floor[] floors; //�� �����յ�
    [SerializeField] Floor floorOrigin; //�� ������

  // [SerializeField] private GameObject[] floorObj; //�� �����Ѵٳ׿�

    public Sprite[] backgroundSprite;

    [SerializeField] Image floorImage;
    [SerializeField] Sprite[] floorSprite;

    [SerializeField] Transform playerTf;

    public bool isAllMove= true ;
    public static GameManager instance;

    public int floorCnt = 3; //�� �� ����
    public const float FLOOR_INTERVAL = 16; //�� ���� y ����

    private int doorCount = 0, npcDoorCount = 0; //�ʼ� �� ī��Ʈ (tmp)



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
        background[] backgrounds = new background[7]; //�� ������ ���̴� �� ���� ����
        //Ư�� ���� ������ �˰� ������ floors[i].backgroundObjs[j]��

        int[] upStairsPos = new int[floorCnt]; //������ UpStairs ��ġ �ε���

        for (int i = 0; i < floorCnt; i++)
        {
            while (true) //��� ����
            {
                if (i != 0) backgrounds[upStairsPos[i - 1] * 3] = background.DownStairs; //���� ���� UpStairs�� �ִ� �ڸ��� DownStairs �ֱ�

                upStairsPos[i] = Random.Range(0, 3);
                if (backgrounds[upStairsPos[i] * 3] != background.None) continue;

                backgrounds[upStairsPos[i] * 3] = background.UpStairs;

                switch (upStairsPos[i]) //���Ϸ���/��ȭ�� ����   
                {
                    case 0: //����� �ǿ����� ��� �����ʿ� �ֱ�
                        backgrounds[Random.Range(4, 6)] = (background)Random.Range(9, 11);
                        break;

                    case 1: //����� �߰��� ��� DownStairs�� ���� �ʿ� �ֱ�
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

                    case 2: //����� �ǿ������� ��� ���ʿ� �ֱ�
                        backgrounds[Random.Range(1, 3)] = (background)Random.Range(9, 11);
                        break;
                }
                break;
            }

            for (int j = 0; j < Random.Range(1, 3); j++) //�����ٱ��� ����. ���� 1~3��
            {
                while (true)
                {
                    int milk = Random.Range(0, 7);

                    if (backgrounds[milk] != background.None) continue;

                    background _mlik = (background)Random.Range(7, 9);

                    if (_mlik == background.MilkPot) continue;
                    backgrounds[milk] = _mlik;
                    break;
                }
            }

            for (int j = 0; j < 7; j++) //�� �� �� ����
            {
                if (backgrounds[j] != background.None) continue;

                while (true)
                {
                    background door = (background)Random.Range(3, 6);

                    backgrounds[j] = door;
                    break;
                }
            }
            floors[i] = Instantiate(floorOrigin, Vector2.zero + Vector2.up * FLOOR_INTERVAL * i, Quaternion.identity);
            floors[i].SetBackgrounds(backgrounds, i);

            for (int j = 0; j < 7; j++) //backgrounds �ʱ�ȭ
            {
                if (backgrounds[j] == background.Lighted) doorCount++;
                else if (backgrounds[j] == background.LightedPot) npcDoorCount++;

                backgrounds[j] = background.None;
            }
        }
        

        while(doorCount < 9) //���� �� 9�� ��ġ
        {
            int _floor = Random.Range(0, 3);
            int _roomNum = Random.Range(0, 7);

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

        while(npcDoorCount < 3) //NPC�� 3�� ��ġ
        {
            int _floor = Random.Range(0, 3);
            int _roomNum = Random.Range(0, 7);

            switch (floors[_floor].backgroundObjs[_roomNum].backgroundType)
            {
                case background.Milk:
                    npcDoorCount++;
                    floors[_floor].SetBackground(background.LightedPot, _roomNum);
                    break;
                default:
                    break;
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

    public void ChangeCurrentFloor(int upDown = 1) //�⺻�� UPs
    {
        CurrentFloor += upDown;
        //SetCurrentFloorBgs((background) ((upDown == 1) ? 2: 1));

        playerTf.position += Vector3.up * upDown * FLOOR_INTERVAL;

        Enemy.instance.SetTarget();
        //if (upDown == -1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f); //��� ��ġ�� �̵�
        //else if (upDown == 1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f);
    }

    //void SetTransformScale(Vector2 trans, Vector2 scale) //�÷��̾� ��ġ ����
    //{
    //    GameObject _player = GameObject.FindWithTag("Player");

    //    _player.transform.position = trans;
    //    _player.transform.localScale = scale;
    //}

    public void GameOver()
    {

    }
}
