using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

using RoxRemoteVehicle.Client.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;

namespace RoxRemoteVehicle.Client
{
    public class Functions : BaseScript
    {
        public enum Controls
        {
            RemoteControl,
            Player
        }

        public static string SelectedVehicleModel { get; set; } = "rcbandito";
        public static Dictionary<string, string> AllVehicleModels { get; set; } = new Dictionary<string, string>();

        public static bool EnableSFX { get; set; } = true;

        public bool IsPlayingCarryAnim { get; set; } = false;
        public bool IsPlayingRemoteAnim { get; set; } = false;

        public bool LostConnectionToRC { get; set; } = false;

        public static int ControllerObj = 0;

        public async void OnRCVehicleEnable()
        {
            uint RCSpawnHash = (uint)GetHashKey(SelectedVehicleModel);

            if (!IsModelAVehicle(RCSpawnHash)) return;

            if (!IsModelInCdimage(RCSpawnHash)) return;

            RequestModel(RCSpawnHash);

            while (!HasModelLoaded(RCSpawnHash))
            {
                await Delay(0);
            }

            Vector3 PlayerPos = Game.Player.Character.Position;
            float PlayerHeading = Game.Player.Character.Heading;

            RCVehicle.List.Vehicle = new Vehicle(CreateVehicle(RCSpawnHash, PlayerPos.X, PlayerPos.Y + 0.100f, PlayerPos.Z, PlayerHeading, true, false))
            {
                IsPersistent = true,
                LockStatus = VehicleLockStatus.CannotBeTriedToEnter,
                CanTiresBurst = false,
                CanWheelsBreak = false
            };

            SwitchControls(Controls.Player);

            StartCarryingAnimation();

            RCVehicle.List.Vehicle.AttachTo(Game.Player.Character, new Vector3(0, 0.500f, 0.100f), new Vector3(0, 0, 90f));
            RCVehicle.List.IsHoldingVehicle = true;

            RCVehicle.List.Enabled = true;
        }

        public async void SwitchControls(Controls control)
        {
            RCVehicle.List.Controls = control;

            if (control == Controls.Player)
            {
                if (EnableSFX)
                {
                    Effects.Stop();
                    await Delay(50);
                }

                RenderScriptCams(false, true, 1000, false, false);
                FreezeEntityPosition(Game.Player.Character.Handle, false);
                SetNuiFocusKeepInput(false);
                Game.Player.CanControlCharacter = true;
            }
            else
            {
                RenderScriptCams(true, true, 1000, false, false);
                FreezeEntityPosition(Game.Player.Character.Handle, true);
                SetNuiFocusKeepInput(true);
                Game.Player.CanControlCharacter = false;

                if (EnableSFX)
                {
                    await Delay(300);
                    Effects.Start(ScreenEffect.DeathFailMpDark, 0, true);
                }
            }
        }

        public void PickUpRCVehicle()
        {
            RCVehicle.List.VehicleCam.Detach();
            RCVehicle.List.VehicleCam.Delete();

            RCVehicle.List.Controls = Controls.Player;
            RCVehicle.List.VehicleCam = new Camera(0);
            RCVehicle.List.VehiclePos = new Vector3(0, 0, 0);

            SwitchControls(Controls.Player);

            StopRemoteControlAnimation();

            StartCarryingAnimation();

            RCVehicle.List.Vehicle.AttachTo(Game.Player.Character, new Vector3(0, 0.500f, 0.100f), new Vector3(0, 0, 90f));
            RCVehicle.List.IsHoldingVehicle = true;
        }

        public async void PlaceDownRCVehicle()
        {
            StopCarryingAnimation();

            RCVehicle.List.Vehicle.Detach();

            RCVehicle.List.Vehicle.Position = Game.Player.Character.GetOffsetPosition(new Vector3(0f, 1f, 0f));

            Camera VehicleCam = new Camera(CreateCam("DEFAULT_SCRIPTED_CAMERA", true));

            VehicleCam.PointAt(RCVehicle.List.Vehicle);
            VehicleCam.AttachTo(RCVehicle.List.Vehicle, new Vector3(0, -2.0f, 1f));

            RCVehicle.List.Controls = Controls.Player;
            RCVehicle.List.VehicleCam = VehicleCam;
            RCVehicle.List.VehiclePos = RCVehicle.List.Vehicle.Position;

            SetVehicleOnGroundProperly(RCVehicle.List.Vehicle.Handle);

            SetVehicleEngineOn(RCVehicle.List.Vehicle.Handle, true, true, false);
            SetEntityAsMissionEntity(RCVehicle.List.Vehicle.Handle, true, true);
            SetVehicleHasBeenOwnedByPlayer(RCVehicle.List.Vehicle.Handle, true);
            SetVehicleModKit(RCVehicle.List.Vehicle.Handle, 0);
            ToggleVehicleMod(RCVehicle.List.Vehicle.Handle, 14, true);
            SetVehicleMod(RCVehicle.List.Vehicle.Handle, 14, 21, false);

            await Delay(500);

            StartRemoteControlAnimation();
        }

        public async void StartRemoteControlAnimation()
        {
            if (!HasAnimDictLoaded("stand_controller@dad"))
            {
                RequestAnimDict("stand_controller@dad");

                while (!HasAnimDictLoaded("stand_controller@dad"))
                {
                    await Delay(100);
                }
            }

            Model controller = new Model("prop_controller_01");

            if (!HasModelLoaded(controller))
            {
                RequestModel(controller);

                while (!HasModelLoaded(controller))
                {
                    await Delay(100);
                }
            }

            ControllerObj = CreateObject(controller, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, true, true, true);

            AttachEntityToEntity(ControllerObj, PlayerPedId(), GetPedBoneIndex(PlayerPedId(), 18905), 0.15f, 0.02f, 0.09f, -136.30f, -54.8f, 5.4f, true, true, false, true, 1, true);
            SetModelAsNoLongerNeeded(controller);

            Game.Player.Character.Task.PlayAnimation("stand_controller@dad", "stand_controller_clip", 3.0f, -1, AnimationFlags.Loop | (AnimationFlags)51);
        }

        public void StopRemoteControlAnimation()
        {
            Game.Player.Character.Task.ClearAnimation("stand_controller@dad", "stand_controller_clip");

            SetEntityAsMissionEntity(ControllerObj, false, false);
            DeleteEntity(ref ControllerObj);
        }

        public async void StartCarryingAnimation()
        {
            if (!HasAnimDictLoaded("anim@heists@box_carry@"))
            {
                RequestAnimDict("anim@heists@box_carry@");

                while (!HasAnimDictLoaded("anim@heists@box_carry@"))
                {
                    await Delay(100);
                }
            }

            Game.Player.Character.Task.PlayAnimation("anim@heists@box_carry@", "idle", 5f, -1, AnimationFlags.Loop | (AnimationFlags)51);
        }

        public void StopCarryingAnimation()
        {
            Game.Player.Character.Task.ClearAnimation("anim@heists@box_carry@", "idle");
            RCVehicle.List.IsHoldingVehicle = false;
        }

        public void OnRCVehicleDisable()
        {
            SwitchControls(Controls.Player);

            if (RCVehicle.List.Controls == Controls.RemoteControl)
            {
                RCVehicle.List.VehicleCam.Detach();
                RCVehicle.List.VehicleCam.Delete();
            }

            RCVehicle.List.Vehicle.Delete();

            RCVehicle.List.Controls = Controls.Player;
            RCVehicle.List.Vehicle = new Vehicle(0);
            RCVehicle.List.IsHoldingVehicle = false;
            RCVehicle.List.VehicleCam = new Camera(0);
            RCVehicle.List.VehiclePos = new Vector3(0, 0, 0);
            RCVehicle.List.Enabled = false;

            StopCarryingAnimation();
            StopRemoteControlAnimation();
        }

        public void HasLostConnectionToRC(bool connectionLost)
        {
            if (connectionLost)
            {
                if (!LostConnectionToRC)
                {
                    LostConnectionToRC = true;

                    if (EnableSFX)
                    {
                        Effects.Start(ScreenEffect.DeathFailMpIn, 0, true);
                    }

                    TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 6, 2500);
                }
            }
            else
            {
                if (LostConnectionToRC)
                {
                    LostConnectionToRC = false;

                    if (EnableSFX)
                    {
                        Effects.Stop(ScreenEffect.DeathFailMpIn);
                    }
                }
            }
        }

        public async Task OnRemoteControlVehicleTick()
        {
            if (RCVehicle.List.Enabled && RCVehicle.List.IsHoldingVehicle)
            {
                if (Game.IsControlJustPressed(0, Control.Pickup) && GetLastInputMethod(0))
                {
                    PlaceDownRCVehicle();
                }

                int scaleform = await DrawScaleformIntructionalBtn("down");
                DrawScaleformMovieFullscreen(scaleform, 255, 255, 255, 255, 0);

                RCVehicle.List.Vehicle.AreLightsOn = false;
            }
            else if (RCVehicle.List.Enabled && !RCVehicle.List.IsHoldingVehicle)
            {
                //ForceVehicleEngineAudio(RCVehicle.List.Vehicle.Handle, null);
                RCVehicle.List.Vehicle.AreLightsOn = false;

                if (RCVehicle.List.Controls == Controls.RemoteControl)
                {
                    if (!NetworkHasControlOfEntity(RCVehicle.List.Vehicle.Handle))
                    {
                        NetworkRequestControlOfEntity(RCVehicle.List.Vehicle.Handle);
                    }

                    DisableControlAction(0, 71, true);
                    DisableControlAction(0, 72, true);
                    DisableControlAction(0, 63, true);
                    DisableControlAction(0, 64, true);
                    DisableControlAction(0, 86, true);
                    DisablePlayerFiring(Game.Player.Handle, true);

                    if (RCVehicle.List.Vehicle.IsInRangeOf(Game.Player.Character.Position, 50f))
                    {
                        HasLostConnectionToRC(false);

                        if (!RCVehicle.List.Vehicle.IsInRangeOf(Game.Player.Character.Position, 25f))
                        {
                            ShowSubtitle("RC Signal Strength: ~o~Poor~o~");
                        }
                        else
                        {
                            ShowSubtitle("RC Signal Strength: ~g~Good~g~");
                        }

                        if (IsDisabledControlPressed(0, 86))
                        {
                            SoundVehicleHornThisFrame(RCVehicle.List.Vehicle.Handle);
                            //RCVehicle.List.Vehicle.SoundHorn(1000);
                        }

                        // Forward
                        if (IsDisabledControlPressed(0, 71) && !IsDisabledControlPressed(0, 72))
                        {
                            if (IsDisabledControlPressed(0, 63))
                            {
                                // Forward Left
                                TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 7, 1);
                            }
                            else if (IsDisabledControlPressed(0, 64))
                            {
                                // Forward Right
                                TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 8, 1);
                            }
                            else
                            {
                                // Forward Straight
                                TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 9, 1);
                            }
                        }
                        else if (IsDisabledControlJustReleased(0, 71))
                        {
                            TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 6, 2500);
                        }
                        // Backwards
                        else if (IsDisabledControlPressed(0, 72) && !IsDisabledControlPressed(0, 71))
                        {
                            if (IsDisabledControlPressed(0, 63))
                            {
                                // Backwards Left
                                TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 13, 1);
                            }
                            else if (IsDisabledControlPressed(0, 64))
                            {
                                // Backwards Right
                                TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 14, 1);
                            }
                            else
                            {
                                // Backwards Straight
                                TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 22, 1);
                            }
                        }
                        else if (IsDisabledControlJustReleased(0, 72))
                        {
                            TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 6, 2500);
                        }
                        else if (IsDisabledControlPressed(0, 71) && IsDisabledControlJustReleased(0, 72))
                        {
                            TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 30, 100);
                        }
                        // Left/Right Only
                        else if (IsDisabledControlPressed(0, 63) && !IsDisabledControlJustReleased(0, 71) && !IsDisabledControlJustReleased(0, 72))
                        {
                            TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 4, 1);
                        }
                        else if (IsDisabledControlPressed(0, 64) && !IsDisabledControlJustReleased(0, 71) && !IsDisabledControlJustReleased(0, 72))
                        {
                            TaskVehicleTempAction(PlayerPedId(), RCVehicle.List.Vehicle.Handle, 5, 1);
                        }
                    }
                    else
                    {
                        HasLostConnectionToRC(true);
                        ShowSubtitle("RC Signal Strength: ~r~Lost Connection~r~");
                    }
                }
                else
                {
                    DisableControlAction(0, 71, false);
                    DisableControlAction(0, 72, false);
                    DisableControlAction(0, 63, false);
                    DisableControlAction(0, 64, false);
                    DisableControlAction(0, 86, false);

                    if (RCVehicle.List.Vehicle.IsInRangeOf(Game.Player.Character.Position, 2f))
                    {
                        int scaleform = await DrawScaleformIntructionalBtn("up");
                        DrawScaleformMovieFullscreen(scaleform, 255, 255, 255, 255, 0);

                        if (Game.IsControlJustPressed(0, Control.Pickup) && GetLastInputMethod(0))
                        {
                            PickUpRCVehicle();
                        }
                    }
                }
            }

            await Task.FromResult(0);
        }

        public async Task<int> DrawScaleformIntructionalBtn(string action)
        {
            var scaleform = RequestScaleformMovie("instructional_buttons");

            while (!HasScaleformMovieLoaded(scaleform))
            {
                await Delay(0);
            }

            DrawScaleformMovieFullscreen(scaleform, 255, 255, 255, 0, 0);

            PushScaleformMovieFunction(scaleform, "CLEAR_ALL");
            PopScaleformMovieFunctionVoid();


            PushScaleformMovieFunction(scaleform, "SET_CLEAR_SPACE");
            PushScaleformMovieFunctionParameterInt(200);
            PopScaleformMovieFunctionVoid();

            PushScaleformMovieFunction(scaleform, "SET_DATA_SLOT");
            PushScaleformMovieFunctionParameterInt(0);
            N_0xe83a3e3557a56640(GetControlInstructionalButton(2, 38, 1));
            BeginTextCommandScaleformString("STRING");

            if (action == "down")
            {
                AddTextComponentScaleform("Place Down RC");
            }
            else if (action == "up")
            {
                AddTextComponentScaleform("Pick Up RC");
            }

            EndTextCommandScaleformString();
            PopScaleformMovieFunctionVoid();

            PushScaleformMovieFunction(scaleform, "DRAW_INSTRUCTIONAL_BUTTONS");
            PopScaleformMovieFunctionVoid();

            PushScaleformMovieFunction(scaleform, "SET_BACKGROUND_COLOUR");
            PushScaleformMovieFunctionParameterInt(0);
            PushScaleformMovieFunctionParameterInt(0);
            PushScaleformMovieFunctionParameterInt(0);
            PushScaleformMovieFunctionParameterInt(80);
            PopScaleformMovieFunctionVoid();

            return scaleform;
        }
    }
}
