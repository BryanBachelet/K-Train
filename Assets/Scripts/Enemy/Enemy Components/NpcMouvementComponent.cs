using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{


    public class NpcMouvementComponent : MonoBehaviour
    {


        [Header("Movement Parameters")]
        public float speed = 5;
        public float speedThreshold = 1;

        private float m_baseSpeed;
        private float m_currentSpeed;

        [Header("Slope Parameters")]
        public bool isAffectedBySlope = false;
        [Range(1.0f, 3.00f)] public float maxVariation = 1.5f;
        [Range(0.001f, 1.00f)] public float minVariation = 0.5f;
        public float m_maxAngle = 60.0f;

        [SerializeField] private float m_groundDistance = 3.0f;
        [SerializeField] private LayerMask m_groundLayerMask;



        [Header("Navmesh Parameters")]
        public float timeBetweenNavRefresh;
        public float minDistanceToFullyActive;
        [SerializeField] private float m_distanceBeforeRepositionning = 400;

        private float m_timerBetweenNavRefresh = 0;

        private bool m_isPauseActive;
        private float m_directionMinDot = 0.45f;

        private Rigidbody m_rigidbody;
        private NavMeshAgent m_navMeshAgent;
        private NpcHealthComponent m_npcHealthComponent;
        public EnemyManager enemiesManager;

        public TargetData targetData;

        public void Start()
        {
            InitComponent();
            SetTarget(m_npcHealthComponent.targetData);
            m_npcHealthComponent.destroyEvent += OnDeath;
            m_baseSpeed = Random.Range(speed - speedThreshold, speed + speedThreshold);
            m_navMeshAgent.speed = m_baseSpeed;
            m_navMeshAgent.destination = (targetData.target.position);
        }



        public void InitComponent()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_npcHealthComponent = GetComponent<NpcHealthComponent>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetTarget(TargetData target)
        {
            targetData = target;

        }

        public void Update()
        {
            if (m_npcHealthComponent.npcState == NpcState.PAUSE)
            {
                if (!m_isPauseActive)
                {
                    SetupPause();
                    m_isPauseActive = true;
                }
                return;
            }
            m_isPauseActive = false;

            if (m_npcHealthComponent.npcState == NpcState.MOVE)
            {
                if (m_navMeshAgent.isActiveAndEnabled && m_navMeshAgent.isStopped) m_navMeshAgent.isStopped = false;

                if (isAffectedBySlope)
                {
                    m_navMeshAgent.speed = CalculateSlopeSpeed();
                }

                if (targetData.isMoving) Move();
            }
            else
            {

            }
        }

        public void SetupPause()
        {
            if (m_navMeshAgent.isActiveAndEnabled) m_navMeshAgent.isStopped = !m_navMeshAgent.isStopped;
        }

        public float CalculateSlopeSpeed()
        {
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(transform.position, -Vector3.up, out hit, m_groundDistance, m_groundLayerMask);
            Vector3 direction = Vector3.Cross(transform.right, hit.normal);
            float currentSlope = GetSlopeAngle(direction);

            float ratio = maxVariation;
            if (currentSlope < 0) ratio = -minVariation;
            float ratioSlope = (currentSlope / m_maxAngle);
            float ratioSpeed = Mathf.Lerp(0, ratio, Mathf.Abs(ratioSlope));
            float speed = m_baseSpeed + m_baseSpeed * ratioSpeed;
            return speed;
        }

        private float GetSlopeAngle(Vector3 direction)
        {
            Quaternion rotTest = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            return Vector3.SignedAngle(rotTest * Vector3.forward, direction, transform.right);
        }


        public void Move()
        {
            Vector3 directionToDestination = m_navMeshAgent.destination - transform.position; 
            Vector3 directionToTarget = targetData.target.position - transform.position;
            float dot = Vector3.Dot(directionToDestination.normalized, directionToTarget.normalized);
            float distancePos = Vector3.Distance(transform.position, targetData.target.position);

            // Repositionning enemi when to far 
            if (distancePos > m_distanceBeforeRepositionning)
            {

                Debug.Log("Distance before repo = " + m_distanceBeforeRepositionning);
                if (enemiesManager.ReplaceFarEnemy(this.gameObject))
                {

                    m_navMeshAgent.destination = targetData.target.position;
                    m_navMeshAgent.nextPosition = transform.position;
                    return;
                }


            }
            if(distancePos > minDistanceToFullyActive && dot > m_directionMinDot)
            {
                return;
            }

            m_navMeshAgent.destination = targetData.target.position;
        }

        public void OnDeath(Vector3 direction, float power)
        {

            m_navMeshAgent.enabled = false;

            if (!m_npcHealthComponent.hasDeathAnimation)
            {

                //m_rigidbody.isKinematic = false;
                m_rigidbody.AddForce((direction.normalized + Vector3.up).normalized * power, ForceMode.Impulse);
            }

            this.enabled = false;
        }
    }
}
