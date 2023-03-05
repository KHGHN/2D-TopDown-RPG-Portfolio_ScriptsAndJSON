using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerController와 MonsterController가 상속받는 필드
public abstract class BaseController : MonoBehaviour
{
    public Rigidbody2D Rigid { get; set; }  // 리지드바디
    public Animator Anim { get; set; }      // 애니메이터
    public Vector2 Direction { get; set; }  // 방향
    public float MoveSpeed { get; set; }    // 이동속도

 }
