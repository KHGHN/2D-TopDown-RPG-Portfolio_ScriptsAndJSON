using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// HP 표시를 위한 HP Bar 처리
public class UI_HPBar : MonoBehaviour
{
    Stat stat;
    Slider slider;
    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        stat = transform.parent.GetComponent<Stat>();
    }

    // Start is called before the first frame update
    void Start()
    {
        slider.transform.rotation = Quaternion.Euler(0, 0, 180);    // 슬라이더가 반대로 된 것 수정
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = stat.Hp / (float)stat.MaxHp;
        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
    {
        slider.value = ratio;
    }
}
