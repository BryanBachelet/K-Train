using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using GuerhoubaGames.GameEnum;
using GuerhoubaTools.Gameplay;
public struct AttackDamageInfo
{
    public string attackName;
    public int damage;
    public Vector3 position;
    public bool bIsHeavy;
}

public class HealthPlayerComponent : MonoBehaviour
{


    [Header("Heath Parameters")]
    [SerializeField] private float m_MaxHealthQuantity;

    private float[] m_CurrentQuarterMinHealth;
    private int m_QuarterNumber = 4;
    private float m_QuarterHealthQuantity;


    [SerializeField] private float m_invulerableLightTime;
    [SerializeField] private float m_invulerableHeavyTime;
    private bool m_isLightInvulnerable = false;
    private bool m_isHeavyInvulnerable = false;

    private ClockTimer m_invulerableLightClock = new ClockTimer();
    private ClockTimer m_invulerableHeavyClock = new ClockTimer();


    [Header("Heath Info")]
    [SerializeField] private float m_CurrentHealth;
    [SerializeField] private int m_CurrentQuarter;

    [Header("Death Info")]
    [SerializeField] private bool activeDeath = false;
    private bool isEndMenuActivate = false;
    public GameObject m_gameOverMenu;

    [Header("UI Object")]
    public GuerhoubaGames.UI.UI_HealthPlayer uiHealthPlayer;

    [Header("Damage Feedback Parameters")]
    [SerializeField] private AnimationCurve m_SlowdownEffectCurve;
    [SerializeField] private AnimationCurve m_SlowdownEffectCurveLastQuarter;
    private AnimationCurve m_currentAnimationCurveToUse;
    [SerializeField] private float m_DurationSlowdownEffect = 1;
    [SerializeField] private bool m_isFeedbackHitActive = false;
    private float tempsEcouleSlowEffect = 0;
    private bool slowDownActive = false;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    public Volume volume;
    private Camera m_cameraUsed;
    [SerializeField] private AnimationCurve m_fieldOfViewEdit;
    [SerializeField] private AnimationCurve m_ColorAdjustementSaturation;


    [Header("Object Parameters")]
    public GameState gameStateObject;
    private Character.CharacterMouvement m_characterMouvement;



    public AnimationCurve evolutionVignetteOverTime;
    private float m_timeLastHit;
    public float timeVignetteFeedbackActive = 0.25f;

    public System.Action<AttackDamageInfo> OnDamage;

    public delegate void OnContact(Vector3 position, EntitiesTrigger tag, GameObject objectHit, GameElement element);
    public event OnContact OnContactEvent = delegate { };


    // Start is called before the first frame update
    void Start()
    {
        InitializedHealthData();
        m_characterMouvement = GetComponent<Character.CharacterMouvement>();
        m_cameraUsed = Camera.main;
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);

    }

    // Update is called once per frame
    void Update()
    {
        if (activeDeath && m_CurrentHealth <= 0 && !isEndMenuActivate)
        {
            GuerhoubaGames.SaveData.GameData.UpdateFarestRoom(TerrainGenerator.roomGeneration_Static);
            GameState.LaunchEndMenu();
            gameStateObject.HideGlobalUI();

            isEndMenuActivate = true;
            return;
        }

        if (m_invulerableLightClock.UpdateTimer())
        {
            m_invulerableLightClock.DeactivateClock();
            m_isLightInvulnerable = false;
        }
        if (m_invulerableHeavyClock.UpdateTimer())
        {
            m_invulerableHeavyClock.DeactivateClock();
            m_isHeavyInvulnerable = false;
        }

        if (m_isFeedbackHitActive)
        {
            if (Time.time - m_timeLastHit < timeVignetteFeedbackActive)
            {
                vignette.opacity.value = evolutionVignetteOverTime.Evaluate(Time.time - m_timeLastHit);
            }
            else
            {
                vignette.opacity.value = 0;
                m_isFeedbackHitActive = false;
            }
        }
        if (slowDownActive)
        {
            float progress = tempsEcouleSlowEffect / m_DurationSlowdownEffect;
            float newTimeScale = m_currentAnimationCurveToUse.Evaluate(progress);
            m_cameraUsed.fieldOfView = m_fieldOfViewEdit.Evaluate(progress);
            colorAdjustments.saturation.value = m_ColorAdjustementSaturation.Evaluate(progress);
            Time.timeScale = newTimeScale;
            tempsEcouleSlowEffect += (Time.deltaTime * (1 + (1 - newTimeScale)));
            if (tempsEcouleSlowEffect >= m_DurationSlowdownEffect)
            {
                newTimeScale = 1;
                tempsEcouleSlowEffect = 0;
                Time.timeScale = 1;
                m_cameraUsed.fieldOfView = 65;
                colorAdjustments.saturation.value = 3.8f;
                slowDownActive = false;
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SlowDownEffect", newTimeScale);
        }


    }

    public void ApplyDamage(AttackDamageInfo attackDamageInfo)
    {
        if (m_isHeavyInvulnerable) return;
        if (!attackDamageInfo.bIsHeavy && m_isLightInvulnerable) return;

        GlobalSoundManager.PlayOneShot(29, transform.position);
        m_isFeedbackHitActive = true;
        StartInvulnerability(attackDamageInfo.bIsHeavy);

        vignette.intensity.value = 0.35f;
        if (m_CurrentQuarter - 1 >= 0 && m_CurrentHealth - attackDamageInfo.damage < m_CurrentQuarterMinHealth[m_CurrentQuarter - 1])
        {
            ActiveSlowEffect(1.5f, m_CurrentQuarter);
            m_CurrentQuarter -= 1;
            m_CurrentHealth = m_CurrentQuarterMinHealth[m_CurrentQuarter];

        }
        else
        {
            m_CurrentHealth -= attackDamageInfo.damage;
        }

        if (OnDamage != null) OnDamage.Invoke(attackDamageInfo);

        uiHealthPlayer.UpdateLifeBar(m_CurrentHealth / m_MaxHealthQuantity, 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter));
        if (attackDamageInfo.bIsHeavy)
        {
            m_characterMouvement.SetKnockback(attackDamageInfo.position, 100);
        }
        else
        {
            m_characterMouvement.SetKnockback(attackDamageInfo.position);
        }
        if (m_CurrentHealth <= 0)
        {
            activeDeath = true;
        }


    }


    private void StartInvulnerability(bool isHeavy)
    {
        if (!isHeavy)
        {
            m_isLightInvulnerable = true;
            m_invulerableLightClock.SetTimerDuration(m_invulerableLightTime);
            m_invulerableLightClock.ActiaveClock();

        }
        else

        {
            m_isHeavyInvulnerable = true;
            m_invulerableHeavyClock.SetTimerDuration(m_invulerableHeavyTime);
            m_invulerableHeavyClock.ActiaveClock();
        }
    }


    public void InitializedHealthData()
    {

        //m_CurrentHealth = m_MaxHealthQuantity;
        m_QuarterHealthQuantity = m_MaxHealthQuantity / m_QuarterNumber;
        m_CurrentQuarter = (int)Mathf.Ceil(m_CurrentHealth / m_MaxHealthQuantity * m_QuarterNumber);
        m_CurrentQuarterMinHealth = new float[m_QuarterNumber];
        for (int i = 0; i < m_CurrentQuarterMinHealth.Length; i++)
        {
            m_CurrentQuarterMinHealth[i] = m_QuarterHealthQuantity * i;
        }


        uiHealthPlayer.UpdateLifeBar(m_CurrentHealth / m_MaxHealthQuantity, 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter));
        m_CurrentHealth = m_MaxHealthQuantity; ;
        m_isLightInvulnerable = false;
    }

    public void AugmenteMaxHealth(int quantity)
    {
        m_MaxHealthQuantity += quantity;
        m_CurrentHealth += quantity;
        InitializedHealthData();
    }

    public void RestoreQuarter()
    {
        if (m_QuarterHealthQuantity == 0 || m_QuarterNumber == 0) return;

        int indexQuarter = (int)(m_CurrentHealth) / (int)(m_QuarterHealthQuantity);
        m_CurrentHealth = Mathf.Clamp((indexQuarter + 1) * m_QuarterHealthQuantity, 0, m_MaxHealthQuantity);
        m_CurrentQuarter = Mathf.Clamp((indexQuarter + 1), 0, 4);
        uiHealthPlayer.UpdateLifeBar(m_CurrentHealth / m_MaxHealthQuantity, 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter));
    }



    public void RestoreFullLife()
    {

        m_CurrentHealth = m_MaxHealthQuantity;
        m_CurrentQuarter = 4;
        uiHealthPlayer.UpdateLifeBar(m_CurrentHealth / m_MaxHealthQuantity, 1 / m_QuarterNumber * (m_QuarterNumber - m_CurrentQuarter));
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            OnContactEvent(collision.transform.position, EntitiesTrigger.Enemies, collision.gameObject, GameElement.NONE);
        }
    }

    public void ActiveSlowEffect(float time, int quarter)
    {
        slowDownActive = true;
        m_DurationSlowdownEffect = time;
        tempsEcouleSlowEffect = 0;
        GlobalSoundManager.PlayOneShot(50, transform.position);
        if (quarter > 1)
        {
            m_currentAnimationCurveToUse = m_SlowdownEffectCurve;
        }
        else
        {
            m_DurationSlowdownEffect += 1;
            m_currentAnimationCurveToUse = m_SlowdownEffectCurveLastQuarter;
        }
    }

}
