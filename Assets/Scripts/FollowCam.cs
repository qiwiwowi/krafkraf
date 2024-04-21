using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    private GameObject _childPlayer;
    private CinemachineVirtualCamera _virtual;

    // Start is called before the first frame update
    void Start()
    {
        _childPlayer = GameObject.FindWithTag("Player");
        _virtual = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_childPlayer.transform.position.x > 9 && _childPlayer.transform.position.x < 50)
        {
            _virtual.enabled = true;
        }
        else _virtual.enabled = false;
    }
}
