using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;

namespace FlegmonCreature
{
    public class CompFlegmonSocial : ThingComp
    {
        private int ticksUntilNextCuddle = 30000; // ~12 hours
        private Pawn lastCuddledPawn = null;
        private const float MoodTherapyRadius = 5f;
        private const int MoodTherapyInterval = 2500; // Check every hour

        public override void CompTick()
        {
            base.CompTick();
            
            Pawn pawn = parent as Pawn;
            if (pawn?.Map == null || pawn.Dead || pawn.Downed) return;

            // Mood therapy aura
            if (parent.IsHashIntervalTick(MoodTherapyInterval))
            {
                ApplyMoodTherapyAura(pawn);
            }

            // Cuddle seeking behavior
            if (parent.IsHashIntervalTick(250))
            {
                ticksUntilNextCuddle -= 250;
                
                if (ticksUntilNextCuddle <= 0 && pawn.Faction == Faction.OfPlayer)
                {
                    TryInitiateCuddle(pawn);
                }
            }
        }

        private void ApplyMoodTherapyAura(Pawn flegmon)
        {
            // Find nearby colonists
            List<Pawn> nearbyPawns = new List<Pawn>();
            foreach (Pawn p in flegmon.Map.mapPawns.FreeColonistsSpawned)
            {
                if (p != flegmon && p.Position.DistanceTo(flegmon.Position) <= MoodTherapyRadius)
                {
                    nearbyPawns.Add(p);
                }
            }

            // Apply mood buff
            foreach (Pawn colonist in nearbyPawns)
            {
                if (colonist.needs?.mood != null)
                {
                    Thought_Memory thought = (Thought_Memory)ThoughtMaker.MakeThought(
                        DefDatabase<ThoughtDef>.GetNamed("FlegmonMoodTherapy"));
                    colonist.needs.mood.thoughts.memories.TryGainMemory(thought);
                }
            }
        }

        private void TryInitiateCuddle(Pawn flegmon)
        {
            // Find a suitable colonist to cuddle
            Pawn targetPawn = FindCuddleTarget(flegmon);
            
            if (targetPawn != null && flegmon.jobs != null)
            {
                Job cuddleJob = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("FlegmonCuddle"), targetPawn);
                flegmon.jobs.TryTakeOrderedJob(cuddleJob);
                
                ticksUntilNextCuddle = Rand.Range(25000, 35000); // 10-14 hours
                lastCuddledPawn = targetPawn;
            }
        }

        private Pawn FindCuddleTarget(Pawn flegmon)
        {
            List<Pawn> candidates = new List<Pawn>();
            
            foreach (Pawn p in flegmon.Map.mapPawns.FreeColonistsSpawned)
            {
                if (p != lastCuddledPawn && 
                    p.Position.DistanceTo(flegmon.Position) < 30f &&
                    !p.Downed && !p.IsBurning() && 
                    p.Awake() && !p.InMentalState)
                {
                    candidates.Add(p);
                }
            }

            if (candidates.Count == 0) return null;
            
            // Prefer colonists with lower mood
            candidates.SortBy(p => p.needs?.mood?.CurLevel ?? 1f);
            return candidates[0];
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksUntilNextCuddle, "ticksUntilNextCuddle", 30000);
            Scribe_References.Look(ref lastCuddledPawn, "lastCuddledPawn");
        }
    }

    public class CompProperties_FlegmonSocial : CompProperties
    {
        public CompProperties_FlegmonSocial()
        {
            compClass = typeof(CompFlegmonSocial);
        }
    }
}