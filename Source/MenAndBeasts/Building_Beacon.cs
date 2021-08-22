using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MenAndBeasts
{
    public class Building_Beacon : Building_WorkTable
    {
        private bool allySummoned;


        private Faction LocalFaction => Faction ?? Faction.OfPlayerSilentFail;

        public override void Tick()
        {
            base.Tick();
            if (allySummoned)
            {
                return;
            }

            allySummoned = true;
            var potentialAllies = Find.World.factionManager.AllFactionsVisible.Where(x =>
                !x.IsPlayer && x.PlayerRelationKind == FactionRelationKind.Ally);
            if (!potentialAllies.Any())
            {
                Messages.Message("LotRM_BeaconLitNoAllies".Translate(LocalFaction.Name), this,
                    MessageTypeDefOf.NegativeEvent);
                return;
            }

            var potentialAlly = potentialAllies.RandomElementByWeight(fac => fac.PlayerGoodwill + 120.000008f);

            var ofPlayer = Faction.OfPlayer;
            var goodwillChange = -25;
            string reason = "GoodwillChangedReason_RequestedMilitaryAid".Translate();
            potentialAlly.TryAffectGoodwillWith(ofPlayer, goodwillChange, false);
            potentialAlly.Notify_GoodwillSituationsChanged(ofPlayer, false, reason, GlobalTargetInfo.Invalid);
            var incidentParms = new IncidentParms
            {
                target = MapHeld,
                faction = potentialAlly,
                raidArrivalModeForQuickMilitaryAid = true,
                points = DiplomacyTuning.RequestedMilitaryAidPointsRange.RandomInRange
            };
            potentialAlly.lastMilitaryAidRequestTick = Find.TickManager.TicksGame;
            IncidentDefOf.RaidFriendly.Worker.TryExecute(incidentParms);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref allySummoned, "allySummoned");
        }
    }
}