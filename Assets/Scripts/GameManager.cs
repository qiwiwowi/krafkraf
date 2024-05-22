using UnityEditor.Networking.PlayerConnection;
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
    background[] backgrounds = new background[7]; //�� ������ ���̴� �� ���� ����
    //Ư�� ���� ������ �˰� ������ floors[i].backgroundObjs[j]��

    Floor[] floors; //�� �����յ�
    [SerializeField] Floor floorOrigin; //�� ������

    public Sprite[] backgroundSprite;

    [SerializeField] Image floorImage;
    [SerializeField] Sprite[] floorSprite;

    [SerializeField] private Player playerCharactor;

    public bool isAllMove;
    public static GameManager instance;

    [SerializeField] int floorCnt = 3; //�� �� ����
    const float floorInterval = 16; //�� ���� y ����

    //Vector3 StairPos;
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
        instance = this;
        isAllMove = true;

        floors = new Floor[floorCnt];

        InstantitateFloors();
        //SetCurrentFloorBgs();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void InstantitateFloors()
    {
        int stairs = 0;
        for (int i = 0; i < floorCnt; i++)
        {
            while (true) //��� ����
            {
                if (i != 0) backgrounds[stairs * 3] = background.DownStairs; //���� ���� UpStairs�� �ִ� �ڸ��� DownStairs �ֱ�

                stairs = Random.Range(0, 3);
                if (backgrounds[stairs * 3] != background.None) continue;

                backgrounds[stairs * 3] = background.UpStairs;

                switch (stairs) //���Ϸ���/��ȭ�� ����
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

            bool hasPot = false; //ȭ���� �ִ� ���� ���Դ°�? ���� ȭ�� ����: 0~1��

            for (int j = 0; j < Random.Range(1, 3); j++) //�����ٱ��� ����. ���� 1~3��
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

            for (int j = 0; j < 7; j++) //�� �� �� ����
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

            floors[i] = Instantiate(floorOrigin, Vector3.zero + Vector3.up * floorInterval * i, Quaternion.identity);
            floors[i].SetBackgrounds(backgrounds);

            for (int j = 0; j < 7; j++) //backgrounds �ʱ�ȭ
            {
                backgrounds[j] = background.None;
            }
        }
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

        playerCharactor.transform.position += Vector3.up * upDown * floorInterval;

        //if (upDown == -1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f); //��� ��ġ�� �̵�
        //else if (upDown == 1) SetTransformScale(new Vector2(StairPos.x, -3.1f), Vector2.one * 0.38f);
    }

    //void SetTransformScale(Vector2 trans, Vector2 scale) //�÷��̾� ��ġ ����
    //{
    //    GameObject _player = GameObject.FindWithTag("Player");

    //    _player.transform.position = trans;
    //    _player.transform.localScale = scale;
    //}
}
