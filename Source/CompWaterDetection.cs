using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace FlegmonCreature
{
    public class CompWaterDetection : ThingComp
    {
        private int ticksSinceLastScan = 0;
        private const int ScanInterval = 60000; // ~1 day
        private const float DetectionRadius = 50f;
        private List<IntVec3> knownWaterSources = new List<IntVec3>();
        private List<IntVec3> markedWaterSources = new List<IntVec3>();

        public override void CompTick()
        {
            base.CompTick();
            
            if (!parent.IsHashIntervalTick(2500)) return;
            
            Pawn pawn = parent as Pawn;
            if (pawn?.Map == null || pawn.Dead) return;

            ticksSinceLastScan += 2500;

            // Perform water detection scan
            if (ticksSinceLastScan >= ScanInterval && pawn.Faction == Faction.OfPlayer)
            {
                ScanForWaterSources(pawn);
                ticksSinceLastScan = 0;
            }

            // Mark newly discovered water sources
            MarkWaterSourcesOnMap(pawn);
        }

        private void ScanForWaterSources(Pawn pawn)
        {
            List<IntVec3> newWaterSources = new List<IntVec3>();
            
            // Search for water tiles within detection radius
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(pawn.Position, DetectionRadius, true))
            {
                if (cell.InBounds(pawn.Map))
                {
                    TerrainDef terrain = cell.GetTerrain(pawn.Map);
                    
                    // Check if terrain is water
                    if (IsWaterTerrain(terrain) && !knownWaterSources.Contains(cell))
                    {
                        // Sample the water area (don't mark every single water tile)
                        bool shouldMark = true;
                        foreach (IntVec3 known in knownWaterSources)
                        {
                            if (known.DistanceTo(cell) < 10f)
                            {
                                shouldMark = false;
                                break;
                            }
                        }
                        
                        if (shouldMark)
                        {
                            newWaterSources.Add(cell);
                        }
                    }
                }
            }

            // Add new water sources to known list
            if (newWaterSources.Count > 0)
            {
                knownWaterSources.AddRange(newWaterSources);
                
                Messages.Message("MessageFlegmonDetectedWater".Translate(
                    pawn.Named("PAWN"), 
                    newWaterSources.Count.Named("COUNT")), 
                    pawn, MessageTypeDefOf.PositiveEvent);
            }
        }

        private bool IsWaterTerrain(TerrainDef terrain)
        {
            if (terrain == null) return false;
            
            // Check various water terrain types
            return terrain == TerrainDefOf.WaterShallow ||
                   terrain == TerrainDefOf.WaterMovingShallow ||
                   terrain == TerrainDefOf.WaterOceanShallow ||
                   terrain == TerrainDefOf.WaterDeep ||
                   terrain == TerrainDefOf.WaterMovingChestDeep ||
                   terrain == TerrainDefOf.WaterOceanDeep ||
                   terrain.defName.ToLower().Contains("water") ||
                   terrain.defName.ToLower().Contains("lake") ||
                   terrain.defName.ToLower().Contains("river");
        }

        private void MarkWaterSourcesOnMap(Pawn pawn)
        {
            // Mark water sources that haven't been marked yet
            foreach (IntVec3 waterSource in knownWaterSources)
            {
                if (!markedWaterSources.Contains(waterSource))
                {
                    // Check if the water source is still valid
                    if (waterSource.InBounds(pawn.Map) && IsWaterTerrain(waterSource.GetTerrain(pawn.Map)))
                    {
                        // Create a map marker
                        Designation designation = new Designation(waterSource, 
                            DefDatabase<DesignationDef>.GetNamed("FlegmonWaterMarker", false));
                        
                        if (designation.def != null)
                        {
                            pawn.Map.designationManager.AddDesignation(designation);
                        }
                        else
                        {
                            // Fallback: Use plan designation with specific symbol
                            pawn.Map.designationManager.AddDesignation(new Designation(waterSource, 
                                DesignationDefOf.Plan));
                        }
                        
                        markedWaterSources.Add(waterSource);
                    }
                }
            }
        }

        public void ShareWaterKnowledge(CompWaterDetection otherComp)
        {
            // Share water source knowledge between Flegmons
            foreach (IntVec3 waterSource in otherComp.knownWaterSources)
            {
                if (!knownWaterSources.Contains(waterSource))
                {
                    knownWaterSources.Add(waterSource);
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksSinceLastScan, "ticksSinceLastScan", 0);
            Scribe_Collections.Look(ref knownWaterSources, "knownWaterSources", LookMode.Value);
            Scribe_Collections.Look(ref markedWaterSources, "markedWaterSources", LookMode.Value);
            
            if (knownWaterSources == null) knownWaterSources = new List<IntVec3>();
            if (markedWaterSources == null) markedWaterSources = new List<IntVec3>();
        }

        public override string CompInspectStringExtra()
        {
            if (knownWaterSources.Count > 0)
            {
                return "WaterSourcesKnown".Translate(knownWaterSources.Count);
            }
            return null;
        }
    }

    public class CompProperties_WaterDetection : CompProperties
    {
        public CompProperties_WaterDetection()
        {
            compClass = typeof(CompWaterDetection);
        }
    }
}