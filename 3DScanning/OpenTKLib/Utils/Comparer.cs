using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKLib
{
    public class KeyValueComparer : IComparer<KeyValuePair<int, float>>
    {

        public int Compare(KeyValuePair<int, float> a, KeyValuePair<int, float> b)
        {

            if (a.Value > b.Value)
                return 1;
            else if (a.Value > b.Value)
                return -1;
            else
                return 0;


        }
    }
    public class DistanceComparer : IComparer<KeyValuePair<OpenTK.Vector3, OpenTK.Vector3>>
    {

        public int Compare(KeyValuePair<OpenTK.Vector3, OpenTK.Vector3> a, KeyValuePair<OpenTK.Vector3, OpenTK.Vector3> b)
        {
            float an = a.Key.NormSquared();
            float bn = a.Key.NormSquared();

            if (an > bn)
                return 1;
            else if (an < bn)
                return -1;
            else
                return 0;

            
        }
    }
    public class NeighboursComparer : IComparer<Neighbours>
    {

        public int Compare(Neighbours a, Neighbours b)
        {

            if (a.Angle > b.Angle)
                return 1;
            else if (a.Angle == b.Angle)
            {
                if (a.Distance > b.Distance)
                {
                    return 1;
                }
                else
                    return -1;
            }

            else
                return -1;

        }
    }
    public class NeighboursListComparer : IComparer<List<Neighbours>>
    {

        public int Compare(List<Neighbours> a, List<Neighbours> b)
        {
            try
            {

                if (a[0].Angle > b[0].Angle)
                    return 1;

                else if (a[0].Angle == b[0].Angle)
                {
                    if (a[0].Distance > b[0].Distance)
                    {
                        return 1;
                    }
                    else
                        return -1;
                }

                else
                    return -1;
            }
            catch(Exception err)
            {
                System.Diagnostics.Debug.WriteLine("Error in sort: " + err.Message);
                return 1;
            }

        }
    }
}
