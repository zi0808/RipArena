using arena.combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRocketLauncher : MonoBehaviour, IShootable
{
    public Transform FirePoint;
    public GameObject Prefab;

    public void Shoot()
    {
        Instantiate(Prefab, FirePoint.position,
            FirePoint.rotation);
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
