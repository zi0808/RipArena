using arena.combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLimb : ReactToDamage
{
    public IJointCharacter Parent;
    public CharacterLimb[] ChildLimbs;
    public float ParentRatio = 1;
    public bool Critical = false;
    public SkinnedMeshRenderer linked_mesh;
    DamageParam Last_Damage_Param;

    static GameObject particle_blood_loop;
    static GameObject particle_blood;

    public override void Start()
    {
        base.Start();
        Parent = GetComponentInParent<IJointCharacter>();

        if (particle_blood_loop == null)
            particle_blood_loop = Resources.Load("Particles/BloodLoop") as GameObject;
        if (particle_blood == null)
            particle_blood = Resources.Load("Particles/BloodOneShot") as GameObject;
    }
    public override void Damage(DamageParam param)
    {
        base.Damage(param);
        DamageParam new_param = new DamageParam
        {
            damage = (uint)(param.damage * ParentRatio),
            kick = param.kick,
            forward = param.forward,
            pos = param.pos
        };
        Parent.Damage(new_param);
        Last_Damage_Param = param;


        GameObject blood = Instantiate(particle_blood);
        blood.transform.position = Last_Damage_Param.pos;
        blood.transform.forward = Last_Damage_Param.forward * -1;
        Destroy(blood, 2.5f);
    }
    public override void OnDeath(IHasHealth object_info)
    {
        LimbDeath();
        foreach (CharacterLimb cl in ChildLimbs)
            cl.LimbDeath(Last_Damage_Param);
        if (Critical)
            Parent.Kill();
        base.OnDeath(object_info);
        Parent.OnJointDeath(this);
    }
    public void LimbDeath()
    {
        LimbDeath(Last_Damage_Param);
    }

    public void LimbDeath(DamageParam new_param)
    {
        Mesh gib_mesh;
        Material gib_mat;
        Last_Damage_Param = new_param;
        if (linked_mesh != null && !Dead)
        {
            Dead = true;
            linked_mesh.gameObject.SetActive(false);
            gib_mesh = linked_mesh.sharedMesh;
            gib_mat = linked_mesh.sharedMaterial;

            GameObject gib = new GameObject("Giblet");
            gib.transform.position = transform.position;
            gib.transform.rotation = transform.rotation;
            gib.AddComponent<MeshFilter>().sharedMesh = gib_mesh;
            MeshCollider MC = gib.AddComponent<MeshCollider>();
            MC.sharedMesh = gib_mesh;
            MC.convex = true;

            GameObject gib_blood = Instantiate(particle_blood_loop);
            gib_blood.transform.position = MC.bounds.center;
            gib_blood.transform.parent = gib.transform;
            gib_blood.transform.forward = Last_Damage_Param.forward * -1;

            gib.AddComponent<MeshRenderer>().sharedMaterial = gib_mat;
            Rigidbody RB = gib.AddComponent<Rigidbody>();
            RB.AddForceAtPosition((Last_Damage_Param.forward + Vector3.up * 0.5f) * Last_Damage_Param.kick * 50,
                Last_Damage_Param.pos);

            Destroy(gib, 10f);
        }
    }
}
