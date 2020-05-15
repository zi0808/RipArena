using arena.combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JointCharacter : ReactToDamage, IJointCharacter
{
    NavMeshAgent agent;
    Animator Anim;
    CharacterLimb[] limbs;

    static FPControl player_target;

    public override void Start()
    {
        if (player_target == null)
            player_target = FindObjectOfType<FPControl>();

        base.Start();
        agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        limbs = GetComponentsInChildren<CharacterLimb>();
        Ragdoll(false);
        StartCoroutine(Follow());
    }

    public IEnumerator Follow()
    {
        while (!Dead)
        {
            agent?.SetDestination(player_target.transform.position);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnJointDeath(IHasHealth limb)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnDeath(IHasHealth object_info)
    {
        base.OnDeath(object_info);
        StopAllCoroutines();
        agent.isStopped = true;
        Ragdoll(true);
    }

    public override void Damage(DamageParam param)
    {
        base.Damage(param);
        if (param.kick > 0)
        {

        }
    }

    public void Ragdoll(bool Toggle = false)
    {
        if (Anim)
            Anim.enabled = !Toggle;
        foreach (CharacterLimb limb in limbs)
        {
            limb.GetComponent<Rigidbody>().isKinematic = !Toggle;
            limb.GetComponent<Rigidbody>().useGravity = Toggle;
        }
    }

    public void FootL()
    {

    }

    public void FootR()
    {

    }
}
