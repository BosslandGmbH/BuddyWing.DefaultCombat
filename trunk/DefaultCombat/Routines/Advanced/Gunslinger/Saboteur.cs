// Copyright (C) 2011-2017 Bossland GmbH// See the file LICENSE for the source code's detailed license


using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Saboteur : RotationBase
	{
		public override string Name
		{
			get { return "Gunslinger Saboteur"; }
		}


		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Lucky Shots")
					);
			}
		}


		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Escape", ret => Me.IsStunned),
					Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 50),
					Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
					Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 50),
					Spell.Buff("Smuggler's Luck", ret => Me.CurrentTarget.BossOrGreater()),
					Spell.Buff("Illegal Mods", ret => Me.CurrentTarget.BossOrGreater()),
					Spell.Cast("Unity", ret => Me.HealthPercent <= 15),
					Spell.Cast("Sacrifice", ret => Me.HealthPercent <= 5)
					);
			}
		}


		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					//Low Energy
					new Decorator(ret => Me.EnergyPercent < 60,
						new PrioritySelector(
							Spell.Cast("Thermal Grenade", ret => Me.HasBuff("Seize the Moment")),
							Spell.Cast("Flurry of Bolts")
							)),


					//Movement
					CombatMovement.CloseDistance(Distance.Ranged),
										
										
					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

					//Rotation
					Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
					Spell.Cast("Explosive Charge"),
					Spell.DoTGround("Incendiary Grenade", 9000),
					Spell.Cast("Speed Shot"),
					Spell.DoT("Shock Charge", "Shock Charge"),
					Spell.Cast("Sabotage", ret => Me.CurrentTarget.HasDebuff("Shock Charge")),
					Spell.Cast("Thermal Grenade", ret => Me.HasBuff("Seize the Moment")),
					Spell.CastOnGround("XS Freighter Flyby", ret => Me.EnergyPercent > 75),
					Spell.DoT("Vital Shot", "Vital Shot"),
					Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30)
					);
			}
		}


		public override Composite AreaOfEffect
		{
			get
			{
				return new Decorator(ret => Targeting.ShouldAoe,
					new PrioritySelector(
						Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
						Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f), //--will only be active when user initiates Heroic Moment--
						Spell.CastOnGround("XS Freighter Flyby", ret => Me.IsInCover() && Me.EnergyPercent > 30),
						Spell.CastOnGround("Sweeping Gunfire", ret => Me.IsInCover() && Me.EnergyPercent > 10),
						Spell.Cast("Incendiary Grenade"),
						Spell.Cast("Thermal Grenade")
						));
			}
		}
	}
}