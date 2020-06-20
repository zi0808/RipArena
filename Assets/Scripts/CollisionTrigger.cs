using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 간단한 충돌 트리거 - 김용현
/// </summary>
public class CollisionTrigger : MonoBehaviour
{
    public float Delay = 1f;
    public UnityEvent NotifyOnCollisionEnter;
    public UnityEvent NotifyOnTriggerEnter;
    bool Locked = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator WaitAndUnlock()
    {
        Locked = true;
        yield return new WaitForSeconds(Delay);
        Locked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Locked)
            return;
        NotifyOnCollisionEnter?.Invoke();
        StartCoroutine(WaitAndUnlock());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Locked)
            return;
        NotifyOnTriggerEnter?.Invoke();
        StartCoroutine(WaitAndUnlock());
    }
}
