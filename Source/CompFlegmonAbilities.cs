using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace FlegmonCreature
{
    public class CompFlegmonAbilities : ThingComp
    {
        private int rainDanceCooldown = 0;
        private int slimeTrailCooldown = 0;
        private const int RainDanceCooldownTicks = 600000; // ~10 days
        private const int SlimeTrailCooldownTicks = 60000; // ~1 day
        private const int SlimeTrailDuration = 10000; // ~4 hours

        public override void CompTick()
        {
            base.CompTick();
            
            if (!parent.IsHashIntervalTick(2500)) return;
            
            Pawn pawn = parent as Pawn;
            if (pawn?.Map == null || pawn.Dead) return;

            // Tick cooldowns
            if (rainDanceCooldown > 0) rainDanceCooldown -= 2500;
            if (slimeTrailCooldown > 0) slimeTrailCooldown -= 2500;

            // Auto-cast abilities for tamed Flegmons
            if (pawn.Faction == Faction.OfPlayer)
            {
                // Rain Dance when drought conditions
                if (rainDanceCooldown <= 0 && ShouldPerformRainDance(pawn))
                {
                    PerformRainDance(pawn);
                }

                // Slime Trail during combat or fleeing
                if (slimeTrailCooldown <= 0 && pawn.mindState?.meleeThreat != null)
                {
                    LeaveSlimeTrail(pawn);
                }
            }
        }

        private bool ShouldPerformRainDance(Pawn pawn)
        {
            // Check if map needs rain (low rainfall or plants dying)
            GameConditionManager gameConditionManager = pawn.Map.gameConditionManager;
            
            // Check for hot weather conditions as drought alternative
            if (pawn.Map.mapTemperature.OutdoorTemp > 35f)
                return true;
                
            // Check if many plants are dying from lack of water
            int dyingPlants = 0;
            int totalPlants = 0;
            foreach (Thing thing in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Plant))
            {
                Plant plant = thing as Plant;
                if (plant != null)
                {
                    totalPlants++;
                    if (plant.Growth < 0.3f) dyingPlants++;
                }
            }
            
            return totalPlants > 50 && (float)dyingPlants / totalPlants > 0.3f;
        }

        private void PerformRainDance(Pawn pawn)
        {
            // Visual effect
            for (int i = 0; i < 5; i++)
            {
                Vector3 pos = pawn.DrawPos + new Vector3(Rand.Range(-1f, 1f), 0, Rand.Range(-1f, 1f));
                MoteMaker.MakeStaticMote(pos, pawn.Map, null);
            }

            // Increase rain chance
            WeatherDef rainWeather = DefDatabase<WeatherDef>.GetNamed("Rain");
            if (rainWeather != null && pawn.Map.weatherManager.curWeather != rainWeather)
            {
                // Force rain with 50% chance
                if (Rand.Chance(0.5f))
                {
                    pawn.Map.weatherManager.TransitionTo(rainWeather);
                    Messages.Message("MessageFlegmonRainDanceSuccess".Translate(pawn.Named("PAWN")), 
                        new TargetInfo(pawn.Position, pawn.Map, false), MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message("MessageFlegmonRainDanceFailed".Translate(pawn.Named("PAWN")), 
                        new TargetInfo(pawn.Position, pawn.Map, false), MessageTypeDefOf.NeutralEvent);
                }
            }

            rainDanceCooldown = RainDanceCooldownTicks;
        }

        private void LeaveSlimeTrail(Pawn pawn)
        {
            // Create slime trail at current position
            Thing slimeTrail = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("FlegmonSlimeTrail"));
            GenSpawn.Spawn(slimeTrail, pawn.Position, pawn.Map);
            
            // Apply to adjacent cells
            foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(new TargetInfo(pawn.Position, pawn.Map)))
            {
                if (cell.InBounds(pawn.Map) && Rand.Chance(0.6f))
                {
                    Thing adjacentSlime = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("FlegmonSlimeTrail"));
                    GenSpawn.Spawn(adjacentSlime, cell, pawn.Map);
                }
            }

            slimeTrailCooldown = SlimeTrailCooldownTicks;
            
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("MessageFlegmonLeftSlimeTrail".Translate(pawn.Named("PAWN")), 
                    new TargetInfo(pawn.Position, pawn.Map, false), MessageTypeDefOf.NeutralEvent);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Pawn pawn = parent as Pawn;
            if (pawn?.Faction != Faction.OfPlayer) yield break;

            // Rain Dance ability button
            yield return new Command_Action
            {
                defaultLabel = "AbilityRainDance".Translate(),
                defaultDesc = "AbilityRainDanceDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/RainDance", false),
                disabled = rainDanceCooldown > 0,
                disabledReason = rainDanceCooldown > 0 ? 
                    "AbilityCooldown".Translate((rainDanceCooldown / 2500).ToString()) : null,
                action = delegate
                {
                    PerformRainDance(pawn);
                }
            };

            // Slime Trail ability button
            yield return new Command_Action
            {
                defaultLabel = "AbilitySlimeTrail".Translate(),
                defaultDesc = "AbilitySlimeTrailDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/SlimeTrail", false),
                disabled = slimeTrailCooldown > 0,
                disabledReason = slimeTrailCooldown > 0 ? 
                    "AbilityCooldown".Translate((slimeTrailCooldown / 2500).ToString()) : null,
                action = delegate
                {
                    LeaveSlimeTrail(pawn);
                }
            };
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref rainDanceCooldown, "rainDanceCooldown", 0);
            Scribe_Values.Look(ref slimeTrailCooldown, "slimeTrailCooldown", 0);
        }
    }

    public class CompProperties_FlegmonAbilities : CompProperties
    {
        public CompProperties_FlegmonAbilities()
        {
            compClass = typeof(CompFlegmonAbilities);
        }
    }
}