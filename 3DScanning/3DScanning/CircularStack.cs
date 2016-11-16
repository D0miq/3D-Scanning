using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DScanning
{
    class CircularStack<R>
    {
        private R[] array;
        private int top = 0;
        private static int maxLength;
        private bool isFull = false;

        public R[] IterableData
        {
            get
            {
                return this.array;
            }
        }

        public int Count
        {
            get
            {
                if(isFull) { return maxLength; }
                else { return top; }
            }
        }

        public CircularStack()
        {
            maxLength = Utility.MAX_INTERPOLATION;
            array = new R[maxLength];
        }

        private CircularStack(int length)
        {
            if (length <= maxLength) {
                array = new R[length]; }
        }

        public bool IsEmpty()
        {
            return (top == 0) && (!isFull);
        }

        public void Push(R item)
        {
            array[top++] = item;
            top = top % maxLength;
            if (top == 0)
            {           
                this.isFull = true;
            }
        }

        public R Last()
        {
            if(top == 0)
            {
                return array[maxLength - 1];
            }
            return array[top-1];
        }

        public CircularStack<R> GetRange(int numberOfItems)
        {
            if(!isFull && top < numberOfItems)
            {
                return null;
            }
            CircularStack<R> temp = new CircularStack<R>(numberOfItems);
            int index = top;
            for(int i = 0; i<numberOfItems; i++)
            {
                if(index - i == 0)
                {
                    index = maxLength + i;
                }
                temp.Push(array[index - i - 1]);
            }
            return temp;
        }

        public void Clear()
        {
            top = 0;
            isFull = false;
        }
    }
}
