using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassicalFiler
{
    public class ChainList<T>
    {
        private ChainItem<T> CurrentItem
        {
            get;
            set;
        }

        public T Current
        {
            get
            {
                return this.CurrentItem.CurrentItem;
            }
        }

        public void Push(T pushItem)
        {
            ChainItem<T> next = new ChainItem<T>(pushItem);

            if (this.CurrentItem != null)
            {
                this.CurrentItem.NextItem = next;
            }
            next.PreviousItem = this.CurrentItem;

            this.CurrentItem = next;
        }
        public bool MovePrevious()
        {
            if (this.CurrentItem.PreviousItem == null)
            {
                return false;
            }

            this.CurrentItem = this.CurrentItem.PreviousItem;

            return true;
        }
    }
}
