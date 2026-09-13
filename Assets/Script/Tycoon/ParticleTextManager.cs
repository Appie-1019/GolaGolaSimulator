using System.Collections.Generic;
using UnityEngine;

public class ParticleTextManager : MonoBehaviour
{
    public static ParticleTextManager Instance { get; private set; }

    public int initialPoolSize = 5;
    public ParticleTextInstance instancePrefab;
    public Transform pointer;

    private Queue<ParticleTextInstance> textPool = new Queue<ParticleTextInstance>();

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
        for (int i = 0; i < initialPoolSize; i++)
        {
            ParticleTextInstance newInstance = Instantiate(instancePrefab, transform);
            newInstance.gameObject.SetActive(false);
            currentPoolSize++;
            newInstance.name = $"TextInstance[{currentPoolSize}]";
            newInstance.initPos = pointer;
            textPool.Enqueue(newInstance);
        }
    }

    public void BackToPool(ParticleTextInstance instance)
    {
        instance.gameObject.SetActive(false);
        textPool.Enqueue(instance);
    }

    public void AddParticleText()
    {
        if (textPool.Count == 0) return;

        ParticleTextInstance instance = textPool.Dequeue();
        instance.gameObject.SetActive(true);
        instance.Init();
    }
}
