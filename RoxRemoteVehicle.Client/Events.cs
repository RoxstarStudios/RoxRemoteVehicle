using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CitizenFX Server Imports //
using CitizenFX;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using RoxRemoteVehicle.Client.Menu;
using MenuAPI;
using static CitizenFX.Core.Native.API;

namespace RoxRemoteVehicle.Client
{
    public class Events : BaseScript
    {
        [EventHandler("RoxRemoteVehicle:Client:OpenMenu")]
        private void OpenMenuEvent()
        {
            MainMenu.Menu.OpenMenu();
        }
    }
}
