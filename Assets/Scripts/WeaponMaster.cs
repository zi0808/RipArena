using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace arena.combat
{
    public class WeaponMaster : MonoBehaviour
    {
        public string WeaponName = "";
        public int MaxAmmo = 2;
        public float RechargeDelay = 1;
        public float DelayPerShot = 0.5f;
        public float ShowDuration = 0.1f;
        public float HideDuration = 0.1f;
        public AnimatorPlaySound SoundPlayer;
        public ParticleSystem ShootParticle;
        IShootable Shooterinterface;
        [HideInInspector]
        public Animator WeaponAnimator;
        bool WLocked = false;
        Coroutine CurrentAnimRoutine;
        Coroutine RechargeCoroutine;
        int CurrentAmmo
        {
            get
            {
                return _current_ammo;
            }
            set
            {
                _current_ammo = value;
                UIGameUI.Instance.AmmoCounterUpdate(CurrentAmmo);
            }
        }
        int _current_ammo;
        private void Start()
        {
            CurrentAmmo = MaxAmmo;
        }
        private void OnEnable()
        {
            if (WeaponAnimator == null)
                WeaponAnimator = GetComponent<Animator>();
            WLocked = true;
            CurrentAnimRoutine = StartCoroutine(WaitForPullOut());
            if (UIGameUI.Instance != null)
                UIGameUI.Instance.AmmoCounterInit(MaxAmmo, WeaponName);
            RechargeCoroutine = StartCoroutine(RechargeRoutine());
        }
        IEnumerator WaitForPullOut()
        {
            WeaponAnimator.SetTrigger("Pullout");
            yield return new WaitForSeconds(ShowDuration);
            WLocked = false;
            CurrentAnimRoutine = null;
        }
        IEnumerator LockFire()
        {
            WLocked = true;
            yield return new WaitForSeconds(DelayPerShot);
            WLocked = false;
        }

        IEnumerator RechargeRoutine()
        {
            while(CurrentAmmo < MaxAmmo)
            {
                yield return new WaitForSeconds(RechargeDelay);
                CurrentAmmo ++;;
            }
            RechargeCoroutine = null;
        }

        public bool ShootWeaponSingle()
        {
            if (WLocked || CurrentAmmo <= 0)
                return false;
            CurrentAmmo--;
            if (Shooterinterface == null)
                Shooterinterface = GetComponent<IShootable>();
            WeaponAnimator.SetTrigger("Shoot");
            ShootParticle.Play();
            Shooterinterface?.Shoot();
            SoundPlayer?.PlaySound();
            StartCoroutine(LockFire());
            if (RechargeCoroutine == null)
                RechargeCoroutine = StartCoroutine(RechargeRoutine());
            return true;
        }

        public IEnumerator HideWeapon()
        {
            WLocked = true;
            WeaponAnimator.SetTrigger("Hide");
            yield return new WaitForSeconds(HideDuration);
            CurrentAnimRoutine = null;
            if (RechargeCoroutine != null)
                StopCoroutine(RechargeCoroutine);
            gameObject.SetActive(false);
        }
    }
}