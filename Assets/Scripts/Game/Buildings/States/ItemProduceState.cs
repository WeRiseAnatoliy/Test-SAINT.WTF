namespace TestTask.Buildings
{
    public class ItemProduceState : FactoryState
    {
        public ItemProduceState(FactoryBuilding factory) : base(factory)
        {

        }

        public override void EnterState()
        {
            Factory.OutputStrorage.AddItem(Factory.OutputItem);

            if (Factory.OutputStrorage.IsFull == false)
                Factory.SetState(FactoryStateType.WaitingResources);
            else
                Factory.SetState(FactoryStateType.OutputStorageFull);
        }

        public override void ExitState()
        {

        }
    }
}