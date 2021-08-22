using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MenAndBeasts
{
    public class Building_BeaconUnlit : Building_WorkTable
    {
        public bool ToBeLit { get; private set; }

        public void Light()
        {
            var curLoc = PositionHeld;
            var curMap = MapHeld;
            DeSpawn();
            var newBeacon = ThingMaker.MakeThing(ThingDef.Named("LotRM_GBeaconLit"));
            GenPlace.TryPlaceThing(newBeacon, curLoc, curMap, ThingPlaceMode.Direct);
            Destroy();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            yield return new Command_Toggle
            {
                hotKey = KeyBindingDefOf.Command_TogglePower,
                icon = ContentFinder<Texture2D>.Get("Things/Building/Misc/Campfire_MenuIcon"),
                defaultLabel = "LotRM_LightTheBeacon".Translate(),
                defaultDesc = "LotRM_LightTheBeaconDesc".Translate(),
                isActive = () => ToBeLit,
                toggleAction = () => ToBeLit = !ToBeLit
            };
        }
    }
}