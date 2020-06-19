using arena.combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : ReactToDamage
{
    FPControl current_fp_control;
    public PlaySounds PainSounds;
    public override void Start()
    {
        base.Start();
        current_fp_control = GetComponent<FPControl>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Damage(new DamageParam
            {
                damage =
                other.GetComponent<JointCharacter>().damage,
                forward = other.transform.forward,
                kick = 5,
            });
        }

    }
    public override void Damage(DamageParam param)
    {
        current_fp_control.Shock(param.forward, param.kick);
        //current_fp_control.cCont.Move(param.forward * param.kick);

        base.Damage(param);
        PainSounds.PlaySound();
    }

}
