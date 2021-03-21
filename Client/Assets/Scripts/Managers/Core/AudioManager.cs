using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AudioManager

{
    AudioSource[] _audioSources = new AudioSource[(int)AudioRole.MaxCount];             // AudioSource : 사운드 플레이어
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();    // 플레이할 음원 캐싱

    public void Init()
    {
        // 오디오소스 생성
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(AudioRole));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();// AudioSource컴포넌트 붙여주기 
                go.transform.parent = root.transform;       // @Sound산하에 넣기
            }

            _audioSources[(int)AudioRole.Bgm].loop = true;
        }
    }

    // 오디오 클립 비우기
    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        // 캐싱 날리기
        _audioClips.Clear();
    }

    // 오디오 클립 경로로 실행
    public void Play(string path, AudioRole type = AudioRole.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    // 오디오 클립 실행
    public void Play(AudioClip audioClip, AudioRole type = AudioRole.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == AudioRole.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)AudioRole.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)AudioRole.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    // 3D 사운드
    public void PlayClipAtPoint(string path, Vector3 position)
    {
        AudioClip audioClip = GetOrAddAudioClip(path);
        PlayClipAtPoint(audioClip, position);
    }
    public void PlayClipAtPoint(AudioClip audioClip, Vector3 position)
    {
        if (audioClip == null)
            return;
        // 한번 플레이 하고 바로 삭제되는 오디오소스
        AudioSource.PlayClipAtPoint(audioClip, position);
    }

    AudioClip GetOrAddAudioClip(string path, AudioRole type = AudioRole.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        // BGM은 어쩌다 한번 바뀌니 캐싱없이!
        if (type == AudioRole.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            // 이펙트는 자주쓰니 캐싱
            // _audioClips에 없으면
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                // 추가
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }
}
