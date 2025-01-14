using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class DayCyclecontroller : MonoBehaviour
{
    [Range(0, 24)]
    [SerializeField] private float m_timeOfDay;
    static public float staticTimeOfTheDay;
    [SerializeField] private Light m_sun;
    [SerializeField] private Light m_moon;
    [SerializeField] private float m_SettingDurationDay = 10; // Correspond au nombre de minute IRL de la dur�e d'une journ�e in-game, de base 10 minutes
    [SerializeField] public float m_orbitSpeed = 1.0f; // Correponds � la vitesse d'�coulement du temps in-game. 1 reviens � avoir une journ�e de 24 secondes IRL
    [SerializeField] public RectTransform m_ClockNeedle;
    [SerializeField] private GlobalSoundManager m_GSM;
    [SerializeField] static public float durationDay;
    [SerializeField] static public float durationNight;
    [SerializeField] private Volume m_LocalNightVolume;
    [SerializeField] private Enemies.EnemyManager m_EnemyManager;
    [SerializeField] private float time;
    [SerializeField] private Text m_DayPhases;
    [SerializeField] private Image m_daySlider;
    private bool isNight = false;
    public float timescale;

    [SerializeField] float[] tempsChaquePhase;
    [SerializeField] string[] nomChaquePhase;
    [SerializeField] float[] heureChaquePhase;
    [SerializeField] string[] nomHeureChaquePhase;
    [SerializeField] int currentPhase = 0;
    [SerializeField] float m_TimeTransitionLastPhase;
    [SerializeField] float m_TimeProchainePhase;

    private string dayprogress;
    private string phaseprogress;

    // Night Event
    public delegate void NightBeginning();
    public event NightBeginning nightStartEvent;
    // Day Event
    public delegate void DayBeginning();
    public event DayBeginning dayStartEvent;


    // Start is called before the first frame update
    void Start()
    {
        m_orbitSpeed = 24 / (m_SettingDurationDay * 60); //on divise 24 (nombre d'heure) par le nombre de secondes qui vont s'�couler IRL.  On multiplie le nombre de minutes r�gl�e dans l'inspector par 60 pour le convertire en seconde.
        durationNight = m_SettingDurationDay / 3;
        durationDay = durationNight * 2;
        Time.timeScale = timescale;
        m_TimeProchainePhase = time + tempsChaquePhase[currentPhase];
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        CheckPhase(m_timeOfDay);
        //if(isNight)
        //{
        //    m_timeOfDay += Time.deltaTime * m_orbitSpeed * 2;
        //}
        //else
        //{
        //    m_timeOfDay += Time.deltaTime * m_orbitSpeed;
        //}
        if(m_timeOfDay > 22 || m_timeOfDay < 4 && isNight)
        {
           m_GSM.UpdateParameter(1, "DayOrNight");
        }
       else
        {
           m_GSM.UpdateParameter(0, "DayOrNight");
        }

        staticTimeOfTheDay = m_timeOfDay;
        UpdateTime();   
    }

    private void OnValidate()
    {
        UpdateTime();

    }
    private void UpdateTime()
    {
        float alpha = m_timeOfDay / 24.0f;
        float sunRotation = Mathf.Lerp(-90, 270, alpha);
        float moonRotation = sunRotation - 180;
        float clockRotation = Mathf.Lerp(0, -360, alpha);

        m_ClockNeedle.rotation = Quaternion.Euler(0, 0, clockRotation + 180);
        m_sun.transform.rotation = Quaternion.Euler(sunRotation, -150.0f, 0);
        m_moon.transform.rotation = Quaternion.Euler(moonRotation, -150.0f, 0);
        if(m_timeOfDay > 5.12f && m_timeOfDay < 18.5f)
        {
            if (m_moon.isActiveAndEnabled)
            {
                m_moon.enabled = false;
            }
        }
        else
        {
            if (!m_moon.isActiveAndEnabled)
            {
                m_moon.enabled = true;
            }
        }
        CheckingNightDayTransition();
    }

    private void CheckingNightDayTransition()
    {
        if(isNight)
        {
            if(m_moon.transform.rotation.eulerAngles.x > 180)
            {
                StartDay();
            }
        }
        else
        {
            if (m_sun.transform.rotation.eulerAngles.x > 180)
            {
                StartNight();
            }
        }
    }

    private void StartDay()
    {
        m_sun.gameObject.SetActive(true);
        isNight = false;
        dayStartEvent.Invoke();
        m_sun.shadows = LightShadows.Soft;
        m_moon.shadows = LightShadows.None;
        //m_LocalNightVolume.enabled = false;
        m_GSM.UpdateParameter(0, "DayOrNight");
        m_moon.gameObject.SetActive(false);
    }

    private void StartNight()
    {
        m_moon.gameObject.SetActive(true);
        isNight = true;
        nightStartEvent.Invoke();
        //m_LocalNightVolume.enabled = true;
        m_sun.shadows = LightShadows.None;
        m_moon.shadows = LightShadows.Soft;
        m_sun.gameObject.SetActive(false);
    }

    public void CheckPhase(float hour)
    {
        UpdatePhaseInfo();
        m_DayPhases.text = dayprogress + " - " + phaseprogress;

    }

    public void UpdatePhaseInfo()
    {
        if(time > m_TimeProchainePhase)
        {
            currentPhase += 1;
            if(currentPhase > tempsChaquePhase.Length)
            {
                currentPhase = 0;
            }
            m_TimeProchainePhase = time + tempsChaquePhase[currentPhase];
            m_TimeTransitionLastPhase = time;
            phaseprogress = nomChaquePhase[currentPhase];
            dayprogress = nomHeureChaquePhase[currentPhase];
            m_DayPhases.text = dayprogress + " - " + phaseprogress;
            
        }
        float sliderValue =1 - ((m_TimeProchainePhase - time) / tempsChaquePhase[currentPhase]);
        if (sliderValue <= 1 && sliderValue >= 0)
        {
            m_daySlider.fillAmount = sliderValue;
        }
        m_timeOfDay = Mathf.Lerp(heureChaquePhase[currentPhase], heureChaquePhase[currentPhase + 1], sliderValue);
        if (m_timeOfDay >= 24)
        {
            m_timeOfDay = 0;
        }
        else if (m_timeOfDay > 5.5f && m_timeOfDay < 6f)
        {
            m_timeOfDay = 6.1f;
        }
        else if (m_timeOfDay > 17.9f && m_timeOfDay < 18.5f)
        {
            m_timeOfDay = 18.5f;
        }
    
        if(currentPhase == 1 || currentPhase == 4 || currentPhase == 7) { m_EnemyManager.ChangeSpawningPhase(true); }
        else { m_EnemyManager.ChangeSpawningPhase(false); }


    }
}
