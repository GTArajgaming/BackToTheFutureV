﻿using BackToTheFutureV.Menu;
using BackToTheFutureV.Settings;
using BackToTheFutureV.Utility;
using FusionLibrary;
using GTA;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using static FusionLibrary.Enums;

namespace BackToTheFutureV.TimeMachineClasses.Handlers
{
    internal class InputHandler : HandlerPrimitive
    {
        public bool InputMode { get; private set; }

        private static string InputBuffer;
        private static bool EnterInputBuffer;

        public Keys lastInput = Keys.None;

        private string _destinationTimeRaw;
        private int _nextReset;

        private DateTime _simulateDate;
        private int _simulateDatePos = -1;
        private int _simulateDateCheck;

        private static readonly UdpClient udp = new UdpClient(1955);

        private static void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 1955);

            string message = Encoding.ASCII.GetString(udp.EndReceive(ar, ref ip));

            if (message.StartsWith("BTTFV="))
            {
                message = message.Replace("BTTFV=", "");

                if (message == "enter")
                    EnterInputBuffer = true;
                else
                    InputBuffer = message;
            }

            StartListening();
        }

        private static void StartListening()
        {
            udp.BeginReceive(Receive, new object());
        }

        static InputHandler()
        {
            StartListening();
        }

        public InputHandler(TimeMachine timeMachine) : base(timeMachine)
        {
            Events.SimulateInputDate += SimulateDateInput;
        }

        public void InputDate(DateTime date, InputType inputType)
        {
            Sounds.InputEnter?.Play();
            Properties.DestinationTime = date;
            InputMode = false;
            _destinationTimeRaw = string.Empty;

            Events.OnDestinationDateChange?.Invoke(inputType);
        }

        public override void KeyDown(KeyEventArgs e)
        {
            if (!Properties.AreTimeCircuitsOn || TcdEditer.IsEditing || RCGUIEditer.IsEditing || Properties.IsRemoteControlled || !Vehicle.IsVisible || CustomNativeMenu.ObjectPool.AreAnyVisible)
                return;

            if (ModSettings.UseInputToggle && e.KeyCode == ModControls.InputToggle)
            {
                InputMode = !InputMode;
                _nextReset = 0;
                _destinationTimeRaw = string.Empty;

                Utils.DisplayHelpText($"{BTTFVMenu.GetLocalizedText("InputMode")} {(InputMode ? BTTFVMenu.GetLocalizedText("On") : BTTFVMenu.GetLocalizedText("Off"))}");
            }

            if (!ModSettings.UseInputToggle || InputMode)
            {
                string keyCode = e.KeyCode.ToString();

                if (keyCode.Contains("NumPad") || (keyCode.Contains("D") && keyCode.Where(char.IsDigit).Count() > 0))
                {
                    if (lastInput == e.KeyCode)
                        return;

                    lastInput = e.KeyCode;
                    ProcessInputNumber(new string(keyCode.Where(char.IsDigit).ToArray()));
                }

                if (e.KeyCode == Keys.Enter)
                {
                    if (lastInput == e.KeyCode)
                        return;

                    lastInput = e.KeyCode;
                    ProcessInputEnter();
                }
            }
        }

        private void SimulateDateInput(DateTime dateTime)
        {
            _simulateDate = dateTime;
            _simulateDatePos = 0;
            _simulateDateCheck = 0;
        }

        public void ProcessInputNumber(string number)
        {
            try
            {
                Sounds.Keypad[int.Parse(number)]?.Play();

                _destinationTimeRaw += number;
                _nextReset = Game.GameTime + 15000;
            }
            catch (Exception)
            {
            }
        }

        public void ProcessInputEnter()
        {
            if (Mods.IsDMC12)
                Utils.PlayerPed.Task.PlayAnimation("veh@low@front_ds@base", "change_station", 8f, -1, AnimationFlags.CancelableWithMovement);

            // If its not a valid length/mode
            if (_destinationTimeRaw.Length != 12 && _destinationTimeRaw.Length != 4 && _destinationTimeRaw.Length != 8)
            {
                Sounds.InputEnterError?.Play();
                InputMode = false;
                _nextReset = 0;
                _destinationTimeRaw = string.Empty;
                return;
            }

            DateTime? dateTime = Utils.ParseFromRawString(_destinationTimeRaw, Properties.DestinationTime, out InputType inputType);

            if (dateTime == null)
            {
                Sounds.InputEnterError?.Play();
                InputMode = false;
                _nextReset = 0;
                _destinationTimeRaw = string.Empty;
            }
            else
            {
                InputDate(dateTime.GetValueOrDefault(), inputType);
            }
        }

        private string DateToInput(DateTime dateTime, int pos)
        {
            return dateTime.ToString("MMddyyyyHHmm")[pos].ToString();
        }

        public override void Tick()
        {
            if (lastInput != Keys.None && !Game.IsKeyPressed(lastInput))
                lastInput = Keys.None;

            if (Properties.AreTimeCircuitsOn && Utils.PlayerVehicle != null && Utils.PlayerVehicle == Vehicle)
            {
                if (_simulateDatePos > -1)
                {
                    if (_simulateDateCheck < Game.GameTime)
                    {
                        if (_simulateDatePos > 11)
                        {
                            _simulateDatePos = -1;
                            ProcessInputEnter();
                        }
                        else
                        {
                            ProcessInputNumber(DateToInput(_simulateDate, _simulateDatePos));

                            _simulateDatePos++;

                            _simulateDateCheck = Game.GameTime + 200;
                        }
                    }
                }

                if (EnterInputBuffer)
                {
                    EnterInputBuffer = false;
                    ProcessInputEnter();
                }

                if (InputBuffer != null)
                {
                    ProcessInputNumber(InputBuffer);
                    InputBuffer = null;
                }
            }

            if (Utils.PlayerVehicle == null || !Utils.PlayerVehicle.IsTimeMachine() || (Utils.PlayerVehicle == Vehicle && !Properties.AreTimeCircuitsOn))
            {
                if (EnterInputBuffer)
                    EnterInputBuffer = false;

                InputBuffer = null;
            }

            if (Game.GameTime > _nextReset)
            {
                _destinationTimeRaw = string.Empty;
            }
        }

        public override void Stop()
        {

        }

        public override void Dispose()
        {

        }
    }
}