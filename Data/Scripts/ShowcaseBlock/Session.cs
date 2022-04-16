using System.Collections.Generic;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Game.Components;
using VRage.Utils;

namespace ShowcaseBlock
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class ShowcaseBlockMod : MySessionComponentBase
    {
        //public static ShowcaseBlockMod Instance;
        //public bool ControlsCreated = false;

        public override void BeforeStart()
        {
            // init terminal controls
            MyAPIGateway.TerminalControls.CustomControlGetter += CreateTerminalControls;
        }
        static ShowcaseContainer GetLogic(IMyTerminalBlock block) => block?.GameLogic?.GetAs<ShowcaseContainer>();
        private void CreateTerminalControls(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (GetLogic(block) == null)
            {
                return;
            }

            IMyTerminalControlSlider pitch = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyLightingBlock>("rotationX");
            pitch.Title = MyStringId.GetOrCompute("Pitch");
            pitch.Enabled = (b) => b.IsWorking && b.IsFunctional;
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
        public override void LoadData()
        {
            //Instance = this;
        }

        protected override void UnloadData()
        {
            //Instance = null;
            MyAPIGateway.TerminalControls.CustomControlGetter -= CreateTerminalControls;
        }
        		
    }

}
