using System.Collections.Generic;
using System.Linq;

namespace TestTask.Common
{
    [System.Serializable]
    public class ValueWithModifers
    {
        public ValueWithModifers(float realValue, params IValueModifer[] modifers) : this(realValue)
        {
            Modifers = modifers.ToList();
        }

        public ValueWithModifers(float realValue)
        {
            RealValue = realValue;
        }

        public float RealValue;

        public float ModifedValue
        {
            get
            {
                var result = RealValue;
                for(var x = 0; x < Modifers.Count; x++)
                    result = Modifers[x].Calculate(result);
                return result;
            }
        }

        public List<IValueModifer> Modifers = 
            new List<IValueModifer>();

        public void SortModifers ()
        {
            Modifers = Modifers.OrderBy((modifer) => modifer.Positon).ToList();
        }

        public void AddAndSort (IValueModifer modifer)
        {
            Modifers.Add(modifer);
            SortModifers();
        }
    }
}