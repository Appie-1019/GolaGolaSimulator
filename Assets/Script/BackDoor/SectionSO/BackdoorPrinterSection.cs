using UnityEngine;

[CreateAssetMenu(fileName = "BackdoorPrinterSection", menuName = "Backdoor/BackdoorPrinterSection")]
public class BackdoorPrinterSection : ScriptableObject
{
    public float firstWaitTime;
    public BackdoorTextData[] data;
}

[System.Serializable]
public struct BackdoorTextData
{
    public BackdoorTextDataSection[] textSections;
    public float textInterval;
    public float textSize;
    public float textSpacing;
    public float showDuration;
}

[System.Serializable]
public struct BackdoorTextDataSection
{
    [Header("Text")]
    public string text;
    public Color mainColor;
    public Color subColor;

    [Header("Time")]
    public float colorTransitionTime;
    public float appearTime;
    public float disappearTime;

    [Header("Operation")]
    public float idlePower;
    public float idleInterval;
    public TextSectionAppear appear;
    public TextSectionIdle idle;
    public TextSectionDisappear disappear;
    public TextSectionColor color;
    [Header("Sound")]
    public AudioClip[] typeSounds;
}

public enum TextSectionAppear
{
    None, Fade, Expansion
}

public enum TextSectionIdle
{
    None, Vibration, Wobble, Bobbing
}

public enum TextSectionDisappear
{
    None, Fade, Contraction
}

public enum TextSectionColor
{
    None, Fade, Twinkling, Rainbow
}