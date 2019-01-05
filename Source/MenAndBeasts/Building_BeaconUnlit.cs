using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MenAndBeasts
{
    public class Building_BeaconUnlit : Building_WorkTable
    {
        public void Light()
        {
            var curLoc = this.PositionHeld;
            var curMap = this.MapHeld;
            this.DeSpawn();
            var newBeacon = ThingMaker.MakeThing(ThingDef.Named("LotRM_GBeaconLit"));
            GenPlace.TryPlaceThing(newBeacon, curLoc, curMap, ThingPlaceMode.Direct);
            this.Destroy();
        }

        private bool toBeLit = false;

        public bool ToBeLit => toBeLit;

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
                yield return g;
            
            yield return new Command_Toggle
            {
                hotKey = KeyBindingDefOf.Command_TogglePower,
                icon = ContentFinder<Texture2D>.Get("Things/Building/Misc/Campfire_MenuIcon"),
                defaultLabel = "LotRM_LightTheBeacon".Translate(),
                defaultDesc = "LotRM_LightTheBeaconDesc".Translate(),
                isActive = (() => toBeLit),
                toggleAction = ()=> toBeLit = !toBeLit
            };
        }
    }
}