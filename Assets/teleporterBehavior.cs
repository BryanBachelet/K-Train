using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleporterBehavior : MonoBehaviour
{
    public Teleporter lastTeleportor;
    public Vector3 lastTpPosition;
    public bool activationTP;
    public Teleporter nextTeleporter;
    public Vector3 nextTpPosition;
    private int nextTerrainNumber = 0;

    public CameraFadeFunction cameraFadeFunction;
    public TerrainGenerator terrainGen;
    public AltarBehaviorComponent altarBehavior;

    public VisualEffect apparitionVFX;
    public VisualEffect disparitionVFX;


    public EventHolder eventHolder;
    // Start is called before the first frame update
    void Start()
    {
        if (cameraFadeFunction == null) { cameraFadeFunction = Camera.main.GetComponent<CameraFadeFunction>(); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetTeleportorData(Teleporter tpObject)
    {
        nextTeleporter = tpObject;
        nextTpPosition = tpObject.transform.position;
        nextTerrainNumber = tpObject.TeleporterNumber;
       // eventHolder.GetNewAltar(tpObject.altarBehavior);
    }

    public void ActivationTeleportation()
    {

        this.gameObject.transform.position = nextTpPosition + new Vector3(0, 10, 0);
        apparitionVFX.Play();
        cameraFadeFunction.fadeOutActivation = true;
        terrainGen.ActiveGenerationTerrain(nextTerrainNumber);
    }
}
