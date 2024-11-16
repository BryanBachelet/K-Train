using GuerhoubaGames.GameEnum;
using UnityEngine;
using GuerhoubaGames.Resources;
using UnityEditor;
public enum ConditionsTrigger
{
    OnHit,
    OnDeath,
    Contact,
}

public enum EntitiesTrigger
{
    Enemies,
    Self,
    Decor,
}


public enum EntitiesTargetSystem
{
    EnemyHit,
    ClosestEnemyAround,
    EnemyRandomAround,
}

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Artefacts Info", order = 1)]
public class ArtefactsInfos : ScriptableObject
{
    [Range(0, 100)]
    public float spawnRate = 5;
    public float radius = 30;
    public GameElement gameElement;
    public ConditionsTrigger conditionsTrigger;
    public EntitiesTrigger entitiesTrigger;
    public EntitiesTargetSystem entitiesTargetSystem;

    public GameObject m_artefactToSpawn;
    public GameObject m_artefactProjectile;
    [HideInInspector] public GameObject characterGo;
    public Sprite icon;
    [TextArea]
    public string description;
    public string nameArtefact;
    public bool isDebugActive = false;

    [HideInInspector] public int activationCount;

    private int proc = 0;
    public int maxProc = 10;
    private float lastTimeRefresh = 0;




    public ArtefactsInfos Clone()
    {
        ArtefactsInfos clone = Instantiate(this);
        return clone;
    }

    public void ActiveArtefactOnHit(Vector3 position, EntitiesTrigger tag, GameObject objectPre, GameElement element)
    {

        if (entitiesTrigger != tag) return;
        if (element != gameElement && element != GameElement.NONE) return;


        float change = Random.Range(0, 100.0f);
        if (change < spawnRate)
        {
            if (isDebugActive)
            { 
                Debug.Log("Artefact active OnHit"); 
            }
            if (!CanApplyOnHit()) return;
            GameObject obj = GamePullingSystem.SpawnObject(m_artefactToSpawn, position, Quaternion.identity);
            Artefact.ArtefactData artefactData = obj.GetComponent<Artefact.ArtefactData>();
            SetupArtefactData(artefactData, objectPre);
            artefactData.OnSpawn?.Invoke();
            activationCount++;
        }

    }

    public void ActiveArtefactOnDeath(Vector3 position, EntitiesTrigger tag, GameObject agent, float distance)
    {
        if(isDebugActive) Debug.Log("Artefact not in range");

        if (entitiesTrigger != tag || distance > radius) return;

        if (isDebugActive) Debug.Log ("Artefact in range");

        float change = Random.Range(0, 100);
        if (change < spawnRate)
        {
            if (isDebugActive) Debug.Log("Artefact in launch");
            GameObject obj = GamePullingSystem.SpawnObject(m_artefactToSpawn, position, Quaternion.identity);
            Artefact.ArtefactData artefactData = obj.GetComponent<Artefact.ArtefactData>();
            SetupArtefactData(artefactData,agent);
            artefactData.OnSpawn?.Invoke();
            activationCount++;
        }
    }

    public void SetupArtefactData(Artefact.ArtefactData artefactData , GameObject agent)
    {
        artefactData.agent = agent;
        artefactData.entitiesTargetSystem = entitiesTargetSystem;
        artefactData.characterGo = characterGo;
        artefactData.radius = radius;
        artefactData.nameArtefact = nameArtefact;
        artefactData.element = (int)gameElement;
    }

    public bool CanApplyOnHit()
    {
        bool usable = false;
        if (Time.time >= lastTimeRefresh + 0.1f)
        {
            lastTimeRefresh = Time.time;
            proc = 0;
        }
        if (proc < maxProc)
        {
            proc++;
            usable = true;
        }
        else
        {
            usable = false;
        }


        return usable;
    }
}
