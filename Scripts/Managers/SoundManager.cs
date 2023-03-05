using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];   // 지금은 맥스카운트가 기본적으로 2임

    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    // Player   -> AudioSource
    // 음원     -> Audio Clip
    // 관객(귀) -> AudioListener

    public void Init()
    {
        GameObject root = GameObject.Find($"@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));   // 리플렉트로 string 뽑아내기
            for (int i = 0; i < soundNames.Length - 1; i++)            // MaxCount 빼야되니까 -1
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;               // UI 에서만 SetParent()를 씀
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
            _audioSources[(int)Define.Sound.Bgm].volume = 0.2f;
        }
    }

    public void Clear()         // 게임하는 중에 플레이어가 이곳저곳 다니면서 사운드가 지워지지 않고 계속 쌓이면 메모리가 박살날 확률이 높으므로 clear해줘야함
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    // clip을 직접 받는 버전
    public void Play(AudioClip clip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (clip == null)
            return;

        if (type == Define.Sound.Bgm)
        {

            //TODO
            AudioSource bgmSource = _audioSources[(int)Define.Sound.Bgm];
            if (bgmSource.isPlaying)
                bgmSource.Stop();

            bgmSource.pitch = pitch;
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            //AudioClip clip = Managers.Resource.Load<AudioClip>(path);         // effect 사운드같은경우 자주 나오는데 계속 이렇게 불러오면 부하가 엄청나다.
            AudioSource effectSource = _audioSources[(int)Define.Sound.Effect];
            effectSource.pitch = pitch;
            effectSource.PlayOneShot(clip);
        }
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        //if (path.Contains("Sounds/") == false)
        //    path = $"Sounds/{path}";

        AudioClip clip = GetOrAddAudioClip(path, type);

        Play(clip, type, pitch);
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;


        if (type == Define.Sound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);


            ////TODO
            //AudioSource bgmSource = _audioSources[(int)Define.Sound.Bgm];
            //if (bgmSource.isPlaying)
            //    bgmSource.Stop();

            //bgmSource.pitch = pitch;
            //bgmSource.clip = clip;
            //bgmSource.Play();
        }
        else
        {
            //AudioClip clip = Managers.Resource.Load<AudioClip>(path);         // effect 사운드같은경우 자주 나오는데 계속 이렇게 불러오면 부하가 엄청나다.

            if (_audioClips.TryGetValue(path, out audioClip) == false)      // true면 audioClip에다가 넣어주는듯
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }          // 그래서 이렇게 캐싱을 해야한다고 함. 이게 일종의 풀링(Pooling)이라고 함


            //AudioSource effectSource = _audioSources[(int)Define.Sound.Effect];
            //effectSource.pitch = pitch;
            //effectSource.PlayOneShot(clip);
        }

        if (audioClip == null)
        {
            Debug.Log($"AudioClip Missing! {path}");
        }

        return audioClip;
    }

}
