﻿using LemonUI.Menus;
using System;
using System.ComponentModel;

namespace BackToTheFutureV
{
    internal class SoundsSettingsMenu : BTTFVMenu
    {
        private NativeCheckboxItem playFluxCapacitorSound;
        private NativeCheckboxItem playDiodeSound;
        private NativeCheckboxItem playSpeedoBeep;
        private NativeCheckboxItem playEngineSounds;

        public SoundsSettingsMenu() : base("Sounds")
        {
            playFluxCapacitorSound = NewCheckboxItem("FluxCapacitor", ModSettings.PlayFluxCapacitorSound);
            playDiodeSound = NewCheckboxItem("CircuitsBeep", ModSettings.PlayDiodeBeep);
            playSpeedoBeep = NewCheckboxItem("SpeedoBeep", ModSettings.PlaySpeedoBeep);
            playEngineSounds = NewCheckboxItem("Engine", ModSettings.PlayEngineSounds);
        }

        public override void Menu_Closing(object sender, CancelEventArgs e)
        {

        }

        public override void Menu_OnItemActivated(NativeItem sender, EventArgs e)
        {

        }

        public override void Menu_OnItemCheckboxChanged(NativeCheckboxItem sender, EventArgs e, bool Checked)
        {
            if (sender == playFluxCapacitorSound)
                ModSettings.PlayFluxCapacitorSound = Checked;

            if (sender == playDiodeSound)
                ModSettings.PlayDiodeBeep = Checked;

            if (sender == playSpeedoBeep)
                ModSettings.PlaySpeedoBeep = Checked;

            if (sender == playEngineSounds)
                ModSettings.PlayEngineSounds = Checked;

            ModSettings.SaveSettings();
        }

        public override void Menu_OnItemSelected(NativeItem sender, SelectedEventArgs e)
        {

        }

        public override void Menu_OnItemValueChanged(NativeSliderItem sender, EventArgs e)
        {

        }

        public override void Menu_Shown(object sender, EventArgs e)
        {

        }

        public override void Tick()
        {

        }
    }
}
