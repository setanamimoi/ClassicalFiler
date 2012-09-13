using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassicalFiler
{
    public class ChainItem<T>
    {
        public ChainItem(T current)
        {
            this.CurrentItem = current;
        }

        public T CurrentItem
        {
            get;
            set;
        }
        public ChainItem<T> NextItem
        {
            get;
            set;
        }
        public ChainItem<T> PreviousItem
        {
            get;
            set;
        }
    }
}
