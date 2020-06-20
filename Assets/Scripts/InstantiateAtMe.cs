using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateAtMe : MonoBehaviour
{
    public GameObject[] Prefab;
    public int Count = 1;
    public Vector3 PRandomizer = new Vector3(0, 0, 0);
    public Vector3 RRandomizer = new Vector3(0, 0, 0);
    public bool ThisAsParent = false;

    public void Make(int count = 1)
    {
        GameObject selected_prefab =
            Prefab[Random.Range(0, Prefab.Length)];

        GameObject G = Instantiate(selected_prefab, ThisAsParent ? transform : null);
        G.transform.position = transform.position;
        G.transform.position += new Vector3(Random.Range(-PRandomizer.x, PRandomizer.x),
            Random.Range(-PRandomizer.y, PRandomizer.y), Random.Range(-PRandomizer.z, PRandomizer.z));
        Vector3 RandomEuler = new Vector3(Random.Range(-RRandomizer.x, RRandomizer.x),
            Random.Range(-RRandomizer.y, RRandomizer.y), Random.Range(-RRandomizer.z, RRandomizer.z));
        RandomEuler += G.transform.eulerAngles;
        G.transform.rotation = Quaternion.Euler(RandomEuler);
    }

    public void Make()
    {
        Make(Count);
    }
}
