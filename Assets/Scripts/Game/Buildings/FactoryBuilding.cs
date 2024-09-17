using Sirenix.OdinInspector;
using System.Collections.Generic;
using TestTask.Common;

namespace TestTask.Buildings
{
    public class FactoryBuilding : Building
    {
        [FoldoutGroup("Items"), ValueDropdown(nameof(items))] public string[] InputItems;
        [FoldoutGroup("Items"), ValueDropdown(nameof(items))] public string OutputItem;

        [FoldoutGroup("Items")] public ValueWithModifers ProductionRate = new ValueWithModifers(2);

        [FoldoutGroup("Debug"), ShowInInspector, ReadOnly] public Dictionary<FactoryStateType, FactoryState> States;
        [FoldoutGroup("Debug"), ShowInInspector, ReadOnly] public FactoryStateType CurrentState;

        private void Start()
        {
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

        private void Update()
        {
            if(States != null && States.Count > 0)
                States[CurrentState].Update();
        }

        private ValueDropdownList<string> items =>
            Items.ItemDatabase.GetItemsListWithEmpty();
    }
}