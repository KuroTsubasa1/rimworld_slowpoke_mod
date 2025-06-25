using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using UnityEngine;

namespace FlegmonCreature
{
    public class JobDriver_FlegmonBathe : JobDriver
    {
        private const int BathingTicks = 2500;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Go to water
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            
            // Bathe
            Toil bathToil = new Toil();
            bathToil.initAction = delegate
            {
                pawn.pather.StopDead();
            };
            bathToil.tickAction = delegate
            {
                pawn.Rotation = Rot4.Random;
                if (Find.TickManager.TicksGame % 100 == 0)
                {
                    MoteMaker.MakeStaticMote(pawn.DrawPos, pawn.Map, null);
                }
            };
            bathToil.defaultCompleteMode = ToilCompleteMode.Delay;
            bathToil.defaultDuration = BathingTicks;
            bathToil.AddFinishAction(delegate
            {
                // Apply wet hediff
                HediffDef wetHediff = DefDatabase<HediffDef>.GetNamed("FlegmonWet", false);
                if (wetHediff != null)
                {
                    pawn.health.AddHediff(wetHediff);
                }
                
                // Remove needs bath hediff
                Hediff needsBath = pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("FlegmonNeedsBath", false));
                if (needsBath != null)
                {
                    pawn.health.RemoveHediff(needsBath);
                }
                
                Messages.Message("MessageFlegmonWentToBathe".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
            });
            
            yield return bathToil;
        }
    }
    
    public class JobDriver_FlegmonCuddle : JobDriver
    {
        private const int CuddleDuration = 1000;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnDowned(TargetIndex.A);
            
            // Go to target
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            
            // Cuddle
            Toil cuddleToil = new Toil();
            cuddleToil.initAction = delegate
            {
                Pawn targetPawn = (Pawn)TargetA.Thing;
                pawn.rotationTracker.FaceTarget(targetPawn);
            };
            cuddleToil.tickAction = delegate
            {
                if (Find.TickManager.TicksGame % 60 == 0)
                {
                    MoteMaker.MakeStaticMote(pawn.DrawPos + new Vector3(0, 0, 0.5f), pawn.Map, null);
                }
            };
            cuddleToil.defaultCompleteMode = ToilCompleteMode.Delay;
            cuddleToil.defaultDuration = CuddleDuration;
            cuddleToil.AddFinishAction(delegate
            {
                Pawn targetPawn = (Pawn)TargetA.Thing;
                if (targetPawn != null && !targetPawn.Dead)
                {
                    // Apply cuddle thought
                    ThoughtDef cuddleThought = DefDatabase<ThoughtDef>.GetNamed("CuddledByFlegmon", false);
                    if (cuddleThought != null && targetPawn.needs?.mood != null)
                    {
                        targetPawn.needs.mood.thoughts.memories.TryGainMemory(cuddleThought);
                    }
                }
            });
            
            yield return cuddleToil;
        }
    }
}