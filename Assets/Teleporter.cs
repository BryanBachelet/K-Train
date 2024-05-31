using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public int TeleporterNumber;
    public bool usedTeleporter = false;

    public bool isReceiver;
    public bool isSpawn;
    public bool activation = false;
    public bool teleportorIsActive = false;
    public Material socleMaterial;

    static private TerrainGenerator terrainGen;
    public AltarBehaviorComponent altarBehavior;
    public Enemies.EnemyManager enemyManager;

    public TeleporterFeebackController tpFeedbackController;
    // Start is called before the first frame update
    void Awake()
    {
        if (socleMaterial == null) socleMaterial = this.GetComponentInChildren<MeshRenderer>().material;
    
        if(terrainGen == null)
        {
            terrainGen = GameObject.Find("TerrainGenerator").GetComponent<TerrainGenerator>();
        }
        tpFeedbackController = gameObject.GetComponentInChildren<TeleporterFeebackController>();
    }


    public void ActivationTeleportor()
    {
        teleportorIsActive = true;
        tpFeedbackController.activeChange = true;
        if (socleMaterial == null)
            socleMaterial = this.GetComponentInChildren<MeshRenderer>().material;

        socleMaterial.SetFloat("_TEXMCOLINT", 50f);
    }

    public void DesactivationTeleportor()
    {
        teleportorIsActive = false;
        if(socleMaterial == null)
            socleMaterial = this.GetComponentInChildren<MeshRenderer>().material;
        socleMaterial.SetFloat("_TEXMCOLINT", -200f);

    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"  && teleportorIsActive && !isReceiver)
        {
            enemyManager.DestroyAllEnemy();
            terrainGen.SelectTerrain(TeleporterNumber);
            usedTeleporter = true;
           
        }
    }
}