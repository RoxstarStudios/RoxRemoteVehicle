using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CitizenFX Client Imports //
using CitizenFX;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace RoxRemoteVehicle.Client
{
    public class Main : BaseScript
    {
        Functions Functions = new Functions();

        public Main()
        {
            Debug.WriteLine("[RoxRemoteVehicle] Starting Client Script...");
            TriggerServerEvent("RoxRemoteVehicle:Server:GetConfigStuff");
            Tick += Functions.OnRemoteControlVehicleTick;
        }

        [EventHandler("RoxRemoteVehicle:Client:SendConfigStuff")]
        public void SendConfigStuff(string defaultVeh, string AllModels, bool EnableSFX)
        {
            Functions.SelectedVehicleModel = defaultVeh;
            Functions.AllVehicleModels = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(AllModels);
            Functions.EnableSFX = EnableSFX;
            Menu.MainMenu.SetupController();
        }
    }
}
