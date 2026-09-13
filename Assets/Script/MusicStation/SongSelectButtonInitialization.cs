using UnityEngine;

public class SongSelectButtonInitialization : MonoBehaviour
{
    public MusicData[] allMusic;
    public SongSelectButton buttonPrefab;

    void Start()
    {
        for (int i = 0; i < allMusic.Length; i++)
        {
            Instantiate(buttonPrefab, transform.position, transform.rotation, transform).Init(allMusic[i]);
        }
    }
}
