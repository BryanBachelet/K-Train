using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
{
    public class TankAttackComponent : MonoBehaviour
    {
        [Header("Attack Parameter")]
        public float radiusOfAttack = 4;
        public float damage = 5.0f;
        public float timeOfCharge = 1.0f;
        public float timeofRecuperation = 0.5f;

        public UnityEngine.VFX.VisualEffect vfxRangeAttack;

        private float m_timerOfCharge;
        private float m_timerOfRecuperation;

        public Transform m_monsterBodyTransform;
        private Transform m_targetTransform;
        public Animator m_tankAnimator;
        private NpcHealthComponent m_npcHealthComponent;

        private Animator m_entityAnimator;

        public void Start()
        {
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_entityAnimator = m_npcHealthComponent.m_entityAnimator;
            m_npcHealthComponent.destroyEvent += OnDeath;
            m_targetTransform = m_npcHealthComponent.targetData.target;
        }

        public void Update()
        {
            if (m_npcHealthComponent.npcState == NpcState.MOVE && Vector3.Distance(m_targetTransform.position, m_monsterBodyTransform.position) < radiusOfAttack)
            {
                m_npcHealthComponent.npcState = NpcState.PREP_ATTACK;

            }

            if (m_npcHealthComponent.npcState == NpcState.PREP_ATTACK)
            {
                if (m_timerOfCharge > timeOfCharge)
                {
                    m_npcHealthComponent.npcState = NpcState.ATTACK;
                    m_timerOfCharge = 0;
                    vfxRangeAttack.SendEvent("ActiveArea");
                    AttackTank();
                   
                }
                else
                {
                    m_timerOfCharge += Time.deltaTime;
                }

            }

            if (m_npcHealthComponent.npcState == NpcState.RECUPERATION)
            {
                if (m_timerOfRecuperation > timeofRecuperation)
                {
                    m_npcHealthComponent.npcState = NpcState.MOVE;
                    timeofRecuperation = 0;
                    m_entityAnimator.SetBool("Attack", false);
                    vfxRangeAttack.SendEvent("UnActiveArea");
                }
                else
                {
                    m_timerOfRecuperation += Time.deltaTime;
                }
            }

        }

        public void AttackTank()
        {
            m_entityAnimator.SetBool("Attack", true);

            if (Vector3.Distance(m_targetTransform.position, m_monsterBodyTransform.position) < radiusOfAttack)
            {
                if (m_targetTransform.tag == "Player")
                {
                    m_targetTransform.GetComponent<health_Player>().GetDamageLeger(damage, transform.position);
                   
                }
                if (m_targetTransform.tag == "Altar")
                {
                    m_targetTransform.GetComponent<ObjectHealthSystem>().TakeDamage((int)damage);

                }
            }
            m_npcHealthComponent.npcState = NpcState.RECUPERATION;


        }

        public void LateUpdate()
        {
            if (m_npcHealthComponent.m_hasChangeTarget)
            {
                m_targetTransform = m_npcHealthComponent.targetData.target;
                m_npcHealthComponent.m_hasChangeTarget = false;
            }
        }
        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(m_monsterBodyTransform.position, radiusOfAttack);
        }

        public void OnDeath( Vector3 direction,float power)
        {
            this.enabled = false;
        }



    }
}
