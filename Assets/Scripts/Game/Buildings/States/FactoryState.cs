namespace TestTask.Buildings
{
    public abstract class FactoryState
    {
        public FactoryBuilding Factory;

        protected FactoryState(FactoryBuilding factory)
        {
            Factory = factory;
        }

        public abstract void EnterState();
        public abstract void ExitState();

        //Can be replaced with DI (ITickable)
        public virtual void Update ()
        {

        }
    }
}