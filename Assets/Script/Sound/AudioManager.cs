using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType { Master, Game, UI }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AudioInstance Prefab")]
    public AudioInstance audioInstancePrefab;

    [Header("Audio Mixer Groups")]
    public AudioMixer mainMixer;
    public AudioMixerGroup MasterMixerGroup;
    public AudioMixerGroup GameMixerGroup;
    public AudioMixerGroup uiMixerGroup;

    private List<AudioInstance> activeAudio = new List<AudioInstance>();
    private Queue<AudioInstance> audioPool = new Queue<AudioInstance>();
    public int initialPoolSize = 20;

    private int currentPoolSize = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentPoolSize = 0;

        for (int i = 0; i < initialPoolSize; i++)
        {
            AudioInstance newInstance = Instantiate(audioInstancePrefab, transform);
            newInstance.gameObject.SetActive(false);
            currentPoolSize++;
            newInstance.name = $"SoundInstance[{currentPoolSize}]";
            audioPool.Enqueue(newInstance);
        }

        SetVolume(DataManager.saveData.UI.MasterVolume / 100, SoundType.Master);
        SetVolume(DataManager.saveData.UI.GameVolume / 100, SoundType.Game);
        SetVolume(DataManager.saveData.UI.UIVolume / 100, SoundType.UI);
    }

    public void SetVolume(float volume0to1, SoundType soundType = SoundType.Master)
    {
        float dB = volume0to1 <= 0.0001f ? -80f : Mathf.Log10(volume0to1) * 20f;
        string type = soundType switch
        {
            SoundType.Master => "Master",
            SoundType.Game => "Game",
            SoundType.UI => "UI",
            _ => "Master"
        };

        mainMixer.SetFloat(type, dB);
    }

    public AudioInstance Play3DSound(AudioClip clip, Vector3 point, SoundType type = SoundType.Game, float asVolume = 1f, float pitch = 1f, bool isLoop = false)
    {
        return PlaySound(clip, type, point, asVolume, pitch, true, isLoop);
    }

    public AudioInstance PlayRandom3DSound(AudioClip[] clips, Vector3 point, SoundType type = SoundType.Game, float asVolume = 1f, float pitch = 1f, bool isLoop = false)
    {
        AudioClip clip = GetRandomClip(clips);
        return PlaySound(clip, type, point, asVolume, pitch, true, isLoop);
    }

    public AudioInstance Play2DSound(AudioClip clip, SoundType type = SoundType.Game, float asVolume = 1f, float pitch = 1f, bool isLoop = false)
    {
        return PlaySound(clip, type, Vector3.zero, asVolume, pitch, false, isLoop);
    }

    public AudioInstance PlayRandom2DSound(AudioClip[] clips, SoundType type = SoundType.Game, float asVolume = 1f, float pitch = 1f, bool isLoop = false)
    {
        AudioClip clip = GetRandomClip(clips);
        return PlaySound(clip, type, Vector3.zero, asVolume, pitch, false, isLoop);
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;

        int randomIndex = Random.Range(0, clips.Length);
        return clips[randomIndex];
    }

    private AudioInstance PlaySound(AudioClip clip, SoundType type, Vector3 point, float asVolume, float pitch, bool is3D, bool isLoop)
    {
        if (clip == null) return null;

        AudioInstance newInstance;
        if (audioPool.Count > 0)
        {
            newInstance = audioPool.Dequeue();
        }
        else
        {
            newInstance = Instantiate(audioInstancePrefab, transform);
            currentPoolSize++;
            newInstance.name = $"SoundInstance[{currentPoolSize}]";
            Debug.Log($"오디오 오브젝트 폴링 갯수 증가됨 : 현재 [{currentPoolSize}]개"); // 자주 호출된다면 initialPoolSize 값을 늘리는 것을 고려할 것.
        }

        newInstance.transform.position = point;
        newInstance.gameObject.SetActive(true);

        AudioMixerGroup targetGroup = type switch
        {
            SoundType.Master => MasterMixerGroup,
            SoundType.Game => GameMixerGroup,
            SoundType.UI => uiMixerGroup,
            _ => MasterMixerGroup
        };

        newInstance.Init(clip, asVolume, pitch, is3D, targetGroup, isLoop);
        activeAudio.Add(newInstance);

        return newInstance;
    }

    public void StopSoundAll(float duration = 0.0f)
    {
        for (int i = activeAudio.Count - 1; i >= 0; i--)
        {
            if (activeAudio[i] == null)
            {
                activeAudio.RemoveAt(i);
                continue;
            }
            activeAudio[i].StopSound(duration);
        }
    }

    public void StopSound(string tag, float duration = 0.0f)
    {
        for (int i = activeAudio.Count - 1; i >= 0; i--)
        {
            if (activeAudio[i] == null)
            {
                activeAudio.RemoveAt(i);
                continue;
            }
            activeAudio[i].StopSound(tag, duration);
        }
    }

    public void StopSound(string[] tags, float duration = 0.0f)
    {
        for (int i = activeAudio.Count - 1; i >= 0; i--)
        {
            if (activeAudio[i] == null)
            {
                activeAudio.RemoveAt(i);
                continue;
            }
            activeAudio[i].StopSound(tags, duration);
        }
    }

    public void ReturnToPool(AudioInstance instance)
    {
        if (activeAudio.Contains(instance))
        {
            activeAudio.Remove(instance);
        }

        audioPool.Enqueue(instance);
        instance.gameObject.SetActive(false);
    }

    public void SetPitch(string tag, float targetPitch, float duration = 0.0f)
    {
        for (int i = activeAudio.Count - 1; i >= 0; i--)
        {
            if (activeAudio[i] == null)
            {
                activeAudio.RemoveAt(i);
                continue;
            }

            if (activeAudio[i].IsTagExists(tag))
            {
                activeAudio[i].SetPitch(targetPitch, duration);
            }
        }
    }

    public AudioInstance GetAudioInstances(params string[] tags)
    {
        if (tags == null || tags.Length == 0) return null;

        for (int i = 0; i < activeAudio.Count; i++)
        {
            if (activeAudio[i] == null) continue;

            if (activeAudio[i].IsTagExists(tags))
            {
                return activeAudio[i];
            }
        }

        return null;
    }

    public AudioInstance[] GetAllAudioInstances(params string[] tags)
    {
        if (tags == null || tags.Length == 0) return new AudioInstance[0];

        List<AudioInstance> result = new List<AudioInstance>();

        for (int i = 0; i < activeAudio.Count; i++)
        {
            if (activeAudio[i] == null) continue;

            if (activeAudio[i].IsTagExists(tags))
            {
                result.Add(activeAudio[i]);
            }
        }

        return result.ToArray();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}