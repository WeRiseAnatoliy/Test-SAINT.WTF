namespace TestTask.Buildings
{
    public class OutputStorageFullState : FactoryState
    {
        public OutputStorageFullState(FactoryBuilding factory) : base(factory)
        {

        }

        public override void EnterState()
        {

        }

        public override void ExitState()
        {

        }

        public override void Update()
        {
            base.Update();

            if (Factory.OutputStorage.IsFull == false)
                Factory.SetState(FactoryStateType.WaitingResources);
        }
    }
}