using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static int global_limit = 50;
    public static int global_amount = 0;

    public GameObject SpawnPrefab;
    public bool AutoEnabled = false;
    public float SpawnDelay = 10f;
    public int SpawnAmount = 5;

    static List<SpawnPoint> spawners;

    private void Awake()
    {
        if (spawners == null)
            spawners = new List<SpawnPoint>();
        spawners.Add(this);
    }

    public static void ActivateAll()
    {
        foreach (SpawnPoint spawn in spawners)
            spawn.StartSpawning();
    }

    private void OnDestroy()
    {
        spawners?.Remove(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Spawn()
    {
        Instantiate(SpawnPrefab, transform.position + Vector3.down +
            Vector3.forward * Random.Range(-5, 5) +
            Vector3.right * Random.Range(-5, 5), Quaternion.Euler(0, 0, 0));
        global_amount++;
    }

    public void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
            Spawn();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (global_amount < global_limit)
                Spawn(SpawnAmount);
            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
