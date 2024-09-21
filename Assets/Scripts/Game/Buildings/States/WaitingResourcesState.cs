using System.Collections.Generic;

namespace TestTask.Buildings
{
    public class WaitingResourcesState : FactoryState
    {
        public WaitingResourcesState(FactoryBuilding factory) : base(factory)
        {

        }

        public override void EnterState()
        {
            Update();
        }

        public override void ExitState()
        {

        }

        public override void Update()
        {
            base.Update();

            if(InputItemsConidition())
            {
                Factory.SetState(FactoryStateType.Production);
            }
            else
            {
                Factory.infoPanel.Text.text = $"Need resources: {string.Join(", ", Factory.InputItems)}";
            }
        }

        private bool InputItemsConidition()
        {
            if (Factory.InputStorage == null || Factory.InputItems.Length == 0)
                return true;

            var usedItemsIdx = new List<int>();
            for (var x = 0; x < Factory.InputItems.Length; x++)
            {
                if (Factory.InputStorage.ContainsItem(Factory.InputItems[x], out var storageArrayIdx, usedItemsIdx.ToArray()))
                {
                    usedItemsIdx.Add(storageArrayIdx);
                }
            }

            return usedItemsIdx.Count == Factory.InputItems.Length;
        }
    }
}