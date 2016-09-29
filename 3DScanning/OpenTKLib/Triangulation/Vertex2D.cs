
using System.Windows;
using MIConvexHull;
using System.Windows.Media;

namespace OpenTKLib
{

    /// <summary>
    /// 
    /// </summary>
    public class Vertex2D : IVector
    {
        //public double[] Position { get; set; }
        public int IndexInModel;
        double[] position = new double[2];

        public Vertex2D(double x, double y)
        {
            position = new double[] { x, y };
        }
        public Vertex2D(int indexInModel, double x, double y)
        {
            position = new double[] { x, y };
            IndexInModel = indexInModel;
        }
        

        public double this[int index]
        {
            get
            {
                return position[index];
            }
            set
            {
                position[index] = value;
            }
        }
        public double[] PositionArray
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }
        
    }
}
