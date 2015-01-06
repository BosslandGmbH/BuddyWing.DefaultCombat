﻿using Buddy.BehaviorTree;
using Buddy.CommonBot;
using Buddy.Navigation;
using Buddy.Swtor;

using Action = Buddy.BehaviorTree.Action;

namespace DefaultCombat.Core
{
    public class CombatMovement
    {
        public static Composite CloseDistance(float range)
        {
            return new Decorator(ret => !DefaultCombat.MovementDisabled && BuddyTor.Me.CurrentTarget != null,
                new PrioritySelector(
                    new Decorator(ret => BuddyTor.Me.CurrentTarget.Distance < range,
                        new Action(delegate
                        {
                            Navigator.MovementProvider.StopMovement();
                            return RunStatus.Failure;
                        })),
                    new Decorator(ret => BuddyTor.Me.CurrentTarget.Distance >= range,
                        CommonBehaviors.MoveAndStop(location => BuddyTor.Me.CurrentTarget.Position, range, true)),
                    new Action(delegate { return RunStatus.Failure; })));
        }
    }
}