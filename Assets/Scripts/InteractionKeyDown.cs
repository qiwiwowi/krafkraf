using UnityEngine;
using UnityEngine.UI;

public class InteractionKeyDown : MonoBehaviour
{
    public static InteractionKeyDown instance;

    [SerializeField] private Image _image;
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

    public void SetPosition(bool _isTrigger, Vector3 _vector)
    {
        isTrigger = _isTrigger;
        _image.enabled = isTrigger;
        imageVector = _vector;

        if (isTrigger) transform.position = imageVector;
    }
}
