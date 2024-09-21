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

                Factory.infoPanel.Text.text = $"Production...";
            }
        }

        private void CollectResources ()
        {
            foreach (var item in Factory.InputItems)
            {
                Factory.InputStorage.ContainsItem(item, out var arrayIdx);
                Factory.InputStorage.Items[arrayIdx].Mover.SetTargetPos(Vector3.zero, Factory.transform);
                Object.Destroy(Factory.InputStorage.Items[arrayIdx].ItemObject, 5f);
                Factory.InputStorage.RemoveItem(arrayIdx);
                Factory.InputStorage.RecalculateItemsPosition();
            }
        }

        public override void Update()
        {
            base.Update();

            if(Factory.OutputStorage.IsFull == false)
            {
                Factory.infoPanel.SetProgress(ProductionPercent);
                if (timeFromLastProd >= Factory.ProductionRate.ModifedValue)
                {
                    Factory.SetState(FactoryStateType.ItemProduce);
                }
            }
        }

        public override void ExitState()
        {
            lastProductionTime = 0;
        }
    }
}