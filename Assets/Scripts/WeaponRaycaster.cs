using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace arena.combat
{
    public struct DamageParam
    {
        public uint damage;
        public Vector3 forward;
        public Vector3 pos;
        public int kick;
    }

    public interface IDamageReaction
    {
        void Damage(DamageParam param);
    }
    public interface IShootable
    {
        void Shoot();
    }

    public class WeaponRaycaster : MonoBehaviour, IShootable
    {
        public enum SpreadType
        {
            Default,
            Random,
        }

        Camera mainCam;
        public SpreadType WeaponSpreadType;
        public uint PerShotCount = 12;
        public uint Damage = 15;
        public float KickMult = 1;
        public float minDistance = 15;
        public float maxDistance = 200;
        public float minSpread = 10;
        public float maxSpread = 15;
        public float SpreadKickPerShot = 15f;
        public ParticleSystem HitEffect;

        float currentSpread = 0;
        Coroutine RecoverKickRoutine;
        // Start is called before the first frame update
        void Start()
        {
            mainCam = Camera.main;
            currentSpread = minSpread;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Shoot()
        {
            if (PerShotCount > 1)
            {
                for (int i = 0; i < PerShotCount; i++)
                {
                    CastRay(currentSpread);
                }
            }
            else
            {
                CastRay(currentSpread);
            }
            currentSpread += SpreadKickPerShot;
            if (RecoverKickRoutine == null)
                RecoverKickRoutine = StartCoroutine(RecoverFromKick());
        }
        IEnumerator RecoverFromKick()
        {
            float decrease_amount = SpreadKickPerShot * 2;
            while (currentSpread > minSpread)
            {
                currentSpread = decrease_amount * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            RecoverKickRoutine = null;
        }

        private void CastRay(float spread_angle)
        {
            Vector3 Forward = mainCam.transform.forward;
            Quaternion RotationVector = Quaternion.Euler(Random.Range(-spread_angle, spread_angle),
                Random.Range(-spread_angle, spread_angle), 0);
            Forward = RotationVector * Forward;
            Ray ray = new Ray(mainCam.transform.position, Forward);
            RaycastHit rhit;
            if (Physics.Raycast(ray, out rhit, maxDistance,
                ~(1 << LayerMask.NameToLayer("Explosion") |
                1 << LayerMask.NameToLayer("Ignore Raycast"))))
            {
                GameObject ParticleObj = Instantiate(HitEffect.gameObject);
                ParticleObj.transform.position = rhit.point;
                ParticleObj.transform.forward = rhit.normal;
                float DistanceMult = rhit.distance > minDistance ?
                    1.0f - rhit.distance / maxDistance : 1;
                uint CalibratedDamage = (uint)(Damage * DistanceMult);

                DamageParam param = new DamageParam
                {
                    damage = CalibratedDamage,
                    forward = ray.direction,
                    pos = rhit.point,
                    kick = (int)(CalibratedDamage * KickMult),
                };

                rhit.collider.GetComponent<IHasHealth>()?.Damage(param);
                rhit.collider.gameObject.SendMessage("Damage", param,
                    SendMessageOptions.DontRequireReceiver);
                Destroy(ParticleObj, 10f);
            }
        }
    }
}