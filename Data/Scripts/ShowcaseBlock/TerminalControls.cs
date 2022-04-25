using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Text;
using System.Collections.Generic;
using VRage.Utils;

namespace ShowcaseBlock
{
    public partial class ShowcaseContainer
    {
        #region Settings
        // https://github.com/THDigi/GravityCollector
        bool LoadSettings()
        {
            if(block.Storage == null)
                return false;

            string rawData;
            if(!block.Storage.TryGetValue(SETTINGS_GUID, out rawData))
                return false;

            try
            {
                var loadedSettings = MyAPIGateway.Utilities.SerializeFromBinary<ShowcaseBlockSettings>(Convert.FromBase64String(rawData));

                if(loadedSettings != null)
                {
                    rotationX.Value = loadedSettings.rotationX;
                    rotationY.Value = loadedSettings.rotationY;
                    rotationZ.Value = loadedSettings.rotationZ;
                    return true;
                }
            }
            catch(Exception e)
            {
                SimpleLog.Error(this, $"Error loading settings!\n{e}");
            }

            return false;
        }

        bool ParseLegacyNameStorage()
        {
            string name = block.CustomName.TrimEnd(' ');

            if(!name.EndsWith("]", StringComparison.Ordinal))
                return false;

            int startIndex = name.IndexOf('[');

            if(startIndex == -1)
                return false;

            var settingsStr = name.Substring(startIndex + 1, name.Length - startIndex - 2);

            if(settingsStr.Length == 0)
                return false;

            string[] args = settingsStr.Split(';');

            if(args.Length == 0)
                return false;

            string[] data;

            foreach(string arg in args)
            {
                data = arg.Split('=');

                ushort f;
                int i;

                if(data.Length == 2)
                {
                    switch(data[0])
                    {
                        case "rotationX":
                            if(ushort.TryParse(data[1], out f))
                                rotationX.Value = f;
                            break;
                        case "rotationY":
                            if(ushort.TryParse(data[1], out f))
                                rotationY.Value = f;
                            break;
                        case "rotationZ":
                            if(ushort.TryParse(data[1], out f))
                                rotationZ.Value = f;
                            break;
                    }
                }
            }

            block.CustomName = name.Substring(0, startIndex).Trim();
            return true;
        }

        void SaveSettings()
        {
            if(block == null)
                return; // called too soon or after it was already closed, ignore

            if(Settings == null)
                throw new NullReferenceException($"Settings == null on entId={Entity?.EntityId};");

            if(MyAPIGateway.Utilities == null)
                throw new NullReferenceException($"MyAPIGateway.Utilities == null; entId={Entity?.EntityId};");

            if(block.Storage == null)
                block.Storage = new Sandbox.Game.EntityComponents.MyModStorageComponent();

            Settings.rotationX = rotationX;
            Settings.rotationY = rotationY;
            Settings.rotationZ = rotationZ;
            block.Storage.SetValue(SETTINGS_GUID, Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(Settings)));
            //block.Storage.Values.Add(Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(Settings)));
        }
        public override bool IsSerialized()
        {
            // called when the game iterates components to check if they should be serialized, before they're actually serialized.
            // this does not only include saving but also streaming and blueprinting.
            // NOTE for this to work reliably the MyModStorageComponent needs to already exist in this block with at least one element.

            try
            {
                SaveSettings();
            }
            catch(Exception e)
            {
                SimpleLog.Error(this, e);
            }

            return base.IsSerialized();
        }
        #endregion

    }
}
