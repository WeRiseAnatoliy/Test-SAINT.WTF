using Sirenix.OdinInspector;

namespace TestTask.Common
{
    public abstract class ValueModiferBase : IValueModifer
    {
        public ValueModiferBase()
        {

        }

        public ValueModiferBase(float defaultValue, int position = 0)
        {
            Value = defaultValue;
            Positon = position;
        }

        [ShowInInspector] public float Value { get; set; }
        [ShowInInspector] public int Positon { get; set; }

        public abstract float Calculate(float InputValue);
    }
}