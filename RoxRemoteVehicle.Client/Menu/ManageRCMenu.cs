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
    public class ManageRCMenu
    {
        public static Functions _Functions = new Functions();

        public static readonly MenuAPI.Menu Menu = new MenuAPI.Menu("RoxRemoteVehicle", "~b~RC Vehicle Management Menu~s~");

        public static void CreateMenu()
        {
            MenuListItem controls = new MenuListItem("Controls", new List<string>() { "Player", "Remote Control" }, 0, "Control the RC Vehicle or Player");

            Menu.AddMenuItem(controls);

            Menu.OnListItemSelect += (_menu, _listItem, _listIndex, _itemIndex) =>
            {
                if (_listItem.Text == controls.Text)
                {
                    if (RCVehicle.List.IsHoldingVehicle == false)
                    {
                        _Functions.SwitchControls(_listIndex == 0 ? Functions.Controls.Player : Functions.Controls.RemoteControl);
                    }
                }
            };
        }
    }
}
