// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Serenity : RotationBase
    {
        public override string Name
        {
            get { return "Shadow Serenity"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Valor"),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled && !Me.IsMounted)
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force of Will", ret => Me.IsStunned),
                    Spell.Buff("Battle Readiness", ret => Me.HealthPercent <= 85),
                    Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
                    Spell.Buff("Resilience", ret => Me.HealthPercent <= 50),
                    Spell.Cast("Force Potency"),
                    Spell.Buff("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Shadow Stride", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f),
                    Spell.Buff("Force Speed", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Interrupts
                    Spell.Cast("Mind Snap", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Force Stun", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .6f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance < .5f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    Spell.Cast("Squelch", ret => Me.ForcePercent < 25 && Me.HasBuff("Force Strike")),
                    Spell.Cast("Saber Strike", ret => Me.ForcePercent < 25),

                    //Rotation
                    Spell.Cast("Squelch", ret => Me.HasBuff("Force Strike")),
                    Spell.CastOnGround("Force in Balance", ret => !Me.CurrentTarget.HasDebuff("Force Suppression") || Me.CurrentTarget.BuffCount("Force Suppression") <= 2),
                    Spell.Cast("Sever Force", ret => !Me.CurrentTarget.HasDebuff("Sever Force") || Me.CurrentTarget.DebuffTimeLeft("Sever Force") <= 2),
                    Spell.Cast("Force Breach", ret => !Me.CurrentTarget.HasDebuff("Force Breach") || Me.CurrentTarget.DebuffTimeLeft("Force Breach") <= 2),
                    Spell.Cast("Spinning Strike", ret => Me.HasBuff("Stalker's Swiftness") || Me.CurrentTarget.HealthPercent <= 30 || Me.HasBuff("Crush Spirit")),
                    Spell.Cast("Serenity Strike", ret => Me.ForcePercent > 40),
                    Spell.Cast("Double Strike")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.DoT("Force Breach", "Force Breach"),
						Spell.Cast("Cleaving Cut"),
                        Spell.Cast("Sever Force", ret => !Me.CurrentTarget.HasDebuff("Sever Force")),
                        Spell.CastOnGround("Force in Balance"),
                        Spell.Cast("Whirling Blow", ret => Me.ForcePercent > 70)
                        ));
            }
        }
    }
}
