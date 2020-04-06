using System;
using System.Text;

namespace MatrixTransformations
{
    public class Vector
    {
        public float x, y, z, w;

        public Vector()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 1;
        }

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 1;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            Vector retVector = new Vector();
            retVector.x = v1.x + v2.x;
            retVector.y = v1.y + v2.y;
            retVector.y = v1.z + v2.z;

            return retVector;
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            Vector retVector = new Vector
            {
                x = v1.x - v2.x,
                y = v1.y - v2.y,
                z = v1.z - v2.z,
            };

            return retVector;
        }

        public static Vector operator /(Vector v1, Vector v2)
        {

            if(v2.x == 0 || v2.y == 0)
            {
                throw new DivideByZeroException();
            }

            Vector retVector = new Vector();
            retVector.x = v1.x / v2.x;
            retVector.y = v1.y / v2.y;
            retVector.z = v1.z / v2.z;

            return retVector;

        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            Vector retVector = new Vector();
            retVector.x = v1.x * v2.x;
            retVector.y = v1.y * v2.y;
            retVector.z = v1.z * v2.z;

            return retVector;
        }


        public override string ToString()
        {
            return "X:" + x + ", Y:" + y + ", Z:" + z + ", W: " + w;
        }
    }
}
