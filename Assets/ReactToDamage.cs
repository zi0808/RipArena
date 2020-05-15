using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace arena
{
    namespace combat
    {
        /// <summary>
        /// IHasHealth - 무기에 대해 반응하고, 내구도가 있는 인터페이스
        /// </summary>
        public interface IHasHealth : IDamageReaction
        {
            void Initialize(int max_health);
            void SetHealth(int health);
            void Heal(int amount);
            void OnDeath(IHasHealth object_info);
            void OnHealthChange(IHasHealth object_info);
            void Kill();
        }
        /// <summary>
        /// IJointCharacter - 
        /// </summary>
        public interface IJointCharacter : IHasHealth
        {
            void OnJointDeath(IHasHealth limb);
        }


        public enum ReactToDamageInterfaceType
        {
            Basic,
            HasHealth,
            JointCharacter,
        }

        public delegate void OnObjectDeath(IHasHealth dead_object);
        public delegate void OnObjectHealthChange(IHasHealth target_object);

        /// <summary>
        /// 무기 파괴력에 대한 반응이 있는 Mono 클래스
        /// IHasHealth, IJointCharacter 인터페이스를 가질 수 있음.
        /// </summary>
        public class ReactToDamage : MonoBehaviour, IHasHealth
        {
            public int MaxHealth;
            [HideInInspector]
            public int CurrentHealth;
            [HideInInspector]
            public bool Dead;

            public event OnObjectDeath ev_obj_death;
            public event OnObjectHealthChange ev_obj_hchange;

            public virtual void Damage(DamageParam param)
            {
                CurrentHealth -= (int)param.damage;
                if (CurrentHealth < 0)
                    OnDeath(this);
                OnHealthChange(this);
            }

            public virtual void Heal(int amount)
            {
                // Skip If Not Needed
                CurrentHealth += amount;
                OnHealthChange(this);
            }

            public virtual void Initialize(int max_health)
            {
                CurrentHealth = max_health;
            }

            public void Kill()
            {
                CurrentHealth = 0;
                Dead = true;
                OnDeath(this);
            }

            public virtual void OnDeath(IHasHealth object_info)
            {
                ev_obj_death?.Invoke(this);
            }

            public virtual void OnHealthChange(IHasHealth object_info)
            {
                ev_obj_hchange?.Invoke(this);
            }

            public virtual void SetHealth(int health)
            {
                CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
                OnHealthChange(this);
            }

            // Start is called before the first frame update
            public virtual void Start()
            {
                Initialize(MaxHealth);
            }

            // Update is called once per frame
            public virtual void Update()
            {

            }
        }
    }
}