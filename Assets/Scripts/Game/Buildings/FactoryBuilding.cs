using Sirenix.OdinInspector;
using System.Collections.Generic;
using TestTask.Common;
using TestTask.Game.UI;
using TestTask.UI;

namespace TestTask.Buildings
{
    public class FactoryBuilding : Building
    {

        [FoldoutGroup("Items")] public ValueWithModifers ProductionRate = new ValueWithModifers(2);

        [FoldoutGroup("Debug"), ShowInInspector, ReadOnly] public Dictionary<FactoryStateType, FactoryState> States;
        [FoldoutGroup("Debug"), ShowInInspector, ReadOnly] public FactoryStateType CurrentState;

        internal ObjectInfoPanel infoPanel;

        protected override void Start()
        {
            infoPanel = UIService.Instance.GetComponentInChildren<SimpleGameScreen>().CreatePanel(this);

            base.Start();

            States = new()
            {
                { FactoryStateType.WaitingResources, new WaitingResourcesState(this) },
                { FactoryStateType.Production, new ProductionState(this) },
                { FactoryStateType.ItemProduce, new ItemProduceState(this) },
                { FactoryStateType.OutputStorageFull, new OutputStorageFullState(this) }
            };

            ForceSetState(FactoryStateType.WaitingResources);
        }

        public void SetState (FactoryStateType state)
        {
            if (CurrentState == state)
                return;

            ForceSetState(state);
        }

        public void ForceSetState (FactoryStateType state)
        {
            if(CurrentState != state)
                States[CurrentState].ExitState();
            CurrentState = state;
            States[CurrentState].EnterState();
        }

        protected override void Update()
        {
            base.Update();

            if(States != null && States.Count > 0)
                States[CurrentState].Update();
        }

        private ValueDropdownList<string> items =>
            Items.ItemDatabase.GetItemsListWithEmpty();
    }
}