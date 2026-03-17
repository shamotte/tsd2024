using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldSavings.App
{
    internal class RandomList<T>
    {
        private List<T> list;
        private Random random;

        RandomList() { 
            list = new List<T>();
            random = new Random();
        }

        public void Add(T item)
        {
            if(random.Next() % 2 == 0)
                list.Add(item);
            else
                list.Insert(0, item);
        }

        public T Get(int index)
        {
            
            return list[random.Next(0, index)];
        }
        public bool IsEmpty()
        {
            return list.Count == 0; 
        }

    }
}
