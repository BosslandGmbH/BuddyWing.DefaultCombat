﻿// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Carnage : RotationBase
    {
        public override string Name
        {
            get { return "Marauder Carnage"; }
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
					Spell.Buff("Furious Power", ret => Me.CurrentTarget.BossOrGreater()),
                    Spell.Buff("Bloodthirst", ret => CombatHotkeys.EnableRaidBuffs),
                    Spell.Buff("Cloak of Pain", ret => Me.HealthPercent <= 75),
                    //Spell.Cast("Force Camouflage"),  enable this if you use hidden savagery or want constant threat drop
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 25),
                    Spell.Buff("Undying Rage", ret => Me.HealthPercent <= 15),
                    Spell.Cast("Frenzy", ret => Me.BuffCount("Fury") < 15),
                    Spell.Cast("Berserk", ret => !Me.HasBuff("Berserk")),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Force Charge", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Vicious Throw", ret => Me.HasBuff("Slaughter") || Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Ferocity", ret => Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Ravage", ret => Me.HasBuff("Ferocity")),
                    Spell.Cast("Devastating Blast", ret => Me.HasBuff("Execute")),
                    Spell.Cast("Gore"),
                    Spell.Cast("Massacre"),
                    Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 7),
                    Spell.Cast("Assault", ret => Me.ActionPoints <= 10)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Ferocity"),
                        Spell.Cast("Sweeping Slash"),
                        Spell.Cast("Smash"),
                        Spell.Cast("Vicious Throw", ret => Me.HasBuff("Slaughter") || Me.CurrentTarget.HealthPercent <= 30),
                        Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 7)
                        ));
            }
        }
    }
}
