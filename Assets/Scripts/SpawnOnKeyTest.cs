using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnKeyTest : MonoBehaviour
{
    public GameObject Zombie;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            for (int i = 0; i < 20; i++)
            {
                Instantiate(Zombie, transform.position + Vector3.down +
                    Vector3.forward * Random.Range(-5, 5) +
                    Vector3.right * Random.Range(-5, 5)
                    , Quaternion.Euler(0, 0, 0));
            }
        }
    }
}
