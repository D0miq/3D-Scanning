using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLinear;

namespace OpenTKLib
{
  
    public class Polyline<T> : IList<Vector3d<T>> //IEquatable<Polyline<T>>
        where T : IEquatable<T>
    {
        private List<Vector3d<T>> list;
        public Polyline()
        { 
            list = new List<Vector3d<T>>();
        }

        
        public void Add(Vector3d<T> element)
        {
            list.Add(element);
        }
        public int IndexOf(Vector3d<T> element)
        {
            return list.IndexOf(element);

        }
        public void CopyTo(Vector3d<T>[] element, int i)
        {
            list.CopyTo(element, i);

        }
        public bool Remove(Vector3d<T> element)
        {
            return list.Remove(element);

        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public IEnumerator<Vector3d<T>> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
            //throw new NotImplementedException();
        }
                   
        public void Insert(int ind, Vector3d<T> element)
        {
            list.Insert(ind, element);
            
        }
        public void RemoveAt(int ind)
        {
            list.RemoveAt(ind);


        }
        public void Clear()
        {
            list.Clear();


        }
        public bool Contains(Vector3d<T> element)
        {
            return list.Contains(element);


        }
        public int Count
        {
            get

            {
                return list.Count;
            }
         
        }
        public Vector3d<T> this[int i] 
        {
            get
            {
                return list[i];
            }
            set
            {
                list[i] = value;
            }

        }
    }
}
