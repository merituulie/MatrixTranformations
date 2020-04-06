using System;
using System.Text;

namespace MatrixTransformations
{
    public class Matrix
    {
        float[,] mat = new float[4, 4];

        public Matrix() 
        {
            this.mat = Identity().mat;
        }

        public Matrix(float m11, float m12,
                      float m21, float m22)
        {
            mat[0, 0] = m11; mat[0, 1] = m12;
            mat[1, 0] = m21; mat[1, 1] = m22;
        }

        public Matrix(float m11, float m12, float m13,
                      float m21, float m22, float m23,
                      float m31, float m32, float m33)
        {
            mat[0, 0] = m11; mat[0, 1] = m12; mat[0, 2] = m13;

            mat[1, 0] = m21; mat[1, 1] = m22; mat[1, 2] = m23;
            
            mat[2, 0] = m31; mat[2, 1] = m32; mat[2, 2] = m33;
        }

        public Matrix(float m11, float m12, float m13, float m14,
              float m21, float m22, float m23, float m24,
              float m31, float m32, float m33, float m34,
              float m41, float m42, float m43, float m44)
        {
            mat[0, 0] = m11; mat[0, 1] = m12; mat[0, 2] = m13; mat[0, 3] = m14;

            mat[1, 0] = m21; mat[1, 1] = m22; mat[1, 2] = m23; mat[1, 3] = m24;

            mat[2, 0] = m31; mat[2, 1] = m32; mat[2, 2] = m33; mat[2, 3] = m34;
            
            mat[3, 0] = m41; mat[3, 1] = m42; mat[3, 2] = m43; mat[3, 3] = m44;
        }

        public Matrix(Vector v)
        {
            mat[0, 0] = v.x;
            mat[1, 0] = v.y;
            mat[2, 0] = v.z;
            mat[3, 0] = 1;
        }

        public Vector ToVector() {

            return new Vector(mat[0, 0], mat[1, 0], mat[2, 0]);
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            Matrix m3 = new Matrix();

            for (int i = 0; i < m1.mat.GetLength(0); i++) {

                for (int j=0; j < m1.mat.GetLength(1); j++) {
                
                    m3.mat[i, j] = m1.mat[i, j] + m2.mat[i, j];
                
                }

            }

            return m3;
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            Matrix m3 = new Matrix();

            for (int i = 0; i < m1.mat.GetLength(0); i++) {

                for (int j=0; j < m1.mat.GetLength(1); j++) {

                    m3.mat[i, j] = m1.mat[i, j] - m2.mat[i, j];
                }

            }

            return m3;
        }

        public static Matrix operator *(Matrix m1, float f)
        {
            Matrix m3 = new Matrix();

            for (int i = 0; i < m1.mat.GetLength(0); i++) {

                for (int j=0; j < m1.mat.GetLength(1); j++) {

                    m3.mat[i, j] = m1.mat[i, j] * f;

                }

            }

            return m3;

        }

        public static Matrix operator *(float f, Matrix m1)
        {

            return m1 * f;

        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix m3 = new Matrix();

            for (int i=0; i < m1.mat.GetLength(0); i++) {

                for(int j= 0; j < m1.mat.GetLength(1); j++) {
                    
                    float sum = 0;

                    for (int k=0; k < m2.mat.GetLength(0); k++) {

                        sum += m1.mat[i, k] * m2.mat[k, j];
                        
                    }

                    m3.mat[i, j] = sum;
                }
            }

            return m3;
        }

        public static Vector operator *(Matrix m1, Vector v)
        {
            return (m1 * new Matrix(v)).ToVector();
        }

        public static Vector operator *(Vector v, Matrix m1)
        {
            return (m1 * new Matrix(v)).ToVector();
        }

        public static Matrix Identity()
        {
            return new Matrix(1, 0, 0, 0,
                                0, 1, 0, 0,
                                 0, 0, 1, 0,
                                  0, 0, 0, 1);
        }

        public static Matrix ScaleMatrix(float s)
        {
            Matrix m = Identity() * s;
            m.mat[3, m.mat.GetLength(0)-1] = 1;

            return m;
        }

        public static Matrix RotateMatrix(float degrees)
        { // Rotating square
            float radians = DegToRadians(degrees);
            float cos = (float) Math.Cos(radians);
            float sin = (float) Math.Sin(radians);

            Matrix m = new Matrix();
            m.mat[0, 0] = cos;
            m.mat[0, 1] = -sin;
            m.mat[1, 0] = sin;
            m.mat[1, 1] = cos;

            return m;
        }

        // Rotating cube
        public static Matrix RotateMatrixAxisX(float degrees)
        {
            float radians = DegToRadians(degrees);
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            Matrix m = new Matrix();
            m.mat[1, 1] = cos;
            m.mat[1, 2] = -sin;
            m.mat[2, 1] = sin;
            m.mat[2, 2] = cos;

            return m;
        }

        public static Matrix RotateMatrixAxisY(float degrees)
        {
            float radians = DegToRadians(degrees);
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            Matrix m = new Matrix();
            m.mat[0, 0] = cos;
            m.mat[0, 2] = sin;
            m.mat[2, 0] = -sin;
            m.mat[2, 2] = cos;

            return m;
        }

        public static Matrix RotateMatrixAxisZ(float degrees)
        {
            float radians = DegToRadians(degrees);
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            Matrix m = new Matrix();
            m.mat[0, 0] = cos;
            m.mat[0, 1] = -sin;
            m.mat[1, 0] = sin;
            m.mat[1, 1] = cos;

            return m;
        }

        public static Matrix ViewMatrix(float r, float phi, float theta)
        {
            Matrix m = new Matrix();
            float radTheta = DegToRadians(theta);
            float radPhi = DegToRadians(phi);

            float cosTheta = (float)Math.Cos(radTheta);
            float cosPhi = (float)Math.Cos(radPhi);
            float sinTheta = (float)Math.Sin(radTheta);
            float sinPhi = (float)Math.Sin(radPhi);

            m.mat[0, 0] = -sinTheta;
            m.mat[0, 1] = cosTheta;
            m.mat[1, 0] = -cosTheta*cosPhi;
            m.mat[1, 1] = -cosPhi*sinTheta;
            m.mat[1, 2] = sinPhi;
            m.mat[2, 0] = cosTheta*sinPhi;
            m.mat[2, 1] = sinTheta*sinPhi;
            m.mat[2, 2] = cosPhi;
            m.mat[2, 3] = -r;

            return m;
        }

        public static Matrix ProjectionMatrix(float d, float z)
        {
            Matrix m = new Matrix();
            float division = d / z;

            m.mat[0, 0] = -division;
            m.mat[1, 1] = -division;

            return m;
        }

        public static Matrix TranslateMatrix(Vector v)
        {
            Matrix m = new Matrix();

            m.mat[0, m.mat.GetLength(0)-1] = v.x;
            m.mat[1, m.mat.GetLength(0)-1] = v.y;
            m.mat[2, m.mat.GetLength(0)-1] = v.z;

            return m;
        }

        public static float DegToRadians(float degrees)
        {
            return (float)((Math.PI / 180) * degrees);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("/" + mat[0, 0] + " " + mat[0, 1] + " " + mat[0, 2] + " " + mat[0, 3] + "\\\n");
            sb.Append(" " + mat[1, 0] + " " + mat[1, 1] + " " + mat[1, 2] +" "+mat[1, 3]+" \n");
            sb.Append(" " + mat[2, 0] + " " + mat[2, 1] + " " + mat[2, 2] +" "+mat[2, 3]+"\n");
            sb.Append("\\" + mat[3, 0] + " " + mat[3, 1] + " " + mat[3, 2] + " " + mat[3, 3] + "/");

            return sb.ToString();
        }
    }
}