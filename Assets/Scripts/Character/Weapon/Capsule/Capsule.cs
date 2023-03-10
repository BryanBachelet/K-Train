using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CapsuleSystem
{
    [Serializable]
    public struct CapsuleAttackInfo
    {
        public string name;
        public GameObject projectile;
        public CapsuleProfil stats;
    }

    [Serializable]
    public struct CapsuleBuffInfo
    {
        public string name;
        public CharacterData stats;
        public float duration;
    }

    [Serializable]
    public enum CapsuleType
    {
       ATTACK,
       DOUBLE,
       BUFF
    }

    [Serializable]
    public class Capsule 
    {
        public string name;
        public CapsuleType type;
    }
    [Serializable]
    public class CapsuleAttack: Capsule
    {
        public CapsuleAttack(CapsuleAttackInfo info)
        {
            name = info.name;
            projectile = info.projectile;
            stats = info.stats;
        }
        public GameObject projectile;
        public CapsuleProfil stats;
    }
    [Serializable]
    public class CapsuleBuff : Capsule
    {
        public CapsuleBuff(CapsuleBuffInfo info)
        {
            name = info.name;
            profil = info.stats;
            type = CapsuleType.BUFF;
            duration = info.duration;
        }

        public CharacterData profil;
        public float duration;
    }
}