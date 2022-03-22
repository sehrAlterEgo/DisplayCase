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
using VRage.ObjectBuilders;
using VRageMath;

namespace ShowcaseBlock
{
    [MyEntityComponentDescriptor(typeof(Sandbox.Common.ObjectBuilders.MyObjectBuilder_InteriorLight), true, new string[] { "DisplayCase1" })]
    class ShowcaseContainer : MyGameLogicComponent
    {
        private IMyLightingBlock m_block;
        private const string DUMMY_SUFFIX = "detector_inventory_";

        IMyInventory inventory;

        private MatrixD displayPosition;

        // for multiple display slots
        //MyEntity[] model;
        //MatrixD[] displayPosition;
        MyEntity model;
        MyDefinitionId defDisplayed;

        bool isDisplaying;
        int idDisplayed;

        private VRage.Game.ModAPI.Ingame.MyInventoryItem first;

        bool IsDedicatedServer { get { return MyAPIGateway.Multiplayer.IsServer && MyAPIGateway.Utilities.IsDedicated; } }
        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
            m_block = (IMyLightingBlock)Entity;
            m_block.NeedsWorldMatrix = true;
        }

        public override void UpdateOnceBeforeFrame()
        {
            base.UpdateOnceBeforeFrame();
            inventory = m_block.GetInventory();
            GetDummies();
            isDisplaying = false;
            // item ids are generated for every session, start at 0
            // and just increment for each new item
            idDisplayed = -1;
            model = null;
            NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
        }
        public override void UpdateAfterSimulation10()
        {
            if (MyAPIGateway.Session == null) // for startup or ds?
            {
                return;
            }

            if (!m_block.IsFunctional || !m_block.IsWorking)
            {
                SimpleLog.Error(this, "not working/functional");
                return;
            }

            if (inventory.ItemCount > 0)
            {
                first = inventory.GetItemAt(0).Value;
                if (idDisplayed == first.ItemId)  // already displaying
                { return; }

                if (isDisplaying) // replace
                {
                    HideItem(); // idDisplayed);
                }

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
                    HideItem(); // idDisplayed);
                }
            }
            else
            {
                if ((idDisplayed != -1) || isDisplaying)
                {
                    HideItem(); //idDisplayed); }
                }
            }
               
        }
        void GetDummies()
        {
            try
            {
                Dictionary<string, IMyModelDummy> dummies = new Dictionary<string, IMyModelDummy>();
                int count = m_block.Model.GetDummies(dummies);
                if (count > 0)
                {
                    try
                    {
                        IMyModelDummy dummy = dummies.Where(x => x.Key.Contains(DUMMY_SUFFIX)).ElementAt(0).Value;
                        displayPosition = MatrixD.CreateScale(1f) * 
                                        MatrixD.CreateFromQuaternion(Quaternion.CreateFromRotationMatrix(MatrixD.Normalize(dummy.Matrix))) * 
                                        MatrixD.CreateTranslation(dummy.Matrix.Translation);
                    }
                    catch { SimpleLog.Error(this, "dummy containing " + DUMMY_SUFFIX + " not found!: " + dummies.Values); }
                }
            }
            catch { SimpleLog.Error(this, "can't get dummies?"); }
        }

        void UpdateVisible()
        {
            if (MyAPIGateway.Session == null)
                return;

            if (IsDedicatedServer)
                return;

            if (isDisplaying)
            {
                ShowItem();
            }
        }

        void HideItem()
        {
            //if (model[id] == null)
            if (model == null)
                return;

            if (m_block == null)
                return;

            idDisplayed = -1;
            isDisplaying = false;

            //model[id].OnRemovedFromScene(m_block);
            //model[id] = null;
            model.OnRemovedFromScene(m_block);
            model = null;

        }
        void ShowItem()
        {
            MyEntity entity = new MyEntity();

            MyPhysicalItemDefinition itemDef = MyDefinitionManager.Static.GetPhysicalItemDefinition(defDisplayed);
            
            entity.Init(null, itemDef.Model, (MyEntity)m_block, null, null);

            entity.Render.EnableColorMaskHsv = true;
            entity.Render.ColorMaskHsv = m_block.Render.ColorMaskHsv;
            entity.Render.PersistentFlags = MyPersistentEntityFlags2.CastShadows;

            entity.PositionComp.LocalMatrix = displayPosition;

            entity.Flags = EntityFlags.UpdateRender | EntityFlags.NeedsWorldMatrix | EntityFlags.Visible | EntityFlags.NeedsDraw | EntityFlags.NeedsDrawFromParent | EntityFlags.InvalidateOnMove;
            
            entity.Name = entity.EntityId.ToString();
            entity.DisplayName = m_block.EntityId.ToString();
            Sandbox.Game.Entities.MyEntities.SetEntityName(entity, true);

            entity.OnAddedToScene(m_block);

            //models[id] = entity;
            model = entity;
        }

    }
}