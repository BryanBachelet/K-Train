using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GuerhoubaGames.AI
{
    public class MoveTo : ActionNode
    {
        public float stopDistance;
        protected override void OnStart()
        {
            agent.state = Enemies.NpcState.MOVE;
            agent.moveComponent.minTargetDistance = stopDistance;
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (agent.moveComponent.IsInRange())
            {
                return State.SUCCESS;
            }
            else
            {
                return State.RUNNING;
            }

        }

    }
}