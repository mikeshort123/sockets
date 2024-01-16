using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crosses
{
    class ThreadSafeList<T>
    {
        private List<T> list;

        public ThreadSafeList() {
            list = new List<T>();
        }

        public void Add(T item) {
            lock (list) {
                list.Add(item);
            }
        }

        public void Remove(T item) {
            lock (list) {
                list.Remove(item);
            }
        }

        public void Map(Action<T> a) {
            lock (list) {
                foreach (T item in list) {
                    a(item);
                }
            }
        }
    }
}
