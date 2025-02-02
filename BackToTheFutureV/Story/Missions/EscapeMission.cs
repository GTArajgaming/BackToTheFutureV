﻿using FusionLibrary;
using FusionLibrary.Extensions;
using FusionLibrary.Memory;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Windows.Forms;
using static BackToTheFutureV.InternalEnums;

namespace BackToTheFutureV
{
    internal class EscapeMission : Mission
    {
        public Vehicle Vehicle { get; private set; }
        public Ped Driver { get; private set; }
        public Ped Shooter { get; private set; }
        public Ped TargetPed => TimeMachine.Vehicle.GetPedOnSeat(VehicleSeat.Driver);

        private PedGroup Peds { get; set; }
        private TimeMachine TimeMachine;

        private int step = -1;
        private int gameTimer;

        protected override void OnEnd()
        {
            if (TimeMachine != null)
            {
                TimeMachine.Properties.MissionType = MissionType.None;
                TimeMachine.Events.OnTimeTravelStarted -= OnTimeTravelStarted;
            }

            TimeMachine = null;

            Vehicle?.DeleteCompletely();

            Peds = null;
            step = -1;
        }

        public void StartOn(TimeMachine timeMachine)
        {
            if (IsPlaying)
                return;

            TextHandler.ShowSubtitle("FoundMe");
            TimeMachine = timeMachine;
            Start();
        }

        protected override void OnStart()
        {
            if (TimeMachine == null)
                TimeMachine = TimeMachineHandler.CurrentTimeMachine;

            Model model = new Model("sabregt");

            FusionUtils.LoadAndRequestModel(model);

            Vehicle = World.CreateVehicle(model, TargetPed.GetOffsetPosition(new Vector3(0, -10, 0)));
            Vehicle.Heading = TargetPed.Heading;
            Vehicle.AddBlip();

            Vehicle.MaxSpeed = (70f).ToMS();

            Driver = Vehicle.CreateRandomPedOnSeat(VehicleSeat.Driver);

            Function.Call(Hash.TASK_VEHICLE_CHASE, Driver, TargetPed);
            Function.Call(Hash.SET_TASK_VEHICLE_CHASE_BEHAVIOR_FLAG, Driver, VehicleDrivingFlags.IgnorePathFinding | VehicleDrivingFlags.AvoidVehicles | VehicleDrivingFlags.DriveBySight, true);
            Function.Call(Hash.SET_TASK_VEHICLE_CHASE_IDEAL_PURSUIT_DISTANCE, Driver, 0f);
            Function.Call(Hash.SET_DRIVER_AGGRESSIVENESS, Driver, 1.0f);

            Shooter = Vehicle.CreateRandomPedOnSeat(VehicleSeat.Passenger);
            Shooter.Weapons.Give(WeaponHash.Pistol, 999, true, true);
            Shooter.Task.VehicleShootAtPed(TargetPed);

            Peds = new PedGroup
            {
                { Driver, true },
                { Shooter, false }
            };

            TimeMachine.Events.OnTimeTravelStarted += OnTimeTravelStarted;
            TimeMachine.Properties.MissionType = MissionType.Escape;

            step = 0;
        }

        public void OnTimeTravelStarted()
        {
            if (!IsPlaying)
                return;

            gameTimer = Game.GameTime + 5000;
            step = 1;

            if (TimeMachine.Properties.TimeTravelType == TimeTravelType.Instant)
                End();
        }

        private void StopVehicle()
        {
            Shooter.Task.ClearAll();
            Driver.Task.ClearAll();

            Vehicle.SteeringAngle = FusionUtils.Random.NextDouble() >= 0.5f ? 35 : -35;
            Vehicle.IsHandbrakeForcedOn = true;
            Vehicle.Speed = Vehicle.Speed / 2;

            VehicleControl.SetBrake(Vehicle, 1f);

            gameTimer = Game.GameTime + 1250;
            step = 2;
        }

        private void ExplodeVehicle()
        {
            Vehicle.Explode();
            Vehicle.AttachedBlip?.Delete();
            End();
        }

        public override void Tick()
        {
            if (!IsPlaying)
                return;

            switch (step)
            {
                case 1:
                    if (Vehicle.DistanceToSquared2D(TargetPed, 2) || gameTimer < Game.GameTime)
                        StopVehicle();

                    break;
                case 2:
                    if (gameTimer < Game.GameTime)
                        ExplodeVehicle();

                    break;
            }
        }

        public override void KeyDown(KeyEventArgs key)
        {

        }
    }
}
