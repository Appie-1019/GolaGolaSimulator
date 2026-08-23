using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public struct QueuedAudio
{
    public AudioClip Clip;
    public float Volume;
    public float Pitch;
    public bool IsLoop;
}

public class AudioInstance : MonoBehaviour
{
    [HideInInspector] public bool IsPaused { get; private set; } = false;

    private HashSet<string> tags;
    private Queue<QueuedAudio> audioQueue;
    private bool isStopping = false;
    private Coroutine setVolumeCoroutine;
    private Coroutine setPitchCoroutine;
    private Coroutine checkPlayingCoroutine;
    private AudioSource AS;
    private AudioSource nextAS;

    private void Awake()
    {
        AS = gameObject.AddComponent<AudioSource>();
        AS.playOnAwake = false;

        nextAS = gameObject.AddComponent<AudioSource>();
        nextAS.playOnAwake = false;
    }

    public void Init(AudioClip clip, float asVolume, float pitch, bool is3D, AudioMixerGroup mixerGroup, bool isLoop)
    {
        isStopping = false;
        IsPaused = false;
        if (tags != null) tags.Clear();
        if (audioQueue != null) audioQueue.Clear();
        nextAS.Stop();

        AS.clip = clip;
        SetVolume(asVolume);
        SetPitch(Mathf.Clamp(pitch, 0.1f, 3f));
        AS.spatialBlend = is3D ? 1.0f : 0.0f;
        AS.outputAudioMixerGroup = mixerGroup;
        AS.loop = isLoop;
        AS.Play();

        if (checkPlayingCoroutine != null) StopCoroutine(checkPlayingCoroutine);

        if (!isLoop)
        {
            checkPlayingCoroutine = StartCoroutine(CheckPlayingAndDestroy());
        }
    }

    public AudioInstance SetVolume(float volume, float duration = 0.0f)
    {
        if (setVolumeCoroutine != null) StopCoroutine(setVolumeCoroutine);

        if (duration <= 0.0f) AS.volume = volume;
        else setVolumeCoroutine = StartCoroutine(SetVolumeCoroutine(volume, duration));

        return this;
    }

    private IEnumerator SetVolumeCoroutine(float targetVolume, float duration)
    {
        float elapsedTime = 0;
        float startVolume = AS.volume;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            AS.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }

        AS.volume = targetVolume;
        setVolumeCoroutine = null;
    }

    public AudioInstance SetPitch(float pitch, float duration = 0.0f)
    {
        if (setPitchCoroutine != null) StopCoroutine(setPitchCoroutine);

        if (duration <= 0.0f) AS.pitch = pitch;
        else setPitchCoroutine = StartCoroutine(SetPitchCoroutine(pitch, duration));

        return this;
    }

    private IEnumerator SetPitchCoroutine(float targetPitch, float duration)
    {
        float elapsedTime = 0;
        float startPitch = AS.pitch;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            AS.pitch = Mathf.Lerp(startPitch, targetPitch, elapsedTime / duration);
            yield return null;
        }

        AS.pitch = targetPitch;
        setPitchCoroutine = null;
    }

    private IEnumerator CheckPlayingAndDestroy()
    {
        bool hasScheduledNext = false;

        while (AS.isPlaying || IsPaused)
        {
            if (!IsPaused && !hasScheduledNext && audioQueue != null && audioQueue.Count > 0)
            {
                float currentPitch = Mathf.Max(Mathf.Abs(AS.pitch), 0.01f);
                int remainingSamples = AS.clip.samples - AS.timeSamples;
                float remainingTime = remainingSamples / (float)(AS.clip.frequency * currentPitch);

                if (remainingTime <= 0.1f)
                {
                    ScheduleNextAudio(remainingTime);
                    hasScheduledNext = true;
                }
            }
            yield return null;
        }

        if (!isStopping)
        {
            if (hasScheduledNext)
            {
                SwapAudioSources();
                if (!AS.loop)
                {
                    checkPlayingCoroutine = StartCoroutine(CheckPlayingAndDestroy());
                }
            }
            else
            {
                StopSound();
            }
        }
    }

    public void StopSound(string tag, float duration = 0.0f)
    {
        if (IsTagExists(tag))
        {
            StopSound(duration);
        }
    }

    public void StopSound(string[] tags, float duration = 0.0f)
    {
        if (tags == null || tags.Length == 0) return;
        if (this.tags == null) return;

        foreach (string tag in tags)
        {
            if (!IsTagExists(tag)) return;
        }

        StopSound(duration);
    }

    public void StopSound(float duration = 0.0f)
    {
        if (isStopping) return;
        isStopping = true;
        IsPaused = false;

        if (audioQueue != null) audioQueue.Clear();
        nextAS.Stop();

        if (duration > 0.0f && gameObject.activeInHierarchy)
        {
            StartCoroutine(StopSoundCoroutine(duration));
        }
        else
        {
            OnStopComplete();
        }
    }

    private IEnumerator StopSoundCoroutine(float duration)
    {
        float time = 0;
        float startVolume = AS.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            AS.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }

        OnStopComplete();
    }

    public AudioInstance PauseSound()
    {
        if (AS.isPlaying)
        {
            AS.Pause();
            IsPaused = true;
        }

        return this;
    }

    public AudioInstance ResumeSound()
    {
        if (IsPaused)
        {
            AS.UnPause();
            IsPaused = false;
        }

        return this;
    }

    public bool IsTagExists(string tag)
    {
        if (tags == null) return false;
        return tags.Contains(tag);
    }

    public bool IsTagExists(params string[] tags)
    {
        if (tags == null) return false;

        for (int i = 0; i < tags.Length; i++)
        {
            if (!IsTagExists(tags[i]))
            {
                return false;
            }
        }
        return true;
    }

    public AudioInstance AddTag(string tag)
    {
        if (tags == null) tags = new HashSet<string>();
        tags.Add(tag);

        return this;
    }

    public AudioInstance AddTag(params string[] newTags)
    {
        if (tags == null) tags = new HashSet<string>();
        if (newTags == null || newTags.Length == 0) return this;

        for (int i = 0; i < newTags.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(newTags[i])) continue;
            tags.Add(newTags[i]);
        }

        return this;
    }

    public AudioInstance RemoveTag(string tag)
    {
        if (tags == null) return this;
        tags.Remove(tag);

        return this;
    }

    public AudioInstance RemoveTag(params string[] targetTags)
    {
        if (tags == null) return this;

        for (int i = 0; i < targetTags.Length; i++)
        {
            RemoveTag(targetTags[i]);
            if (tags == null) break;
        }

        return this;
    }

    public AudioInstance EnqueueAudio(AudioClip clip, float volume = -1f, float pitch = -1f, bool isLoop = false)
    {
        if (audioQueue == null) audioQueue = new Queue<QueuedAudio>();

        audioQueue.Enqueue(new QueuedAudio
        {
            Clip = clip,
            Volume = volume,
            Pitch = pitch,
            IsLoop = isLoop
        });

        return this;
    }

    public AudioInstance PlayNextInQueue()
    {
        if (audioQueue == null || audioQueue.Count == 0)
        {
            StopSound();
            return this;
        }

        AS.Stop();
        ScheduleNextAudio(0f);
        SwapAudioSources();

        isStopping = false;
        IsPaused = false;

        if (checkPlayingCoroutine != null) StopCoroutine(checkPlayingCoroutine);

        if (!AS.loop)
        {
            StartCoroutine(CheckPlayingAndDestroy());
        }

        return this;
    }

    private void ScheduleNextAudio(float delayTime)
    {
        QueuedAudio next = audioQueue.Dequeue();

        nextAS.clip = next.Clip;
        nextAS.volume = next.Volume < 0f ? AS.volume : next.Volume;
        nextAS.pitch = next.Pitch < 0f ? AS.pitch : next.Pitch;
        nextAS.loop = next.IsLoop;
        nextAS.spatialBlend = AS.spatialBlend;
        nextAS.outputAudioMixerGroup = AS.outputAudioMixerGroup;

        double exactTime = AudioSettings.dspTime + delayTime;
        nextAS.PlayScheduled(exactTime);
    }

    private void SwapAudioSources()
    {
        AudioSource temp = AS;
        AS = nextAS;
        nextAS = temp;

        nextAS.Stop();
        nextAS.clip = null;
    }

    private void OnStopComplete()
    {
        if (audioQueue != null && audioQueue.Count > 0)
        {
            PlayNextInQueue();
        }
        else
        {
            CleanupAndDestroy();
        }
    }

    private void CleanupAndDestroy()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReturnToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}