namespace TestTask.Common
{
    public interface IValueModifer
    {
        public int Positon { get; }
        public float Value { get; }
        public float Calculate(float InputValue);
    }
}