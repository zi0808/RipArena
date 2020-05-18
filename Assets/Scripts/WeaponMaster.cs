using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace arena.combat
{
    public class WeaponMaster : MonoBehaviour
    {
        public bool FullAuto = false;
        public string WeaponName = "";
        public int MaxAmmo = 2;
        public float RechargeDelay = 1;
        public float Kick = 5;
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
        Coroutine AutoFireRoutine;
        static FPControl fp_cont;
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
            if (fp_cont == null)
                fp_cont = FindObjectOfType<FPControl>();
        }
        private void OnEnable()
        {
            if (WeaponAnimator == null)
                WeaponAnimator = GetComponent<Animator>();
            WeaponAnimator.enabled = true;
            WLocked = true;
            CurrentAnimRoutine = StartCoroutine(WaitForPullOut());
            if (UIGameUI.Instance != null)
            {
                UIGameUI.Instance.AmmoCounterInit(MaxAmmo, WeaponName);
                UIGameUI.Instance.AmmoCounterUpdate(_current_ammo);
            }
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
        public void StopFiring()
        {
            StopCoroutine(AutoFireRoutine);
            AutoFireRoutine = null;
        }
        public void StartFiring()
        {
            AutoFireRoutine = StartCoroutine(AutoFire());
        }
        IEnumerator AutoFire()
        {
            while (CurrentAmmo > 0)
            {
                ShootWeaponSingle();
                yield return new WaitForSeconds(DelayPerShot);
            }
        }

        public bool ShootWeaponSingle()
        {
            if (WLocked || CurrentAmmo <= 0)
                return false;
            fp_cont.FOVKick(Kick);
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
            while (WLocked)
                yield return new WaitForEndOfFrame();

            WLocked = true;
            WeaponAnimator.SetTrigger("Hide");
            yield return new WaitForSeconds(HideDuration);
            CurrentAnimRoutine = null;
            if (RechargeCoroutine != null)
                StopCoroutine(RechargeCoroutine);
            WeaponAnimator.enabled = false;
            gameObject.SetActive(false);
        }
    }
}