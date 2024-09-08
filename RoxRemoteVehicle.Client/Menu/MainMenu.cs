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
    public class MainMenu
    {
        public static Functions _Functions = new Functions();

        public static readonly MenuAPI.Menu Menu = new MenuAPI.Menu("RoxRemoteVehicle", "~b~Main Menu~s~");

        public static void SetupController()
        {
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Left;

            CreateMenu();
            ManageRCMenu.CreateMenu();
            CreditsMenu.CreateMenu();
        }

        public static void CreateMenu()
        {
            Menu.ClearMenuItems();
            MenuController.AddMenu(Menu);

            MenuListItem selectedVehicle = new MenuListItem("RC Vehicle Model", Functions.AllVehicleModels.Values.Select((s) => s.Replace("\" ", "").Replace(" \"", "")).ToList(), 0, "Select the RC Vehicle you want to use!");

            MenuItem enableDisableVehicle = new MenuItem("Enable RC Vehicle", "Enable/Disable the RC Vehicle")
            {
                Label = "🟢"
            };

            MenuItem manageVehicle = new MenuItem("RC Vehicle Management", "Manage the RC Vehicle")
            {
                Label = "→→→",
                Enabled = false,
                Description = "Enable RC Vehicles to access this menu."
            };

            MenuItem credits = new MenuItem("Credits", "Thank You!")
            {
                Label = "ℹ️",
                Enabled = true,
                Description = "Thank You!"
            };

            Menu.AddMenuItem(selectedVehicle);
            Menu.AddMenuItem(enableDisableVehicle);
            Menu.AddMenuItem(manageVehicle);
            Menu.AddMenuItem(credits);
            MenuController.BindMenuItem(Menu, ManageRCMenu.Menu, manageVehicle);
            MenuController.BindMenuItem(Menu, CreditsMenu.Menu, credits);

            Menu.OnListIndexChange += (_menu, _listItem, _oldIndex, _newIndex, _itemIndex) =>
            {
                if (_listItem.Text == selectedVehicle.Text)
                {
                    string ModelName = Functions.AllVehicleModels.Values.ToList()[_newIndex].Replace("\" ", "").Replace(" \"", "");

                    var modelFromName = Functions.AllVehicleModels.Single((s) => s.Value.Replace("\" ", "").Replace(" \"", "") == ModelName);

                    Debug.WriteLine($"{modelFromName.Value.Replace("\" ", "").Replace(" \"", "")} | {modelFromName.Key.Replace("\" ", "").Replace(" \"", "")}");

                    Functions.SelectedVehicleModel = modelFromName.Key.Replace("\" ", "").Replace(" \"", "");
                }
            };

            Menu.OnItemSelect += (MenuAPI.Menu Menu, MenuItem MenuItem, int ItemIndex) =>
            {
                string SelectedItem = MenuItem.Text;

                switch (SelectedItem)
                {
                    case var value when value == enableDisableVehicle.Text:
                        RCVehicle.List.Enabled = !RCVehicle.List.Enabled;

                        if (!RCVehicle.List.Enabled)
                        {
                            enableDisableVehicle.Text = "Enable RC Vehicle";
                            enableDisableVehicle.Label = "🟢";
                            manageVehicle.Enabled = false;
                            selectedVehicle.Enabled = true;
                            manageVehicle.Description = "Enable RC Vehicles to access this menu.";
                            _Functions.OnRCVehicleDisable();
                        }
                        else
                        {
                            enableDisableVehicle.Text = "Disable RC Vehicle";
                            enableDisableVehicle.Label = "🔴";
                            manageVehicle.Enabled = true;
                            selectedVehicle.Enabled = false;
                            manageVehicle.Description = "Access the RC Vehicle Management menu.";
                            _Functions.OnRCVehicleEnable();
                        }

                        break;

                    case var value when value == manageVehicle.Text:
                        ManageRCMenu.Menu.OpenMenu();

                        break;
                }
            };
        }
    }
}
