using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace MenAndBeasts
{
    public class JobDriver_LightBeacon : JobDriver
    {
        private const float WarmupTicks = 80f;

        private const float TicksBetweenRepairs = 16f;
        public static int remainingDuration = 500; // A few seconds

        protected float ticksToNextRepair;


        protected Building_BeaconUnlit Beacon => (Building_BeaconUnlit) job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);

            //Toil 1: Go to the pruning site.
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            //Toil 2: Begin pruning.
            var toil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = remainingDuration
            };
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.initAction = delegate { ticksToNextRepair = WarmupTicks; };
            toil.tickAction = delegate
            {
                var actor = pawn;
                actor.skills.Learn(SkillDefOf.Construction, 0.5f);
                var statValue = actor.GetStatValue(StatDefOf.ConstructionSpeed);
                ticksToNextRepair -= statValue;
                if (!(ticksToNextRepair <= 0f))
                {
                    return;
                }

                ticksToNextRepair += TicksBetweenRepairs;
                TargetThingA.HitPoints++;
                TargetThingA.HitPoints = Mathf.Min(TargetThingA.HitPoints, TargetThingA.MaxHitPoints);
                //if (this.TargetThingA.HitPoints == this.TargetThingA.MaxHitPoints)
                //{
                //    actor.records.Increment(RecordDefOf.ThingsRepaired);
                //    actor.jobs.EndCurrentJob(JobCondition.Succeeded, true);
                //}
            };
            toil.WithEffect(TargetThingA.def.repairEffect, TargetIndex.A);
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            yield return toil;


            //Toil 3 Unreserve
            yield return Toils_Reserve.Release(TargetIndex.A);

            //Toil 4: Transform the altar once again.
            yield return new Toil
            {
                initAction = LightBeacon,
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        public void LightBeacon()
        {
            Beacon.Light();
        }
    }
}