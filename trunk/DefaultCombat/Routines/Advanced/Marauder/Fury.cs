﻿// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Fury : RotationBase
    {
        public override string Name
        {
            get { return "Marauder Fury"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Unnatural Might")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Unleash", ret => Me.IsStunned),
                    Spell.Buff("Bloodthirst", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Buff("Cloak of Pain", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Force Camouflage", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 25),
                    Spell.Buff("Undying Rage", ret => Me.HealthPercent <= 15),
                    Spell.Buff("Frenzy", ret => Me.BuffCount("Fury") < 15),
                    Spell.Buff("Berserk", ret => !Me.HasBuff("Berserk")),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Dual Saber Throw", ret => CombatHotkeys.EnableCharge && !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Force Charge", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Force Crush", ret => !Me.HasBuff("Destruction")),
                    Spell.Cast("Obliterate", ret => CombatHotkeys.EnableCharge && !Me.HasBuff("Dominate")),
                    Spell.Cast("Furious Strike"),
                    Spell.Cast("Raging Burst", ret => Me.HasBuff("Destruction") || Me.HasBuff("Dominate")),
                    Spell.Cast("Ravage", ret => Me.HasBuff("Cascading Power")),
                    Spell.Cast("Battering Assault", ret => Me.HasBuff("Enraged Slash")),
                    Spell.Cast("Force Scream", ret => Me.HasBuff("Battle Cry")),
                    Spell.Cast("Vicious Slash"),
                    Spell.Cast("Assault", ret => Me.ActionPoints <= 4)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Smash"),
                        Spell.Cast("Sweeping Slash"),
                        Spell.Cast("Dual Saber Throw", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance <= 3f)
                        ));
            }
        }
    }
}
