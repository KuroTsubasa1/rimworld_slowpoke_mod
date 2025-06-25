using RimWorld;
using Verse;

namespace FlegmonCreature
{
    public class CompFlegmonTailDrop : ThingComp
    {
        private int ticksToNextDrop = 300000; // ~5 days

        public override void CompTick()
        {
            base.CompTick();
            
            if (parent.IsHashIntervalTick(2500)) // Check every ~1 hour
            {
                ticksToNextDrop -= 2500;
                
                if (ticksToNextDrop <= 0)
                {
                    DropTailMeat();
                    ticksToNextDrop = 300000; // Reset timer
                }
            }
        }

        private void DropTailMeat()
        {
            Pawn pawn = parent as Pawn;
            if (pawn?.Map != null && !pawn.Dead)
            {
                Thing meat = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Meat_FlegmonSchwanz"));
                meat.stackCount = Rand.Range(3, 6);
                GenPlace.TryPlaceThing(meat, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                
                if (pawn.Faction == Faction.OfPlayer)
                {
                    Messages.Message("MessageFlegmonDroppedTail".Translate(pawn.Named("PAWN")), 
                        pawn, MessageTypeDefOf.PositiveEvent, false);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksToNextDrop, "ticksToNextDrop", 300000);
        }
    }

    public class CompProperties_FlegmonTailDrop : CompProperties
    {
        public CompProperties_FlegmonTailDrop()
        {
            compClass = typeof(CompFlegmonTailDrop);
        }
    }
}