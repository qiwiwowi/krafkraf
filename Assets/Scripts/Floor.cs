using UnityEngine;

public class Floor : MonoBehaviour
{
    public Background[] backgroundObjs;
    [SerializeField] SpriteRenderer[] backgroundSr;

    public void SetBackgrounds(background[] backgrounds)
    {
        for (int i = 0; i < 7; i++)
        {
            backgroundObjs[i].backgroundType = backgrounds[i];
            backgroundSr[i].sprite = GameManager.instance.backgroundSprite[(int)backgrounds[i]];
        }
    }
}