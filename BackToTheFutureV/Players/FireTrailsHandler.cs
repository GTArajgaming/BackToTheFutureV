﻿using FusionLibrary;
using System;
using System.Collections.Generic;

namespace BackToTheFutureV
{
    internal class FireTrailsHandler
    {
        private static List<FireTrail> fireTrails = new List<FireTrail>();

        static FireTrailsHandler()
        {
            TimeHandler.OnTimeChanged += OnTimeChanged;
        }

        public static FireTrail SpawnForTimeMachine(TimeMachine timeMachine)
        {
            FireTrail fireTrail = new FireTrail(timeMachine.Vehicle, timeMachine.Constants.FireTrailsIs99, timeMachine.Constants.FireTrailsDisappearTime, timeMachine.Constants.FireTrailsAppearTime, timeMachine.Constants.FireTrailsUseBlue, timeMachine.Constants.FireTrailsLength);
            fireTrails.Add(fireTrail);

            return fireTrail;
        }

        public static void RemoveTrail(FireTrail trail)
        {
            trail.Stop();
            fireTrails.Remove(trail);
        }

        public static void Tick()
        {
            fireTrails.ForEach(x => x.Tick());
        }

        public static void OnTimeChanged(DateTime time)
        {
            Abort();
        }

        public static void Abort()
        {
            fireTrails.ForEach(x => x.Stop());
            fireTrails.Clear();
        }
    }
}
