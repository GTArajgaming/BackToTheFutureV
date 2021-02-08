﻿using BackToTheFutureV.Utility;
using FusionLibrary;
using FusionLibrary.Extensions;
using GTA.Math;
using System.Collections.Generic;
using System.Windows.Forms;
using static FusionLibrary.Enums;

namespace BackToTheFutureV.TimeMachineClasses.Handlers.BaseHandlers
{
    public class PropsHandler : Handler
    {
        private AnimateProp BTTFDecals;

        //Hover Mode
        public AnimateProp HoverModeWheelsGlow;
        public AnimateProp HoverModeVentsGlow;
        public List<AnimateProp> HoverModeUnderbodyLights = new List<AnimateProp>();

        //Fuel
        public AnimateProp EmptyGlowing;
        public AnimateProp EmptyOff;

        //Hoodbox
        public AnimateProp HoodboxLights;

        //Time travel
        public AnimateProp Coils;

        //Flux capacitor
        public AnimateProp FluxBlue;

        //Plutonium gauge
        public AnimateProp GaugeGlow;

        //Speedo
        public AnimateProp Speedo;

        //Compass
        public AnimateProp Compass;

        //TFC
        public AnimateProp TFCOn;
        public AnimateProp TFCOff;
        public AnimateProp TFCHandle;

        //Time travel
        public AnimateProp WhiteSphere;
        public AnimateProp InvisibleProp;

        //Wheels
        public List<AnimateProp> RRWheels = new List<AnimateProp>();

        //TCD
        public AnimateProp DiodesOff;
        public AnimateProp TickingDiodes;
        public AnimateProp TickingDiodesOff;

        //License plate
        public AnimateProp LicensePlate;

        public PropsHandler(TimeMachine timeMachine) : base(timeMachine)
        {
            //Wheels
            RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_lf"));
            RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_lr"));
            RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_rf"));
            RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_rr"));

            if (Vehicle.Bones["wheel_lm1"].Index > 0)
                RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_lm1"));

            if (Vehicle.Bones["wheel_rm1"].Index > 0)
                RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_rm1"));

            if (Vehicle.Bones["wheel_lm2"].Index > 0)
                RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_lm2"));

            if (Vehicle.Bones["wheel_rm2"].Index > 0)
                RRWheels.Add(new AnimateProp(Vehicle, ModelHandler.RRWheelProp, "wheel_rm2"));

            WhiteSphere = new AnimateProp(Vehicle, ModelHandler.WhiteSphere, Vector3.Zero, Vector3.Zero)
            {
                Duration = 0.25f
            };

            InvisibleProp = new AnimateProp(TimeMachine.Vehicle, ModelHandler.InvisibleProp, new Vector3(0, 3.4f, -0.6f), new Vector3(0, 0, 180));
            InvisibleProp.SpawnProp();

            if (!Mods.IsDMC12)
                return;
            BTTFDecals = new AnimateProp(Vehicle, ModelHandler.BTTFDecals, Vector3.Zero, Vector3.Zero);
            BTTFDecals.SpawnProp();

            //Hover Mode
            for (int i = 1; i < 6; i++)
            {
                if (!ModelHandler.UnderbodyLights.TryGetValue(i, out var model))
                    continue;

                var prop = new AnimateProp(Vehicle, model, Vector3.Zero, Vector3.Zero);
                HoverModeUnderbodyLights.Add(prop);
            }

            HoverModeVentsGlow = new AnimateProp(Vehicle, ModelHandler.VentGlowing, Vector3.Zero, Vector3.Zero);
            HoverModeWheelsGlow = new AnimateProp(Vehicle, ModelHandler.HoverGlowing, Vector3.Zero, Vector3.Zero)
            {
                Duration = 1.8f
            };

            //Fuel
            EmptyGlowing = new AnimateProp(Vehicle, ModelHandler.Empty, Vector3.Zero, Vector3.Zero);
            EmptyOff = new AnimateProp(Vehicle, ModelHandler.EmptyOff, Vector3.Zero, Vector3.Zero);

            //Time travel
            Coils = new AnimateProp(Vehicle, ModelHandler.CoilsGlowing, Vector3.Zero, Vector3.Zero);

            //Plutonium gauge
            GaugeGlow = new AnimateProp(Vehicle, ModelHandler.GaugeGlow, Vector3.Zero, Vector3.Zero);

            //Speedo
            Speedo = new AnimateProp(Vehicle, ModelHandler.BTTFSpeedo, "bttf_speedo");
            Speedo.SpawnProp();

            //Compass
            Compass = new AnimateProp(Vehicle, ModelHandler.Compass, "bttf_compass");

            //TFC
            TFCOn = new AnimateProp(Vehicle, ModelHandler.TFCOn, Vector3.Zero, Vector3.Zero);
            TFCOff = new AnimateProp(Vehicle, ModelHandler.TFCOff, Vector3.Zero, Vector3.Zero);
            TFCHandle = new AnimateProp(Vehicle, ModelHandler.TFCHandle, new Vector3(-0.03805999f, -0.0819466f, 0.5508024f), Vector3.Zero);
            TFCHandle[AnimationType.Rotation][AnimationStep.First][Coordinate.Y].Setup(true, true, false, -45, 0, 1, 135, 1);
            TFCHandle.SpawnProp();

            //TCD
            DiodesOff = new AnimateProp(Vehicle, ModelHandler.DiodesOff, Vector3.Zero, Vector3.Zero);
            DiodesOff.SpawnProp();
            TickingDiodes = new AnimateProp(Vehicle, ModelHandler.TickingDiodes, Vector3.Zero, Vector3.Zero);
            TickingDiodesOff = new AnimateProp(Vehicle, ModelHandler.TickingDiodesOff, Vector3.Zero, Vector3.Zero);
            TickingDiodesOff.SpawnProp();

            //Hoodbox lights
            HoodboxLights = new AnimateProp(Vehicle, ModelHandler.HoodboxLights, "bonnet");

            //Flux capacitor
            FluxBlue = new AnimateProp(Vehicle, ModelHandler.FluxBlueModel, "flux_capacitor");

            //License plate
            LicensePlate = new AnimateProp(Vehicle, ModelHandler.LicensePlate, Vehicle.GetPositionOffset(Vehicle.RearPosition).GetSingleOffset(Coordinate.Z, -0.1f), Vector3.Zero);

            LicensePlate[AnimationType.Rotation][AnimationStep.First][Coordinate.Z].Setup(false, true, true, 0, 360 * 2, 1, 720, 1);
            LicensePlate[AnimationType.Rotation][AnimationStep.Second][Coordinate.X].Setup(false, true, false, -90, 0, 1, 180, 1);
            LicensePlate[AnimationType.Offset][AnimationStep.Second][Coordinate.Z].Setup(false, true, true, -0.1f, -0.07f, 1, 0.06f, 1);

            LicensePlate.SaveAnimation();
        }

        public override void Dispose()
        {
            BTTFDecals?.Dispose();

            //Hover Mode
            HoverModeWheelsGlow?.Dispose();
            HoverModeVentsGlow?.Dispose();

            foreach (var prop in HoverModeUnderbodyLights)
                prop?.Dispose();

            //Fuel
            EmptyGlowing?.Dispose();
            EmptyOff?.Dispose();

            //Hoodbox
            HoodboxLights?.Dispose();

            //Time travel
            Coils?.Dispose();
            WhiteSphere?.Dispose();

            //Plutonium gauge
            GaugeGlow?.Dispose();

            //Speedo
            Speedo?.Dispose();

            //Compass
            Compass?.Dispose();

            //TFC
            TFCOn?.Dispose();
            TFCOff?.Dispose();
            TFCHandle?.Dispose();

            //Wheels
            RRWheels?.ForEach(x => x?.Dispose());

            //TCD
            DiodesOff?.Dispose();
            TickingDiodes?.Dispose();
            TickingDiodesOff?.Dispose();

            //Flux capacitor
            FluxBlue?.Dispose();

            //License plate
            LicensePlate?.Dispose(LicensePlate.IsSpawned);
        }

        public override void KeyDown(Keys key)
        {
            //if (key == Keys.L)
            //{
            //    LicensePlate.SpawnProp();
            //    LicensePlate.Play();
            //}
        }

        public override void Process()
        {

        }

        public override void Stop()
        {

        }
    }
}
