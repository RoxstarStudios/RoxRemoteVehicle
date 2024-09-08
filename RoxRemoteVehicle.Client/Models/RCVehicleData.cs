using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;

namespace RoxRemoteVehicle.Client.Models
{
    public class RCVehicleData
    {
        public bool Enabled;
        public Functions.Controls Controls;
        public Vehicle Vehicle;
        public bool IsHoldingVehicle;
        public Vector3 VehiclePos;
        public Camera VehicleCam;
    }

    public class RCVehicle
    {
        public static RCVehicleData List = new RCVehicleData()
        {
            Enabled = false,
            Controls = Functions.Controls.Player,
            Vehicle = new Vehicle(0),
            IsHoldingVehicle = true,
            VehiclePos = new Vector3(0, 0, 0),
            VehicleCam = new Camera(0)
        };
    }
}
