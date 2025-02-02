using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarBehaviorComponent : MonoBehaviour
{
    [Header("Event Parameters")]
    [Range(0, 3)]
    [SerializeField] int eventElementType = 0;
    [SerializeField] private Color[] colorEvent;
    [SerializeField] private Material[] materialEvent;
    [ColorUsage(showAlpha:true, hdr:true)]
    [SerializeField] private Color[] colorEventTab;
    [SerializeField] private float m_TimeInvulnerability;
    [SerializeField] private float m_MaxHealth;
    [SerializeField] private int m_CurrentHealth;
    [SerializeField] private float m_MaxKillEnemys;
    [SerializeField] private int m_CurrentKillCount;
    public float radiusEventActivePlayer = 300;
    public float radiusEjection;
    public int rangeEvent = 100;

    [Header("Reward Parameters")]
    [SerializeField] private int XpQuantity = 100;
    [SerializeField] private GameObject[] xpObject;
    [SerializeField] private float m_ImpusleForceXp;


    private bool m_hasEventActivate = true;
    private bool m_isEventOccuring;


    private Text displayTextDescription1;
    private Text displayTextDescription2;
    private Text eventTextName;

    [Header("Event UI Parameters")]
    public GameObject displayEventDetail;
    public GameObject displayArrowEvent;
    private GameObject ownDisplayEventDetail;
    private GameObject ownArrowDisplayEventDetail;

    public Animator m_myAnimator;
    public EventDisplay displayAnimator;

    private RectTransform canvasPrincipal;
    private Enemies.EnemyManager m_EnemyManagerScript;

    static int altarCount = 0;
    private int ownNumber = 0;
    private Color myColor;
    private Transform m_playerTransform;


    public string txt_EventName;
    int resetNumber = 0;

    public MeshRenderer[] altarAllMesh;

    public Material socleMaterial;
    [HideInInspector] public bool isAltarDestroy = false;

    private ObjectHealthSystem m_objectHealthSystem;

    // Start is called before the first frame update
    void Start()
    {
      
        eventElementType = Random.Range(0, 4);
        GetComponentInChildren<Light>().color = colorEvent[eventElementType];
        ownNumber = altarCount;
        altarCount++;
        InitComponent();
        m_CurrentHealth = (int)m_MaxHealth;

        altarAllMesh[0].material.shader = Shader.Find("Intensity");
        altarAllMesh[0].material.shader = Shader.Find("Color");
        altarAllMesh[0].material = materialEvent[eventElementType];
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
           
            altarAllMesh[i].material.SetColor("_SelfLitColor", colorEventTab[eventElementType]);
        }
        //DisableColor();
        m_playerTransform = m_EnemyManagerScript.m_playerTranform;
    }

    private void InitComponent()
    {
        m_objectHealthSystem = GetComponent<ObjectHealthSystem>();
        m_EnemyManagerScript = GameObject.Find("Enemy Manager").GetComponent<Enemies.EnemyManager>();
        //canvasPrincipal = GameObject.Find("MainUI_EventDisplayHolder").GetComponent<RectTransform>();
        //ownDisplayEventDetail = Instantiate(displayEventDetail, canvasPrincipal.position, canvasPrincipal.rotation, canvasPrincipal);
        //ownArrowDisplayEventDetail = Instantiate(displayArrowEvent, canvasPrincipal.position, canvasPrincipal.rotation, canvasPrincipal.parent);
        //ownArrowDisplayEventDetail.GetComponent<UI_ArrowPointingEvent>().refGo = this.gameObject;
        //displayAnimator = ownDisplayEventDetail.GetComponent<EventDisplay>();
        //m_myAnimator = this.GetComponentInChildren<Animator>();
        //displayTextDescription1 = displayAnimator.m_textDescription1;
        //displayTextDescription2 = displayAnimator.m_textDescription2;
        //eventTextName = displayAnimator.m_textEventName;

    }
    // Update is called once per frame
    void Update()
    {
        
     
        float ennemyTokill = m_MaxKillEnemys * (1 + 0.1f * (resetNumber + 1));

        if (ennemyTokill <= m_CurrentKillCount && m_objectHealthSystem.IsEventActive())
        {
            m_myAnimator.SetBool("ActiveEvent", false);
            GiveRewardXp();
            m_objectHealthSystem.ChangeState(EventObjectState.Deactive);

            //displayAnimator.InvertDisplayStatus(2);
        }
        else
        {
            //displayTextDescription1.text = m_CurrentHealth + "/" + m_MaxHealth;
            //displayTextDescription2.text = (m_MaxKillEnemys * (1 + 0.1f * (resetNumber + 1))) - m_CurrentKillCount + " Remaining";
        }


        if(m_objectHealthSystem.IsEventActive() && Vector3.Distance(m_playerTransform.position,transform.position)> radiusEventActivePlayer)
        {
            DestroyAltar();
        }

        if (m_objectHealthSystem.eventState == EventObjectState.Death)
        {
            DestroyAltar();
        }



    }

  
    private void DestroyAltar()
    {
        m_objectHealthSystem.ChangeState(EventObjectState.Deactive);
        m_myAnimator.SetBool("ActiveEvent", false);
        m_hasEventActivate = true;
        m_isEventOccuring = false;
        isAltarDestroy = true;
        m_EnemyManagerScript.RemoveTarget(transform);
        m_EnemyManagerScript.RemoveAltar(transform);
        Debug.Log("Destroy event");
    }


    // Need to set active
    public void ActiveEvent()
    {
        if (!m_objectHealthSystem.IsEventActive())
        {
            m_EnemyManagerScript.AddTarget(this.transform);
            m_EnemyManagerScript.AddAltar(transform);
            for(int i = 0; i < altarAllMesh.Length; i++)
            {
                altarAllMesh[i].material.SetFloat("_SelfLitIntensity", 0.15f);
            }
            m_myAnimator.SetBool("ActiveEvent", true);
            GlobalSoundManager.PlayOneShot(13, transform.position);
            m_objectHealthSystem.ChangeState(EventObjectState.Active);
            m_hasEventActivate = false;
            m_isEventOccuring = true;
            Debug.Log("Event has been Activate");

            //this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            //this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            //this.transform.GetChild(0).gameObject.SetActive(true);
            //Enemies.EnemyManager.EnemyTargetPlayer = false;
            //displayAnimator.InvertDisplayStatus(1);
            //eventTextName.text = txt_EventName + " (+" + resetNumber + ")";
            //ActiveColor();
        }
        else
        {
            Debug.Log("Cette objet [" + this.name + "] ne peut pas �tre activ�");
        }
    }

    public void IncreaseKillCount()
    {
        m_CurrentKillCount++;
    }

    public void GiveRewardXp()
    {

        m_EnemyManagerScript.RemoveTarget(transform);
        m_EnemyManagerScript.RemoveAltar(transform);
        m_isEventOccuring = false;
        m_myAnimator.SetBool("IsDone", true);
        for (int i = 0; i < altarAllMesh.Length; i++)
        {
            altarAllMesh[i].material.SetFloat("_SelfLitIntensity", 0.32f);
        }
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        GlobalSoundManager.PlayOneShot(14, transform.position);
        for (int i = 0; i < XpQuantity + 25 * resetNumber; i++)
        {
            Vector2 rndVariant = new Vector2((float)Random.Range(-2, 2), (float)Random.Range(-2, 2));
            GameObject xpGenerated;
            xpGenerated = Instantiate(xpObject[0], transform.position + new Vector3(rndVariant.x * radiusEjection, 0, rndVariant.y * radiusEjection), Quaternion.identity);
        }
        //xpGenerated.GetComponent<Rigidbody>().AddForce(new Vector3(rndVariant.x, 1 * m_ImpusleForceXp, rndVariant.y) , ForceMode.Impulse);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        StartCoroutine(ResetEventWithDelay(3));
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusEjection);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeEvent);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radiusEventActivePlayer);
    }

    public void ResetAltarEvent()
    {
        m_myAnimator.SetBool("ActiveEvent", false);
        resetNumber++;
        m_hasEventActivate = true;
        m_isEventOccuring = false;
        m_CurrentKillCount = 0;
        float maxHealth = 100 * (1 - 0.1f * (resetNumber + 1));
        m_objectHealthSystem.SetMaxHealth((int)maxHealth);
        m_objectHealthSystem.ResetCurrentHealth();

        //this.transform.GetChild(0).gameObject.SetActive(false);
        //Enemies.EnemyManager.EnemyTargetPlayer = true;
        //eventTextName.text = "Ready !";
        //displayAnimator.InvertDisplayStatus(2);
    }

    public IEnumerator ResetEventWithDelay(float time)
    {
        //eventTextName.text = "Finish !";
        yield return new WaitForSeconds(time);
        ResetAltarEvent();
    }



}
