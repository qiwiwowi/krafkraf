using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class TextManager : MonoBehaviour
{
    [SerializeField] private string[] textMessage;
    [SerializeField] private TextMeshProUGUI barText;

    [SerializeField] private float speedFactor;

    private void Start()
    {
            StartCoroutine(TypeText());
        
    }

    IEnumerator TypeText() //텍스트 타이핑 코루틴
    {
        foreach(string _str in textMessage)
        {
            barText.text = ""; //텍스트 초기화

            for(int i = 0; i < _str.Length; i++)
            {
                barText.text = _str.Substring(0, i);

                yield return new WaitForSeconds(speedFactor * 0.05f);
            }

            yield return new WaitForSeconds(2);
        }
    }
}
