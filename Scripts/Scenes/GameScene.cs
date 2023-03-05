using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 게임 씬 관련 처리 매서드
public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        
        // BGM 재생
        Managers.Sound.Play("Sounds/BGM/joshua-mclean_free-music-pack-8_super_ogg/island-hopper",Define.Sound.Bgm);
    }


    protected override void Clear()
    {
      
    }
}
