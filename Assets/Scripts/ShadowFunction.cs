using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class ShadowFunction : MonoBehaviour
{
    public bool onShadow = false;
    public bool outShadow = false;

    static public bool onShadowStatic = false;
    static public bool outShadowStatic = false;

    [SerializeField] private float m_TimeOnShadow = 0;
    [SerializeField] private float m_TimeBeforeDetection = 2;
    static public bool onShadowSpawnStatic = false;
    private bool onShadowSpawn = false;

    [SerializeField] private float m_TimeOutShadow = 0;
    [SerializeField] private float m_TimeBeforeStopDetection = 2;
    static public bool outShadowSpawnStatic = false;
    private bool outShadowSpawn = false;

    public Animator detectionAnimator;
    public UnityEngine.VFX.VisualEffect vfxDetection;

    public EventReference Globalsound;
    public EventInstance shadowDetection;

    public EventReference activationShadowDetection_Attribution;

    public Enemies.EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        shadowDetection = RuntimeManager.CreateInstance(Globalsound);
    }
    //ShadowDetection
    // Update is called once per frame
    void Update()
    {
        if (onShadow)
        {
            if (m_TimeOnShadow < m_TimeBeforeDetection)
            {
                m_TimeOnShadow += Time.deltaTime;
                if (vfxDetection.HasInt("Rate")) vfxDetection.SetInt("Rate", (int)(m_TimeOnShadow / m_TimeOnShadow * 5));
            }
            else
            {
                if (!onShadowSpawn)
                {
                    m_TimeOutShadow = 0;
                    StopDetectionSoundFeedback();
                    if (vfxDetection.HasFloat("Rate")) vfxDetection.SetInt("Rate", 0);
                    RuntimeManager.PlayOneShot(activationShadowDetection_Attribution, transform.position);
                    GlobalSoundManager.PlayOneShot(40, transform.position);
                    onShadowSpawn = true;
                    onShadowSpawnStatic = true;
                    outShadowSpawn = false;
                    outShadowSpawnStatic = false;
                    enemyManager.ChangeSpawningPhase(true);
                }

            }

        }
        else
        {
            if (m_TimeOutShadow < m_TimeBeforeStopDetection)
            {
              if(vfxDetection.HasInt("Rate"))  vfxDetection.SetInt("Rate", 0);
                m_TimeOutShadow += Time.deltaTime;
            }
            else
            {
                if (!outShadowSpawn && !DayCyclecontroller.isNightState)
                {
                    m_TimeOnShadow = 0;
                    StopDetectionSoundFeedback();
                    outShadowSpawn = true;
                    outShadowSpawnStatic = true;
                    onShadowSpawn = false;
                    onShadowSpawnStatic = false;
                    enemyManager.ChangeSpawningPhase(false);
                }

            }
        }

    }

    public void OnShadow()
    {
        StartDetectionSoundFeedback();
        onShadow = true;
        outShadow = false;

        //onShadowStatic = onShadow;
        //outShadowStatic = outShadow;
        Debug.Log("On Shadow enter !");
    }

    public void OutShadow()
    {
        StopDetectionSoundFeedback();
        onShadow = false;
        outShadow = true;
        //onShadowStatic = onShadow;
        //outShadowStatic = outShadow;
        Debug.Log("On Shadow out !");
    }

    public void OnEnterShadow()
    {
        m_TimeOnShadow = 0;
    }
    public void OnExitShadow()
    {
        m_TimeOutShadow = 0;
    }
    public void StartDetectionSoundFeedback()
    {
        shadowDetection.start();
    }

    public void StopDetectionSoundFeedback()
    {
        shadowDetection.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}