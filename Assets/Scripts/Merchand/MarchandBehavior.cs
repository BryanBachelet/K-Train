using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;
using GuerhoubaGames.UI;

public struct ItemData
{
    public CharacterObjectType type;
    public GameElement element;
    public int price;
    public int index;
    public bool hasBeenBuy;
    public bool isBuyable;
}

public struct MerchandItemData
{
    public ItemData[] itemSpellData;
    public SpellSystem.Capsule[] spellData;
    public ItemData[] itemFragmentData;
    public ArtefactsInfos[] fragmentData;
}

public class MarchandBehavior : InteractionInterface
{
    public enum BuyResult
    {
        BUY = 0 ,
        NOT_ENOUGH_MONEY = 1
    }

    [Header("Marchand Variables")]
    public int spellPrice = 50;
    public int fragmentPrice = 25;


    private MarchandUiView m_merchandView;
    private UI_Inventory m_uiInventory;
    private UIDispatcher m_uiDispatcher;
    private CapsuleManager m_capsuleManager;
    private FragmentManager m_fragmentManager;
    private GameObject m_playerGo;
    private CristalInventory m_cristalInventory;

    [HideInInspector] public ItemData[] spellItemData = new ItemData[2];
    [HideInInspector] public ItemData[] fragmentItemData = new ItemData[2];

    private MerchandItemData merchandItemData;

    public void InitComponents()
    {
        if (GameState.m_uiManager) m_uiDispatcher = GameState.m_uiManager.GetComponent<UIDispatcher>();

        if (m_uiDispatcher == null) return;
        m_merchandView = m_uiDispatcher.marchandUiView;

        m_merchandView.marchandBehavior = this;
        m_cristalInventory = GameState.s_playerGo.GetComponent<CristalInventory>();

        m_uiInventory = m_uiDispatcher.uiInventory;
        m_capsuleManager = GameState.m_enemyManager.GetComponent<CapsuleManager>();
        m_fragmentManager = GameState.m_enemyManager.GetComponent<FragmentManager>();
        SetSpellItem();
        SetFragmentItem();
    }

    #region Interaction Functions
    public override void OnInteractionEnd(GameObject player)
    {
        m_merchandView.DeactiveMarchandUI();
        m_uiInventory.DeactivateInventoryInterface();
        m_uiDispatcher.fixeGameplayUI.SetActive(true);
        m_uiDispatcher.bandenoir.SetActive(false);
        GameState.ChangeState();

    }

    public override void OnInteractionStart(GameObject player)
    {
            m_merchandView.ActiveMarchandUI(merchandItemData);
        m_uiDispatcher.fixeGameplayUI.SetActive(false);
        m_uiDispatcher.bandenoir.SetActive(true);
        m_uiInventory.ActivateInventoryInterface();
        GameState.ChangeState();
    }
    #endregion


    public void SetSpellItem()
    {
        merchandItemData.spellData = new SpellSystem.Capsule[2];
        for (int i = 0; i < 2; i++)
        {
            int spellIndex = Random.Range(0, m_capsuleManager.capsules.Length);
            merchandItemData.spellData[i] = m_capsuleManager.capsules[spellIndex];
            spellItemData[i].index = spellIndex;
            spellItemData[i].price = spellPrice;
            spellItemData[i].type = CharacterObjectType.SPELL;
            spellItemData[i].element = m_capsuleManager.attackInfo[spellIndex].element;
            spellItemData[i].hasBeenBuy = false;
            spellItemData[i].isBuyable = m_cristalInventory.HasEnoughCristal(spellItemData[i].price, spellItemData[i].element);
        }

        merchandItemData.itemSpellData = spellItemData;
    }

    public void SetFragmentItem()
    {
        merchandItemData.fragmentData = new ArtefactsInfos[2];
        for (int i = 0; i < 2; i++)
        {
           int index = m_fragmentManager.GetRandomIndexFragment();
            merchandItemData.fragmentData[i] = m_fragmentManager.GetArtefacts(index);
            fragmentItemData[i].index = index;
            fragmentItemData[i].price = fragmentPrice;
            fragmentItemData[i].type = CharacterObjectType.FRAGMENT;
            fragmentItemData[i].element = merchandItemData.fragmentData[i].gameElement;
            fragmentItemData[i].hasBeenBuy = false;
            fragmentItemData[i].isBuyable = m_cristalInventory.HasEnoughCristal(fragmentItemData[i].price, fragmentItemData[i].element);
        }

        merchandItemData.itemFragmentData = fragmentItemData;
    }

    public BuyResult AcquiereNewSpell(int index)
    {
        // Check the price and pay price

        bool canPay = m_cristalInventory.HasEnoughCristal(merchandItemData.itemSpellData[index].price, merchandItemData.itemSpellData[index].element);

        if (!canPay || merchandItemData.itemSpellData[index].hasBeenBuy) return BuyResult.NOT_ENOUGH_MONEY;
        m_cristalInventory.RemoveCristalCount((int)merchandItemData.itemSpellData[index].element, -merchandItemData.itemSpellData[index].price);

        merchandItemData.itemSpellData[index].hasBeenBuy = true;
        // Add Spell to the player inventory
        Character.CharacterShoot characterShoot = GameState.s_playerGo.GetComponent<Character.CharacterShoot>();
        int spellIndex = merchandItemData.itemSpellData[index].index;
        characterShoot.AddSpell(spellIndex);
        CheckPlayerBuyCapacity();
        // Update Interface from inventory and Store
        m_uiInventory.ActualizeInventory();

        return BuyResult.BUY;
    }

    public BuyResult AcquiereNewFragment(int index)
    {
        bool canPay = m_cristalInventory.HasEnoughCristal(merchandItemData.itemFragmentData[index].price, merchandItemData.itemFragmentData[index].element);

        if (!canPay || merchandItemData.itemFragmentData[index].hasBeenBuy) return BuyResult.NOT_ENOUGH_MONEY;
        m_cristalInventory.RemoveCristalCount((int)merchandItemData.itemFragmentData[index].element, -merchandItemData.itemFragmentData[index].price);
       
        merchandItemData.itemFragmentData[index].hasBeenBuy = true;
        m_fragmentManager.GiveArtefact(merchandItemData.itemFragmentData[index].index, GameState.s_playerGo);
        CheckPlayerBuyCapacity();

        m_uiInventory.ActualizeInventory();
        return BuyResult.BUY;
    }

    private void CheckPlayerBuyCapacity()
    {
        for (int i = 0; i < 2; i++)
        {
            merchandItemData.itemSpellData[i].isBuyable = m_cristalInventory.HasEnoughCristal(merchandItemData.itemSpellData[i].price, merchandItemData.itemSpellData[i].element);
            merchandItemData.itemFragmentData[i].isBuyable = m_cristalInventory.HasEnoughCristal(merchandItemData.itemFragmentData[i].price, merchandItemData.itemFragmentData[i].element);
        }
    }


    public void AcquiereRandomNewSpell( DragData dragData)
    {
        int spellIndex = Random.Range(0, m_capsuleManager.capsules.Length);
        Character.CharacterShoot characterShoot = GameState.s_playerGo.GetComponent<Character.CharacterShoot>();
        characterShoot.RemoveSpell(dragData.indexObj);
        characterShoot.AddSpell(spellIndex);
        m_uiInventory.ActualizeInventory();
    }

    public void AcquiereRandomNewFragment(DragData dragData)
    {
        int index = m_fragmentManager.GetRandomIndexFragment(); 
        m_fragmentManager.RemoveFragment(dragData.indexObj, GameState.s_playerGo);
        m_fragmentManager.GiveArtefact(index, GameState.s_playerGo);
        m_uiInventory.ActualizeInventory();
    }
}
