using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using UnityEngine;

namespace FlegmonCreature
{
    public class CompWaterLover : ThingComp
    {
        private int ticksSinceLastBath = 0;
        private const int TicksUntilUnhappy = 180000; // ~3 days
        private const int BathingDuration = 2500; // ~1 hour
        private bool isBathing = false;
        private int bathingTicks = 0;

        public CompProperties_WaterLover Props => (CompProperties_WaterLover)props;

        public override void CompTick()
        {
            base.CompTick();
            
            if (!parent.IsHashIntervalTick(250)) return;
            
            Pawn pawn = parent as Pawn;
            if (pawn?.Map == null || pawn.Dead) return;

            ticksSinceLastBath += 250;

            // Handle bathing state
            if (isBathing)
            {
                bathingTicks += 250;
                if (bathingTicks >= BathingDuration)
                {
                    CompleteBathing(pawn);
                }
                return;
            }

            // Check if needs bath
            if (ticksSinceLastBath > TicksUntilUnhappy && pawn.Faction == Faction.OfPlayer)
            {
                // Try to find water
                IntVec3 waterCell = FindNearbyWater(pawn);
                if (waterCell.IsValid && pawn.mindState?.duty == null)
                {
                    StartBathing(pawn, waterCell);
                }
            }

            // Apply mood effects
            UpdateMoodEffects(pawn);
        }

        private IntVec3 FindNearbyWater(Pawn pawn)
        {
            Map map = pawn.Map;
            Room room = pawn.GetRoom();
            
            for (int i = 0; i < 50; i++)
            {
                IntVec3 cell = pawn.Position + GenRadial.RadialPattern[i];
                if (cell.InBounds(map) && cell.GetTerrain(map).IsWater)
                {
                    return cell;
                }
            }
            
            return IntVec3.Invalid;
        }

        private void StartBathing(Pawn pawn, IntVec3 waterCell)
        {
            isBathing = true;
            bathingTicks = 0;
            
            // Make pawn move to water
            Job job = JobMaker.MakeJob(JobDefOf.Goto, waterCell);
            pawn.jobs.TryTakeOrderedJob(job);
        }

        private void CompleteBathing(Pawn pawn)
        {
            isBathing = false;
            bathingTicks = 0;
            ticksSinceLastBath = 0;

            // Apply wet hediff
            Hediff wetHediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("WetFlegmon"), pawn);
            pawn.health.AddHediff(wetHediff);

            // Show message
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("MessageFlegmonBathed".Translate(pawn.Named("PAWN")), 
                    pawn, MessageTypeDefOf.PositiveEvent, false);
            }
        }

        private void UpdateMoodEffects(Pawn pawn)
        {
            if (pawn.needs?.mood == null) return;

            // Remove old thoughts
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(DefDatabase<ThoughtDef>.GetNamed("FlegmonNeedsBath"));
            pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(DefDatabase<ThoughtDef>.GetNamed("FlegmonRecentlyBathed"));

            // Add appropriate thought
            if (ticksSinceLastBath < 60000) // Less than 1 day
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("FlegmonRecentlyBathed"));
            }
            else if (ticksSinceLastBath > TicksUntilUnhappy)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("FlegmonNeedsBath"));
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksSinceLastBath, "ticksSinceLastBath", 0);
            Scribe_Values.Look(ref isBathing, "isBathing", false);
            Scribe_Values.Look(ref bathingTicks, "bathingTicks", 0);
        }
    }

    public class CompProperties_WaterLover : CompProperties
    {
        public CompProperties_WaterLover()
        {
            compClass = typeof(CompWaterLover);
        }
    }
}