using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRage.ObjectBuilders;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRageMath;
using VRage.Game.ModAPI.Network;
using VRage.Network;

namespace ShowcaseBlock
{
    [MyEntityComponentDescriptor(typeof(Sandbox.Common.ObjectBuilders.MyObjectBuilder_InteriorLight), true, new string[] { "DisplayCase1", "DisplayCase2" })]
    public partial class ShowcaseContainer : MyGameLogicComponent, IMyEventProxy
    {
        // using MySync
        // https://discord.com/channels/125011928711036928/126460115204308993/938979913162170478

        private bool init = true;

        private const string DUMMY_SUFFIX = "detector_trophycase_";

        IMyLightingBlock block;
        IMyInventory inventory;

        private List<VRage.Game.ModAPI.Ingame.MyInventoryItem> itemList = new List<VRage.Game.ModAPI.Ingame.MyInventoryItem>();
        private VRage.Game.ModAPI.Ingame.MyInventoryItem first;

        public MySync<UInt16, VRage.Sync.SyncDirection.BothWays> rotationX;
        public MySync<UInt16, VRage.Sync.SyncDirection.BothWays> rotationY;
        public MySync<UInt16, VRage.Sync.SyncDirection.BothWays> rotationZ;
        public readonly ShowcaseBlockSettings Settings = new ShowcaseBlockSettings();
        public Guid SETTINGS_GUID { get; private set; }

        private MatrixD dummyPosition;
        private Matrix displayPosition;
        // for multiple display slots
        //private MyEntity[] model;
        //private MatrixD[] displayPosition;
        private MyEntity model;
        private MyDefinitionId defDisplayed;

        bool isDisplaying;
        int idDisplayed;

        bool IsDedicatedServer { get { return MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated; } }

	// TODO
        private static Guid GetGUID(IMyLightingBlock block)
        {
            Guid defaultGuid = new Guid("63afc52f-2324-473e-b680-a410dc079af0");
            string subTypeName = block?.GetObjectBuilder()?.SubtypeName;
            if (subTypeName == null) { return defaultGuid; }
            switch (subTypeName)
            {
                case "DisplayCase2":
                    return new Guid("2e3ce819-6b2d-426c-996c-20df4d44ce77");
                default:
                    return defaultGuid;
            }
        }
        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            block = (IMyLightingBlock)Entity;
            block.NeedsWorldMatrix = true;
            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }
        private void GetDummies()
        {
            try
            {
                Dictionary<string, IMyModelDummy> dummies = new Dictionary<string, IMyModelDummy>();
                int count = block.Model.GetDummies(dummies);
                if (count > 0)
                {
                    try
                    {
                        IMyModelDummy dummy = dummies.Where(x => x.Key.Contains(DUMMY_SUFFIX)).ElementAt(0).Value;
                        dummyPosition = MatrixD.CreateScale(1f) *
                                        MatrixD.CreateFromQuaternion(Quaternion.CreateFromRotationMatrix(MatrixD.Normalize(dummy.Matrix))) *
                                        MatrixD.CreateTranslation(dummy.Matrix.Translation);
                        displayPosition = dummyPosition;
                    }
                    catch { SimpleLog.Error(this, "dummy containing " + DUMMY_SUFFIX + " not found!: " + dummies.Values); }
                }
            }
            catch { SimpleLog.Error(this, "can't get dummies?"); }
        }
        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();

            // cleanup for repaints
            HideItem();

            if (init)
            {
                // set default settings
                rotationX.Value = (ushort)180;
                rotationY.Value = (ushort)180;
                rotationZ.Value = (ushort)180;
                rotationX.ValueChanged += (_) => Rotate();
                rotationY.ValueChanged += (_) => Rotate();
                rotationZ.ValueChanged += (_) => Rotate();
                
                SETTINGS_GUID = GetGUID(block);
                if(!LoadSettings())
                {
                    ParseLegacyNameStorage();
                }

                SaveSettings(); // required for IsSerialized()
                init = false;

                MyAPIGateway.Session.DamageSystem.RegisterDestroyHandler(1, DestroyHandler);
            }

            try 
            { 
                inventory = block.GetInventory();
                GetDummies();
                // item ids are generated for every session, start at 0
                // and just increment for each new item
                idDisplayed = -1;
                isDisplaying = false;
                model = null;
            }
            catch (Exception e)
            { SimpleLog.Error(this, e); }

            NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
        }

        private void DestroyHandler(object target, MyDamageInformation damageInformation)
        {
            IMySlimBlock slimBlock = target as IMySlimBlock;
            if (slimBlock == null) return;

            IMyTerminalBlock terminalTarget = slimBlock.FatBlock as IMyTerminalBlock;
            if (terminalTarget == null) return;

            ShowcaseContainer container = ShowcaseBlockMod.GetLogic(terminalTarget);
            if (container != null)
            {
                container.DropInventory();
            }
            return;
        }

        public override void UpdateAfterSimulation10()
        {
           if (MyAPIGateway.Session == null) // for startup or ds?
             return;

           if (!block.IsFunctional || !block.IsWorking)
           {
               if (isDisplaying) { HideItem(); }
               return;
           }

           if (inventory.ItemCount > 0)
           {
               first = inventory.GetItemAt(0).Value;

               // already displaying
               if (idDisplayed == first.ItemId)
               { return; }

               // replace
               if (isDisplaying)
                   HideItem();

               bool result = MyDefinitionId.TryParse(first.Type.TypeId,
                                                 first.Type.SubtypeId,
                                                 out defDisplayed);
               if (result)
               {
                   idDisplayed = (int)first.ItemId;
                   isDisplaying = true;
                   UpdateVisible();
               } else
               {
                   SimpleLog.Error(this, "could not parse item definition: " + first.Type);
                   HideItem();
               }
           }
           else
           {
               if ((idDisplayed != -1) || isDisplaying)
               {
                   HideItem();
               }
           }
        }
        private void UpdateVisible()
        {
            if (MyAPIGateway.Session == null)
                return;

            if (IsDedicatedServer)
                return;

            if (isDisplaying)
            {
                ShowItem();
		        Rotate();
            }
        }

        private void HideItem()
        {
            //if (model[id] == null)
            if (model == null)
                return;

            if (block == null)
                return;

            Sandbox.Game.Entities.MyEntities.Remove(model);

            //model[id] = null;
            model = null;
            idDisplayed = -1;
            isDisplaying = false;
        }
        private void ShowItem()
        {
            MyEntity entity = new MyEntity();
            try
            {
                MyPhysicalItemDefinition itemDef = MyDefinitionManager.Static.GetPhysicalItemDefinition(defDisplayed); 
                
                entity.Init(null, itemDef.Model, (MyEntity)block, null, null);
                entity.Save = false;
                entity.SyncFlag = false;
                entity.IsPreview = true;
                entity.NeedsWorldMatrix = true;

                entity.Render.EnableColorMaskHsv = true;
                entity.Render.ColorMaskHsv = block.Render.ColorMaskHsv;
                entity.Render.PersistentFlags = MyPersistentEntityFlags2.CastShadows;
                entity.Flags = EntityFlags.UpdateRender | EntityFlags.NeedsWorldMatrix | EntityFlags.Visible | EntityFlags.NeedsDraw | EntityFlags.NeedsDrawFromParent | EntityFlags.InvalidateOnMove;
                entity.Name = entity.EntityId.ToString();
                entity.DisplayName = block.EntityId.ToString();
                entity.PositionComp.SetLocalMatrix(ref displayPosition);
                Sandbox.Game.Entities.MyEntities.SetEntityName(entity, true);

                Sandbox.Game.Entities.MyEntities.Add(entity, true);

                model = entity;
                //models[id] = entity;
            }
            catch (Exception e) { SimpleLog.Error(this, e); }
        }
        private void Rotate()
        {
            if (model != null)
            {
                // reset
                displayPosition = dummyPosition;
                model.PositionComp.SetLocalMatrix(ref displayPosition, updateWorld: false);

                displayPosition = model.PositionComp.LocalMatrixRef;

                displayPosition = Matrix.Transform(displayPosition, Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationY), MathHelper.ToRadians(rotationX), MathHelper.ToRadians(rotationZ)));
                displayPosition.Translation = dummyPosition.Translation;
                displayPosition = Matrix.Normalize(displayPosition); // normalize to avoid any rotation inaccuracies over time resulting in weird scaling?

                model.PositionComp.SetLocalMatrix(ref displayPosition, source: block);
            }
        }
        private void DropInventory()
        {
            if (!MyAPIGateway.Multiplayer.IsServer)
                return;

            if (inventory == null || inventory.Empty()) 
                return;

            inventory.GetItems(itemList);
            foreach (VRage.Game.ModAPI.Ingame.MyInventoryItem item in itemList)
            {
                inventory.RemoveItems(item.ItemId, amount: item.Amount, spawn: true);
            }
            if (itemList != null)
                itemList.Clear();

            HideItem();
        }
    }
}