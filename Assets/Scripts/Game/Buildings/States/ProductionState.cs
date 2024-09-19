using UnityEngine;

namespace TestTask.Buildings
{
    public class ProductionState : FactoryState
    {
        public float ProductionPercent =>
            timeFromLastProd / Factory.ProductionRate.ModifedValue;

        private float lastProductionTime;

        private float timeFromLastProd =>
            Time.time - lastProductionTime;

        public ProductionState(FactoryBuilding factory) : base(factory)
        {

        }

        public override void EnterState()
        {
            if (Factory.OutputStorage.IsFull)
            {
                Factory.SetState(FactoryStateType.OutputStorageFull);
            }
            else
            {
                lastProductionTime = Time.time;
                CollectResources();
            }
        }

        private void CollectResources ()
        {
            foreach (var item in Factory.InputItems)
            {
                Factory.InputStorage.ContainsItem(item, out var arrayIdx);
                Factory.InputStorage.RemoveItem(arrayIdx);
            }
        }

        public override void Update()
        {
            base.Update();

            if (Factory.OutputStorage.IsFull == false &&
                timeFromLastProd >= Factory.ProductionRate.ModifedValue)
            {
                Factory.SetState(FactoryStateType.ItemProduce);
            }
        }

        public override void ExitState()
        {
            lastProductionTime = 0;
        }
    }
}