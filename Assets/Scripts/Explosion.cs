using arena.combat;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public enum DamageType
    {
        Default,
        Continous
    }
    public float Size = 4;
    public float lifetime = 2;
    [Header("Effect Parameters")]
    public DamageType type;
    public float effect_duration;
    [Header("Damage Paramter")]
    public uint Damage = 5;
    public int Kick = 5;
    bool Used = false;

    DamageParam exp_param;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (type == DamageType.Continous)
        {
            yield return new WaitForSeconds(effect_duration);
            Used = true;
            yield return new WaitForSeconds(lifetime = effect_duration);
        }
        else
            yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Used)
            return;
        float distance = Vector3.Distance(transform.position, other.transform.position) / Size;
        distance = Mathf.Clamp(distance, 0f, 0.75f);
        exp_param = new DamageParam
        {
            pos = other.ClosestPoint(transform.position),
            damage = (uint)( Damage * (1f - distance)),
            kick = Kick,
            forward = (other.ClosestPoint(transform.position) - transform.position).normalized
        };

        other.GetComponent<IHasHealth>()?.Damage(exp_param);
        if (type == DamageType.Default)
            StartCoroutine(TriggerLock());
    }
    IEnumerator TriggerLock()
    {
        yield return new WaitForEndOfFrame();
        Used = true;
    }
}
