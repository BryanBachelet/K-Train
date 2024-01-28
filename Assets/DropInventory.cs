using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInventory : MonoBehaviour
{
    public HintDropAcquisition hintDropAcquisitionObject;
    public HintDropAcquisition.DropInfo lastDropInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addNewItem(int capsuleID)
    {
        lastDropInfo.m_dropType = "[Spell]";
        lastDropInfo.m_dropImage = CapsuleManager.instance.attackInfo[capsuleID].sprite;
        lastDropInfo.dropDescription = CapsuleManager.instance.attackInfo[capsuleID].description;
        lastDropInfo.dropName = CapsuleManager.instance.attackInfo[capsuleID].name;
        hintDropAcquisitionObject.AddNewDrop(lastDropInfo);
    }

    public void AddNewArtefact(ArtefactsInfos artefact)
    {
        lastDropInfo.m_dropType = "[Artefact]";
        lastDropInfo.m_dropImage = artefact.icon;
        lastDropInfo.dropDescription = artefact.description;
        lastDropInfo.dropName = artefact.name;
        hintDropAcquisitionObject.AddNewDrop(lastDropInfo);
    }
}
