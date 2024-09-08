using CitizenFX.Core;

using RoxRemoteVehicle.Client.Models;

using MenuAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoxRemoteVehicle.Client.Menu
{
    public class CreditsMenu
    {
        public static Functions _Functions = new Functions();

        public static readonly MenuAPI.Menu Menu = new MenuAPI.Menu("RoxRemoteVehicle", "~b~Credits~s~");

        public static void CreateMenu()
        {
            MenuItem Owner = new MenuItem("Owner", "RoxRemoteVehicle is owned by Roxstar Studios.");
            MenuItem Developer = new MenuItem("Developer", "Developed by XdGoldenTiger (461 - Chris D | OCRP)");
            MenuItem Stewartc = new MenuItem("Addon Vehicles", "Special thanks to 13Stewartc for giving us permission to use his addon RC vehicle pack (can be found on GTA5 Mods).");
            MenuItem DarksAnim = new MenuItem("Custom Animations", "Special thanks to Darks Animations for allowing us to use a custom asset for the remote control animation.");

            Menu.AddMenuItem(Owner);
            Menu.AddMenuItem(Developer);
            Menu.AddMenuItem(Stewartc);
            Menu.AddMenuItem(DarksAnim);
        }
    }
}
