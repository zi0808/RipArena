using arena.combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JointCharacter : ReactToDamage, IJointCharacter
{
    public uint damage = 2;
    public AnimatorPlaySound FootStepSoundPlayer;
    NavMeshAgent agent;
    Animator Anim;
    CharacterLimb[] limbs;
    bool Jumping = false;
    static FPControl player_target;
    IHasHealth player_health_interface;
    Rigidbody RBody;

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
        float ticker = 0;
        while (!Dead)
        {
            if (ticker > 3)
            {
                agent.SetDestination(transform.position + transform.forward +
                    UnityEngine.Random.Range(-1f, 1f) * transform.forward +
                    UnityEngine.Random.Range(-1f, 1f) * transform.right);
                yield return new WaitForSeconds(0.1f);
                ticker = 0;
            }
            agent?.SetDestination(player_target.transform.position);
            yield return new WaitForSeconds(0.05f);
            ticker += 0.05f;
            //agent?.ResetPath();
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
        GetComponent<Collider>().enabled = false;
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
        FootStepSoundPlayer?.PlaySound();
    }

    public void FootR()
    {
        FootStepSoundPlayer?.PlaySound();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Jumping = false;
            RBody.isKinematic = agent.enabled = true;
        }
    }
    /*
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.controller.gameObject == player_target)
        {
            if (player_health_interface == null)
                player_health_interface = player_target.GetComponent<IHasHealth>();

            player_health_interface.Damage(new DamageParam
            {
                damage = damage,
                forward = transform.forward,
                kick = 5,
            });
        }
    }
    */
}
