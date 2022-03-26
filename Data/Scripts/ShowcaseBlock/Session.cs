using System.Collections.Generic;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Utils;

namespace ShowcaseBlock
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class ShowcaseBlockMod : MySessionComponentBase
    {
        public static ShowcaseBlockMod Instance;
        //public List<MyEntity> Entities = new List<MyEntity>();

        public bool ControlsCreated = false;

        // public override void SaveData()
        // {
        //     // executed AFTER world was saved
        // }

        public override void LoadData()
        {
            Instance = this;
        }

        protected override void UnloadData()
        {
            Instance = null;
        }

    }

}
