using SharpConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CitizenFX Server Imports //
using CitizenFX;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using RoxRemoteVehicle.Server.Models;
using System.Xml.Linq;
using System.Collections.Specialized;
using static System.Net.Mime.MediaTypeNames;

namespace RoxRemoteVehicle.Server.Handlers
{
    public class ConfigReturnRes
    {
        public bool isSuccess { get; set; }
        public string resMessage { get; set; }

        public ConfigReturnRes(bool _isSuccess, string _resMessage)
        {
            isSuccess = _isSuccess;
            resMessage = _resMessage;
        }
    }

    public class ConfigHandler : BaseScript
    {
        public static ConfigReturnRes Load()
        {
            try
            {
                Configuration configFile = Configuration.LoadFromFile(string.Format("{0}/Config.ini", GetResourcePath(GetCurrentResourceName())));

                if (string.IsNullOrEmpty(configFile.ToString()))
                {
                    Exception InvalidNameException = new Exception($"\r\n\r\n^1[RoxRemoteVehicle] The Config file is empty. Please make sure to fix this!\r\n\r\n\r\n^7");

                    try
                    {
                        throw InvalidNameException;
                    }
                    catch (Exception e)
                    {
                        Debug.Write(e.Message);
                    }

                    return new ConfigReturnRes(false, "[RoxRemoteVehicle] The Config file is empty. Please make sure to fix this!");
                }

                var configFileOptionsGen = configFile["General"];
                var configFileOptionsVeh = configFile["Vehicles"];

                Config.General.OpenMenuCommand = configFileOptionsGen["OpenMenuCommand"].StringValue;
                Config.General.EnableScreenEffects = configFileOptionsGen["EnableScreenEffects"].BoolValue;

                Config.Vehicles.DefaultVehicle = configFileOptionsVeh["DefaultVehicle"].StringValue;

                var AllowedVehicles = configFileOptionsVeh["AllowedVehicles"].GetValueArray<string>();

                AllowedVehicles.ToList().ForEach(AllowedVehicle =>
                {
                    var keyValue = AllowedVehicle.Trim('{', '}').Split(':');
                    var key = keyValue[0].Trim('"');
                    var value = keyValue[1].Trim('"');

                    Config.Vehicles.AllowedVehicles.Add(key, value);
                    Debug.WriteLine($"{key} | {value}");
                });

                Debug.WriteLine($"[RoxRemoteVehicle] Config has been loaded!");
                Debug.WriteLine($"[RoxRemoteVehicle] Started successfully! Open Menu Command set as \"/{Config.General.OpenMenuCommand}\"");

                return new ConfigReturnRes(true, $"[RoxRemoteVehicle] Config has been loaded!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[RoxRemoteVehicle] Failed to start!");

                Exception InvalidNameException = new Exception($"\r\n\r\n^1[RoxRemoteVehicle] Something went wrong while loading the config.\n{ex}\r\n\r\n\r\n^7");

                try
                {
                    throw InvalidNameException;
                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                }

                return new ConfigReturnRes(false, $"[RoxRemoteVehicle] Something went wrong while loading the config.\n{ex}");
            }
        }
    }
}
