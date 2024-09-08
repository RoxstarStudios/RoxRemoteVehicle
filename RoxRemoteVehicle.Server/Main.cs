using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoxRemoteVehicle.Server.Handlers;

// CitizenFX Server Imports //
using CitizenFX;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using RoxRemoteVehicle.Server.Models;

namespace RoxRemoteVehicle.Server
{
    public class Main : BaseScript
    {
        public Main()
        {
            Debug.WriteLine("[RoxRemoteVehicle] Starting Server Script...");

            ConfigHandler.Load();

            RegisterCommand(Config.General.OpenMenuCommand, new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (source > 0)
                {
                    Player sourcePly = Players[source];
                    sourcePly.TriggerEvent("RoxRemoteVehicle:Client:OpenMenu");
                }
            }), false);
        }

        [EventHandler("RoxRemoteVehicle:Server:GetConfigStuff")]
        public void GetConfigStuff([FromSource] Player player)
        {
            player.TriggerEvent("RoxRemoteVehicle:Client:SendConfigStuff", Config.Vehicles.DefaultVehicle, Newtonsoft.Json.JsonConvert.SerializeObject(Config.Vehicles.AllowedVehicles), Config.General.EnableScreenEffects);
        }
    }
}
