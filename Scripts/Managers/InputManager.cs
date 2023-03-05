using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 키보드, 마우스 입력 시 처리하기 위해
public class InputManager
{
    public Action KeyAction = null;

    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false;
    float _pressedTime = 0;

    public void OnUpdate()
    {
        //if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && Input.anyKey && KeyAction != null)
        //    KeyAction.Invoke();

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0)) // Press                                //  누르고만 있어도 이고 Down버전은 눌렀을때 처음만  // 0번은 마우스 왼쪽버튼임
            {
                if (!_pressed)  // 한번도 누른적이 없는 상태라면 false임 다시말해서 처음 누른것
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;   // start하고 흐른 시간

                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)           // Click
                {
                    if (Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false;
                _pressedTime = 0;
            }
        }

    }
}
