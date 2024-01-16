using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crosses
{
    class CircleQueue<T>
    {
        private int size;
        private T[] list;
        private int ptr;

        public CircleQueue(int size) {
            this.size = size;
            list = new T[size];
            ptr = 0;
        }

        public void Add(T item) {
            list[ptr] = item;
            ptr = (ptr + 1) % size;
        }

        public IEnumerable<T> Iter() {
            for (int i = 0; i < size; i++)
            {
                int index = (i + ptr) % size;
                if (list[index] != null)
                    yield return list[(i + ptr) % size];
            }
        }

        public int Length {
            get {
                return size;
            }
        }
    }
}
