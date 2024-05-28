using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace WindowsFormsApp1
{

    public struct VectorAndPosition
    {
        public Point2f Position { get; }

        public Point2f Vector { get; }

        public VectorAndPosition(Point2f position, Point2f vector)
        {
            Position = position;
            Vector = vector;
        }
    }


    public struct VectorSort
    {
        public float Distance { get; set; }
        public int Index { get; set; }

        public VectorSort(float distance, int index)
        {
            Distance = distance;
            Index = index;
        }

        public void Clear()
        {
            Distance = default(float);
            Index = default(int);
        }
    }

}
