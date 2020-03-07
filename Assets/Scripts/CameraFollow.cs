using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform PlayerCamera;
    public Transform Player;
    float distanceToPlayer = 0;

    void Start()
    {
        distanceToPlayer = PlayerCamera.position.z -  Player.position.z;
    }
    void Update()
    {
        PlayerCamera.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z + distanceToPlayer);
    }
    
}
