using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoxRemoteVehicle.Server.Models
{
    public static class Config
    {
        public class General
        {
            public static string OpenMenuCommand { get; set; }
            public static bool EnableScreenEffects { get; set; }
        }

        public class Vehicles
        {
            public static string DefaultVehicle { get; set; }
            public static Dictionary<string, string> AllowedVehicles { get; set; } = new Dictionary<string, string> { };
        }
    }
}
