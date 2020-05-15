using arena.combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRocketLauncher : MonoBehaviour, IShootable
{
    public GameObject Prefab;

    public void Shoot()
    {
        Instantiate(Prefab, transform.position,
            Quaternion.Euler(transform.eulerAngles + transform.localEulerAngles));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
