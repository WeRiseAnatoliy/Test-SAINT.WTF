namespace TestTask.Common
{
    public class AddModifer : ValueModiferBase
    {
        public override float Calculate(float InputValue)
        {
            return InputValue + Value;
        }
    }
}