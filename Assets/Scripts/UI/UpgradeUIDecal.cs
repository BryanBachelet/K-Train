using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIDecal : MonoBehaviour
{
    public Text[] m_upgradeName = new Text[3];
    public Text[] m_upgradeDescription = new Text[3];
    public Image[] m_upgradeIcon = new Image[3];
    public Image[] m_upgradeTypeIcon = new Image[3];

    public GameObject upgradePanelGameObject;

    public GameObject selectionPanelGameObject;
    public GameObject spellEquipPanelGameObject;
    public GameObject spellselectDescriptionPanelGameObject;

    private Image m_iconSpellSelected;
    private TMP_Text[] m_textDescription;

    public GameObject upgradeScreenState;
    // Start is called before the first frame update
    void Start()
    {
        m_iconSpellSelected = spellselectDescriptionPanelGameObject.GetComponentInChildren<Image>();
        m_textDescription = spellselectDescriptionPanelGameObject.GetComponentsInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUpgradeDisplay(Upgrade[] upgrades)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            m_upgradeName[i].text = upgrades[i].gain.nameUgrade;
            m_upgradeDescription[i].text = upgrades[i].gain.description;
            m_upgradeIcon[i].sprite = upgrades[i].gain.icon_Associat;
        }
    }

    public void SpellFocusDisplay(CapsuleSystem.Capsule infoSpell)
    {
        m_textDescription[0].text = infoSpell.name;
        m_textDescription[1].text = infoSpell.description;
        m_iconSpellSelected.sprite = infoSpell.sprite;
    }
    public void ChangeStateDisplay(bool newState)
    {
        selectionPanelGameObject.SetActive(newState);
        spellEquipPanelGameObject.SetActive(newState);
        spellselectDescriptionPanelGameObject.SetActive(newState);
    }

}
