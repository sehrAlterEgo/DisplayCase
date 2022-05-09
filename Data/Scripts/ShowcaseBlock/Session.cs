using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game.Components;
using VRage.Utils;

namespace ShowcaseBlock
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class ShowcaseBlockMod : MySessionComponentBase
    {

        public override void LoadData()
        {
            MyAPIGateway.TerminalControls.CustomControlGetter += CreateTerminalControls;
            MyAPIGateway.TerminalControls.CustomActionGetter += CreateTerminalActions;
        }

        protected override void UnloadData()
        {
            MyAPIGateway.TerminalControls.CustomControlGetter -= CreateTerminalControls;
            MyAPIGateway.TerminalControls.CustomActionGetter -= CreateTerminalActions;
        }

        public static ShowcaseContainer GetLogic(IMyTerminalBlock block) => block?.GameLogic?.GetAs<ShowcaseContainer>();
        static bool ControlEnabled(IMyTerminalBlock block)
        {
            return block.IsWorking && block.IsFunctional;
        }
        static string GetPath(string relativePath, VRage.Game.ModAPI.IMyModContext ModContext = null)
        {
            if(relativePath.StartsWith("\\") || relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1); // remove leading slashes

            if (ModContext != null)
            {
                string modPath = ModContext.ModPath;
                return Path.Combine(modPath, relativePath);
            }
            return Path.Combine(MyAPIGateway.Utilities.GamePaths.ContentPath, relativePath);
        }
        private void CreateTerminalActions(IMyTerminalBlock block, List<IMyTerminalAction> actions)
        {
            if (GetLogic(block) == null)
            {
                return;
            }

            IMyTerminalAction incPitch = MyAPIGateway.TerminalControls.CreateAction<IMyLightingBlock>("increasePitch");
            incPitch.Name = new StringBuilder("increase Pitch");
            incPitch.Enabled = ControlEnabled;
            
            incPitch.Icon = GetPath(@"Textures\GUI\Icons\Actions\Increase.dds");
            incPitch.ValidForGroups = true;
            incPitch.Action = (tb) => 
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    if (b.rotationX.Value <= 350)
                        b.rotationX.Value += (ushort)10;
                    else
                        b.rotationX.Value = 360;
                }


            }; 
            incPitch.Writer = (tb, sb) =>
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    sb.Clear();
                    sb.Append($"Pitch({b.rotationX.Value})");

                }
            };
            actions.Add(incPitch);

            IMyTerminalAction decPitch = MyAPIGateway.TerminalControls.CreateAction<IMyLightingBlock>("decreasePitch");
            decPitch.Name = new StringBuilder("decreasePitch");
            decPitch.Enabled = ControlEnabled;
            decPitch.Icon = GetPath(@"Textures\GUI\Icons\Actions\Decrease.dds");
            decPitch.ValidForGroups = true;
            decPitch.Action = (tb) => 
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    if (b.rotationX.Value >= 10)
                        b.rotationX.Value -= (ushort)10;
                    else
                        b.rotationX.Value = 0;
                }
            }; 
            decPitch.Writer = (tb, sb) =>
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    sb.Clear();
                    sb.Append($"Decrease Pitch({b.rotationX.Value})");

                }
            };
            actions.Add(decPitch);

            IMyTerminalAction incYaw = MyAPIGateway.TerminalControls.CreateAction<IMyLightingBlock>("increaseYaw");
            incYaw.Name = new StringBuilder("increaseYaw");
            incYaw.Enabled = ControlEnabled;
            incYaw.Icon = GetPath(@"Textures\GUI\Icons\Actions\Increase.dds");
            incYaw.ValidForGroups = true;
            incYaw.Action = (tb) => 
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    if (b.rotationY.Value <= 350)
                        b.rotationY.Value += (ushort)10;
                    else
                        b.rotationY.Value = 360;
                }


            }; 
            incYaw.Writer = (tb, sb) =>
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    sb.Clear();
                    sb.Append($"Increase Yaw({b.rotationY.Value})");

                }
            };
            actions.Add(incYaw);

            IMyTerminalAction decYaw = MyAPIGateway.TerminalControls.CreateAction<IMyLightingBlock>("decreaseYaw");
            decYaw.Name = new StringBuilder("decreaseYaw");
            decYaw.Enabled = ControlEnabled;
            decYaw.Icon = GetPath(@"Textures\GUI\Icons\Actions\Decrease.dds");
            decYaw.ValidForGroups = true;
            decYaw.Action = (tb) => 
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    if (b.rotationY.Value >= 10)
                        b.rotationY.Value -= (ushort)10;
                    else
                        b.rotationY.Value = 0;
                }
            }; 
            decYaw.Writer = (tb, sb) =>
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    sb.Clear();
                    sb.Append($"Decrease Yaw({b.rotationY.Value})");

                }
            };
            actions.Add(decYaw);

            IMyTerminalAction incRoll = MyAPIGateway.TerminalControls.CreateAction<IMyLightingBlock>("increaseRoll");
            incRoll.Name = new StringBuilder("increaseRoll");
            incRoll.Enabled = ControlEnabled;
            incRoll.Icon = GetPath(@"Textures\GUI\Icons\Actions\Increase.dds");
            incRoll.ValidForGroups = true;
            incRoll.Action = (tb) => 
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    if (b.rotationX.Value <= 350)
                        b.rotationX.Value += (ushort)10;
                    else
                        b.rotationX.Value = 360;
                }


            }; 
            incRoll.Writer = (tb, sb) =>
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    sb.Clear();
                    sb.Append($"Increase Roll({b.rotationX.Value})");

                }
            };
            actions.Add(incRoll);

            IMyTerminalAction decRoll = MyAPIGateway.TerminalControls.CreateAction<IMyLightingBlock>("decreaseRoll");
            decRoll.Name = new StringBuilder("decreaseRoll");
            decRoll.Enabled = ControlEnabled;
            decRoll.Icon = GetPath(@"Textures\GUI\Icons\Actions\Decrease.dds");
            decRoll.ValidForGroups = true;
            decRoll.Action = (tb) => 
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    if (b.rotationX.Value >= 10)
                        b.rotationX.Value -= (ushort)10;
                    else
                        b.rotationX.Value = 0;
                }
            }; 
            decRoll.Writer = (tb, sb) =>
            {
                var b = GetLogic(tb);
                if (b != null)
                {
                    sb.Clear();
                    sb.Append($"Decrease Roll({b.rotationX.Value})");

                }
            };
            actions.Add(decRoll);
        }
        private void CreateTerminalControls(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (GetLogic(block) == null)
            {
                return;
            }

            IMyTerminalControlSlider pitch = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyLightingBlock>("Pitch");
            pitch.Title = MyStringId.GetOrCompute("Pitch");
            pitch.Enabled = ControlEnabled;
            pitch.Visible = (_) => true;
            pitch.SetLimits(-180, 180);
            pitch.Getter = (tb) => {
                var b = GetLogic(tb);
                return (b == null ? 0 : b.rotationX - 180); 
            };
            pitch.Setter = (tb, value) => {
                var b = GetLogic(tb);
                if(b != null)
                    b.rotationX.Value = (ushort)(value + 180);
            };
            pitch.Writer = (tb, sb) => { 
                var b = GetLogic(block);
                if(b != null)
                    sb.Append((b.rotationX - 180));
            };
            pitch.SupportsMultipleBlocks = true;
            controls.Add(pitch);

            IMyTerminalControlSlider yaw = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyLightingBlock>("rotationY");
            yaw.Title = MyStringId.GetOrCompute("Yaw");
            yaw.Enabled = (b) => b.IsWorking && b.IsFunctional;
            yaw.Visible = (_) => true;
            yaw.SetLimits(-180, 180);
            yaw.Getter = (tb) => {
                var b = GetLogic(tb);
                return (b == null ? 0 : b.rotationY - 180); 
            };
            yaw.Setter = (tb, value) => {
                var b = GetLogic(tb);
                if(b != null)
                    b.rotationY.Value = (ushort)(value + 180);
            };
            yaw.Writer = (tb, sb) => { 
                var b = GetLogic(block);
                if(b != null)
                    sb.Append((b.rotationY - 180));
            };
            yaw.SupportsMultipleBlocks = true;
            controls.Add(yaw);

            IMyTerminalControlSlider roll = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyLightingBlock>("rotationZ");
            roll.Title = MyStringId.GetOrCompute("Roll");
            roll.Enabled = (b) => b.IsWorking && b.IsFunctional;
            roll.Visible = (_) => true;
            roll.SetLimits(-180, 180);
            roll.Getter = (tb) => {
                var b = GetLogic(tb);
                return (b == null ? 0 : b.rotationZ - 180); 
            };
            roll.Setter = (tb, value) => {
                var b = GetLogic(tb);
                if(b != null)
                    b.rotationZ.Value = (ushort)(value + 180);
            };
            roll.Writer = (tb, sb) => { 
                var b = GetLogic(block);
                if(b != null)
                    sb.Append((b.rotationZ - 180));
            };
            roll.SupportsMultipleBlocks = true;
            controls.Add(roll);
        }        		
    }

}
