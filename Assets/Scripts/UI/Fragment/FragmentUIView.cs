using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuerhoubaGames.UI
{
    public class FragmentUIView : MonoBehaviour
    {
        [SerializeField] private Image m_backgroundColorImg;
        [SerializeField] private Image m_borderColorImg;
        [SerializeField] private Image m_nameImg;
        [SerializeField] private Image m_spriteImg;
        [SerializeField] private Image m_elementImg;
        [SerializeField] private TMPro.TMP_Text m_nameText;
        public TooltipTrigger tooltipTrigger;

        public void UpdateInteface(ArtefactsInfos artefactsInfos)
        {
            FragmentUIRessources instanceResources = FragmentUIRessources.instance;
          
            int indexElement = (int)artefactsInfos.gameElement;
            Debug.Assert(indexElement != 0, "Artefact "+ artefactsInfos.nameArtefact +  " doesn't have element");
           

            m_backgroundColorImg.sprite = instanceResources.backgroundSprite[(int)artefactsInfos.gameElement];
            m_elementImg.sprite = instanceResources.elementSprite[(int)artefactsInfos.gameElement ];
            m_borderColorImg.sprite = instanceResources.raretySprite[(int)artefactsInfos.levelTierFragment +1 ];
            m_spriteImg.sprite = artefactsInfos.icon;
            m_nameText.text = artefactsInfos.nameArtefact;

            tooltipTrigger.IsActive = true;
            tooltipTrigger.header = artefactsInfos.nameArtefact;
            tooltipTrigger.content = artefactsInfos.descriptionResult;
        }

        public void ResetFragmentUIView()
        {
            FragmentUIRessources instanceResources = FragmentUIRessources.instance;


            m_backgroundColorImg.sprite = instanceResources.backgroundSprite[0];
            m_elementImg.sprite = instanceResources.elementSprite[0];
            m_borderColorImg.sprite = instanceResources.raretySprite[0];
            m_spriteImg.sprite = null;
            m_nameText.text = "";

            tooltipTrigger.IsActive = false;
        }

    }
}