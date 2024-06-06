using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionKeyDown : MonoBehaviour
{
    public static InteractionKeyDown instance;

    [SerializeField] private Image _image, _interactionkey;

    public Vector3 imageVector;
    private bool isTrigger;
    //public bool IsTrigger
    //{
    //    get
    //    {
    //        return isTrigger;
    //    }
    //    set
    //    {

    //    }
    //}
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isTrigger = false;
        imageVector = Vector3.one;
    }

    private void Update()
    {
        
    }

    public void SetPosition(bool _isTrigger, Vector3 _vector) //키보드 스프라이트가 따라감
    {
        isTrigger = _isTrigger;

        if (_image == null) return;

        _image.enabled = isTrigger;
        _interactionkey.enabled = isTrigger;

        imageVector = _vector;

        if (isTrigger)  transform.position = imageVector;
    }
}
