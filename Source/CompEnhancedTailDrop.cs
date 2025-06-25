using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace FlegmonCreature
{
    public class CompEnhancedTailDrop : ThingComp
    {
        private int ticksToNextDrop = 300000; // ~5 days
        private TailStage currentTailStage = TailStage.Full;
        private int tailRegrowthTicks = 0;
        private const int TicksPerStage = 100000; // ~1.67 days per stage

        public enum TailStage
        {
            Stub = 0,
            Growing = 1,
            Full = 2
        }

        public override void CompTick()
        {
            base.CompTick();
            
            if (!parent.IsHashIntervalTick(2500)) return;
            
            Pawn pawn = parent as Pawn;
            if (pawn?.Map == null || pawn.Dead) return;

            // Handle tail regrowth
            if (currentTailStage < TailStage.Full)
            {
                tailRegrowthTicks += 2500;
                if (tailRegrowthTicks >= TicksPerStage)
                {
                    currentTailStage++;
                    tailRegrowthTicks = 0;
                    UpdateTailHediff(pawn);
                }
            }
            else
            {
                // Full tail - count down to drop
                ticksToNextDrop -= 2500;
                if (ticksToNextDrop <= 0)
                {
                    DropTailMeat(pawn);
                }
            }
        }

        private void DropTailMeat(Pawn pawn)
        {
            if (pawn?.Map == null) return;

            // Determine if premium or regular
            bool isPremium = Rand.Chance(0.15f); // 15% chance for premium
            ThingDef meatDef = isPremium ? 
                DefDatabase<ThingDef>.GetNamed("Meat_FlegmonSchwanzPremium") : 
                DefDatabase<ThingDef>.GetNamed("Meat_FlegmonSchwanz");
            
            Thing meat = ThingMaker.MakeThing(meatDef);
            meat.stackCount = isPremium ? Rand.Range(2, 4) : Rand.Range(3, 6);
            GenPlace.TryPlaceThing(meat, pawn.Position, pawn.Map, ThingPlaceMode.Near);
            
            // Reset tail to stub
            currentTailStage = TailStage.Stub;
            tailRegrowthTicks = 0;
            ticksToNextDrop = 300000;
            UpdateTailHediff(pawn);
            
            // Message
            if (pawn.Faction == Faction.OfPlayer)
            {
                string messageKey = isPremium ? "MessageFlegmonDroppedPremiumTail" : "MessageFlegmonDroppedTail";
                Messages.Message(messageKey.Translate(pawn.Named("PAWN")), 
                    pawn, MessageTypeDefOf.PositiveEvent, false);
            }
        }

        private void UpdateTailHediff(Pawn pawn)
        {
            // Remove old tail hediffs
            RemoveTailHediffs(pawn);
            
            // Add appropriate hediff
            string hediffDefName = $"FlegmonTail{currentTailStage}";
            HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed(hediffDefName);
            if (hediffDef != null)
            {
                pawn.health.AddHediff(hediffDef);
            }
        }

        private void RemoveTailHediffs(Pawn pawn)
        {
            List<Hediff> hediffsToRemove = new List<Hediff>();
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.def.defName.StartsWith("FlegmonTail"))
                {
                    hediffsToRemove.Add(hediff);
                }
            }
            foreach (Hediff hediff in hediffsToRemove)
            {
                pawn.health.RemoveHediff(hediff);
            }
        }

        public override string CompInspectStringExtra()
        {
            string stageText;
            switch (currentTailStage)
            {
                case TailStage.Stub:
                    stageText = "TailStageStub".Translate();
                    break;
                case TailStage.Growing:
                    stageText = "TailStageGrowing".Translate();
                    break;
                case TailStage.Full:
                    stageText = "TailStageFull".Translate();
                    break;
                default:
                    stageText = "Unknown";
                    break;
            }
            
            return "TailStatus".Translate(stageText);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksToNextDrop, "ticksToNextDrop", 300000);
            Scribe_Values.Look(ref currentTailStage, "currentTailStage", TailStage.Full);
            Scribe_Values.Look(ref tailRegrowthTicks, "tailRegrowthTicks", 0);
        }
    }

    public class CompProperties_EnhancedTailDrop : CompProperties
    {
        public CompProperties_EnhancedTailDrop()
        {
            compClass = typeof(CompEnhancedTailDrop);
        }
    }
}