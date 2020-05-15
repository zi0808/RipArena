using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float RocketSpeedMult = 10;
    public float SelfDestTimer = 15;

    public GameObject ExplosionPrefab;
    public ParticleSystem detach_trail;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * RocketSpeedMult;

        if (SelfDestTimer > 0)
            SelfDestTimer -= Time.deltaTime;
        else
            Trigger();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 1 << LayerMask.NameToLayer("Explosion"))
            return;
        Trigger();
    }

    public void Trigger()
    {
        detach_trail.transform.parent = null;
        detach_trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Instantiate(ExplosionPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);

    }
}
