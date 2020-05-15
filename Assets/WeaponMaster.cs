using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace arena.combat
{
    public class WeaponMaster : MonoBehaviour
    {
        public AnimatorPlaySound SoundPlayer;

        IShootable Shooterinterface;


        public void ShootWeaponSingle()
        {
            Shooterinterface = GetComponent<IShootable>();

            Shooterinterface?.Shoot();
            SoundPlayer?.PlaySound();
        }
    }
}