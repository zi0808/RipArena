using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 간단한 충돌 트리거 - 김용현
/// </summary>
public class CollisionTrigger : MonoBehaviour
{
    public UnityEvent NotifyOnCollisionEnter;
    public UnityEvent NotifyOnTriggerEnter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        NotifyOnCollisionEnter?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        NotifyOnTriggerEnter?.Invoke();
    }
}
