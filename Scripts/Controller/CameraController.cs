using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 카메라가 플레이어를 따라갈 수 있도록 임시로 설정
public class CameraController : MonoBehaviour
{
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 카메라의 위치는 플레이어의 위치로 계속 이동
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y,-10.0f);
    }
}
