using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInstance : MonoBehaviour
{
    public AudioSource AS;

    private HashSet<string> tags;
    private bool isStopping = false;

    public void Init(AudioClip clip, float asVolume, float pitch, bool is3D, AudioMixerGroup mixerGroup, bool isLoop)
    {
        isStopping = false;
        if (tags != null)
        {
            tags.Clear();
        }

        AS.volume = asVolume;
        AS.clip = clip;
        AS.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        AS.spatialBlend = is3D ? 1.0f : 0.0f;
        AS.outputAudioMixerGroup = mixerGroup;
        AS.loop = isLoop;
        AS.Play();

        if (!isLoop)
        {
            StartCoroutine(CheckPlayingAndDestroy());
        }
    }

    private IEnumerator CheckPlayingAndDestroy()
    {
        yield return null;

        while (AS != null && AS.isPlaying)
        {
            yield return null;
        }

        if (!isStopping)
        {
            StopSound();
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

        if (duration > 0.0f && gameObject.activeInHierarchy)
        {
            StartCoroutine(StopSoundCoroutine(duration));
        }
        else
        {
            CleanupAndDestroy();
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

        CleanupAndDestroy();
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

    public bool IsTagExists(string tag)
    {
        if (tags == null) return false;
        return tags.Contains(tag);
    }

    public void AddTag(string tag)
    {
        if (tags == null) tags = new HashSet<string>();
        tags.Add(tag);
    }

    public void AddTag(params string[] newTags)
    {
        if (tags == null) tags = new HashSet<string>();

        for (int i = 0; i < newTags.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(newTags[i])) continue;
            tags.Add(newTags[i]);
        }
    }

    public void RemoveTag(string tag)
    {
        if (tags == null) return;
        tags.Remove(tag);

        if (tags.Count == 0)
        {
            tags = null;
        }
    }

    public void RemoveTag(params string[] targetTags)
    {
        if (tags == null) return;

        for (int i = 0; i < targetTags.Length; i++)
        {
            RemoveTag(targetTags[i]);
            if (tags == null) break;
        }
    }
}