namespace TestTask.Common
{
    public class MultiplyModifer : ValueModiferBase
    {
        public override float Calculate(float InputValue)
        {
            return InputValue * Value;
        }
    }
}