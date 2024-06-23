using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuerhoubaGames.GameEnum;

public class RewardTypologie : MonoBehaviour
{
    public RewardType rewardType;
    private Chosereward choseReward;
    public RewardDistribution rewardDistribution;
    // Start is called before the first frame update



    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            rewardDistribution.RewardValidate();

            switch (rewardType)
            {
                case RewardType.UPGRADE:
                    other.GetComponent<Character.CharacterUpgrade>().GiveUpgradePoint(3);
                    other.GetComponent<Character.CharacterUpgrade>().ShowUpgradeWindow();
                    break;
                case RewardType.SPELL:
                    other.GetComponent<Character.CharacterUpgrade>().ShowSpellChoiceInteface();
                    break;
                case RewardType.ARTEFACT:
                    if (choseReward == null)
                        choseReward = FindAnyObjectByType<Chosereward>();
                    choseReward.GiveArtefact();
                    break;
                case RewardType.HEAL:
                    other.GetComponent<HealthPlayerComponent>().RestoreHealQuarter(1);
                    break;
                default:
                    break;
            }

            Destroy(this.gameObject);
        }
    }
}
