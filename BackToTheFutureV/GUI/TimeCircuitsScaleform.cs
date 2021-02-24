﻿using BackToTheFutureV.HUD.Core;
using FusionLibrary;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace BackToTheFutureV.GUI
{
    public enum TCDBackground
    {
        Metal, Transparent
    }

    public class TimeCircuitsScaleform : ScaleformGui
    {
        public TimeCircuitsScaleform() : base("bttf_2d_gui")
        {
            dates["red"] = new DateTime();
            dates["green"] = new DateTime();
            dates["yellow"] = new DateTime();
        }

        private readonly Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();

        public void Draw2D()
        {
            Render2D(ModSettings.TCDPosition, new SizeF(ModSettings.TCDScale * (1501f / 1100f) / Screen.AspectRatio, ModSettings.TCDScale));
        }

        public void SetEmpty(EmptyType emptyType)
        {
            switch (emptyType)
            {
                case EmptyType.Off:
                    CallFunction("SET_EMPTY_STATE", 0);
                    break;
                case EmptyType.On:
                    CallFunction("SET_EMPTY_STATE", 1);
                    break;
                default:
                    CallFunction("HIDE_EMPTY");
                    break;
            }
        }

        public void SetDate(string type, DateTime date)
        {
            dates[type] = date;

            CallFunction("SET_" + type.ToUpper() + "_MONTH", date.Month);
            CallFunction("SET_" + type.ToUpper() + "_DAY", date.Day);
            CallFunction("SET_" + type.ToUpper() + "_YEAR", date.Year);
            CallFunction("SET_" + type.ToUpper() + "_HOUR", ((date.Hour + 11) % 12) + 1);
            CallFunction("SET_" + type.ToUpper() + "_MINUTE", date.Minute);

            CallFunction("SET_AM_PM", type.ToLower(), date.ToString("tt", CultureInfo.InvariantCulture) == "AM" ? 1 : 2);
        }

        public void SetBackground(TCDBackground background)
        {
            CallFunction("SET_TCD_BACKGROUND", GetStringFromBackgroundType(background));
        }

        public void SetVisible(string type, bool toggle, bool month = true, bool day = true, bool year = true, bool hour = true, bool minute = true, bool amPm = true)
        {
            if (toggle)
            {
                SetDate(type, dates[type]);

                if (!month)
                    CallFunction("SET_" + type.ToUpper() + "_MONTH", -1);

                if (!day)
                    CallFunction("SET_" + type.ToUpper() + "_DAY", -1);

                if (!year)
                    CallFunction("SET_" + type.ToUpper() + "_YEAR", -1);

                if (!hour)
                    CallFunction("SET_" + type.ToUpper() + "_HOUR", -1);

                if (!minute)
                    CallFunction("SET_" + type.ToUpper() + "_MINUTE", -1);

                if (!amPm)
                    CallFunction("SET_AM_PM", type.ToLower(), 3);

            }
            else
            {
                if (month)
                    CallFunction("SET_" + type.ToUpper() + "_MONTH", -1);

                if (day)
                    CallFunction("SET_" + type.ToUpper() + "_DAY", -1);

                if (year)
                    CallFunction("SET_" + type.ToUpper() + "_YEAR", -1);

                if (hour)
                    CallFunction("SET_" + type.ToUpper() + "_HOUR", -1);

                if (minute)
                    CallFunction("SET_" + type.ToUpper() + "_MINUTE", -1);

                if (amPm)
                    CallFunction("SET_AM_PM", type.ToLower(), 3);
            }
        }

        private string GetStringFromBackgroundType(TCDBackground background)
        {
            if (background == TCDBackground.Metal)
                return "metal";
            else
                return "trans";
        }
    }
}
