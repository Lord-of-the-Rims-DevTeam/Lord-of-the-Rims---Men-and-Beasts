using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MenAndBeasts
{
    public class WorkGiver_LightBeacon : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);
            }
        }

        public IEnumerable<Thing> BeaconsToLight(Pawn pawn)
        {
                List<Thing> thingsToCheck = new List<Thing>(from Thing things in pawn.Map.listerBuildings.allBuildingsColonist
                                                            where things.def.defName == "LotRM_GBeacon"
                                                            select things);
                return thingsToCheck;
            
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return BeaconsToLight(pawn);
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return BeaconsToLight(pawn).Count<Thing>() == 0;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced=false)
        {
            Building_BeaconUnlit building = t as Building_BeaconUnlit;
            if (building == null)
            {
                return false;
            }
            if (!building.ToBeLit)
            {
                return false;
            }
            if (t.Faction != pawn.Faction)
            {
                return false;
            }
            if (pawn.Faction == Faction.OfPlayer && !pawn.Map.areaManager.Home[t.Position])
            {
                JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans);
                return false;
            }
            if (!pawn.CanReserve(t)) return false;// pawn.Map.reservationManager.IsReserved(t, pawn.Faction)) return false;
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return new Job(DefDatabase<JobDef>.GetNamed("LotRM_LightBeacon"), t);
        }
    }
}
