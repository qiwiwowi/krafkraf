using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] CinemachineVirtualCamera _virtual;

    // Update is called once per frame
    void Update()
    {
        if ((player.transform.position.x > 4 && player.transform.position.x < 50.3 && player.transform.position.y == -3.1f))
        {
            _virtual.enabled = true;
        }
        else _virtual.enabled = false;
    }
}
