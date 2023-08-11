using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    List<AudioSource> _audioSources = new List<AudioSource>();
    Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
    public float bgmvolume, effectvolume, totalvolume;
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");

        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            
            for(int i = 0; i < soundNames.Length; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources.Add(go.AddComponent<AudioSource>());
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";
        
        AudioClip audioClip = GetOrAddAudioClip(path);
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = bgmvolume * totalvolume;
            audioSource.Play();
        }

        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.volume = effectvolume * totalvolume;
            audioSource.PlayOneShot(audioClip);
        }
    }

    AudioClip GetOrAddAudioClip(string path)
    {
        AudioClip audioClip = null;
        if(!_clips.TryGetValue(path, out audioClip))
        {
            audioClip = Manager.Resource.Load<AudioClip>(path);
            _clips.Add(path, audioClip);
        }
        
        return audioClip;
    }
    public void SetBgmVolume()
    {
        AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
        audioSource.volume = bgmvolume * totalvolume;
    }
    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
    }
}
