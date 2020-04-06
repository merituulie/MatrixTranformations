using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace MatrixTransformations
{
    public partial class Form1 : Form
    {
        // Timer
        System.Timers.Timer timer;

        // Axes
        AxisX x_axis;
        AxisY y_axis;
        AxisZ z_axis;

        // Objects
        Square square;
        Square square1;
        Square square2;
        Square square3;

        // Cube
        Cube cube;

        List<Vector> vb = new List<Vector>();

        // Window dimensions
        const int WIDTH = 800;
        const int HEIGHT = 600;

        static float scale = 1f;
        static float rotateX = 0f;
        static float rotateY = 0f;
        static float rotateZ = 0f;
        static float d = 800f;
        static float r = 10f;
        static float theta = -100f;
        static float phi = -10f;
        static float stepSize = 0.01f;
        static float phaseCounter = 1f;
        static float startAnimationPhi = 0f;
        static float startAnimationTheta = 0f;

        public bool phaseLoop1 = true;
        public bool phaseLoop2 = true;
        public bool phaseLoop3 = true;
        public bool phaseLoop4 = true;
        public bool phiReset = false;
        public bool thetaReset = false;
        public bool animation = false;

        Vector vector = new Vector(); // for translation

        public Form1()
        {
            InitializeComponent();

            this.Width = WIDTH;
            this.Height = HEIGHT;
            this.DoubleBuffered = true;

            Vector v1 = new Vector();
            Console.WriteLine(v1);
            Vector v2 = new Vector(1, 2, 1);
            Console.WriteLine(v2);
            Vector v3 = new Vector(2, 6, 1);
            Console.WriteLine(v3);
            Vector v4 = v2 + v3;
            Console.WriteLine(v4); // 3, 8

            Matrix m1 = new Matrix();
            Console.WriteLine(m1); // 1, 0, 0, 1

            Matrix m2 = new Matrix(2, 4, -1, 3);

            Console.WriteLine(m2);
            Console.WriteLine(m1 + m2); // 3, 4, -1, 4
            Console.WriteLine(m1 - m2); // -1, -4, 1, -2
            Console.WriteLine(m2 * m2); // 0, 20, -5, 5
            Console.WriteLine(m2 * v3); // 28, 16

            Matrix m3 = new Matrix();
            Vector v5 = new Vector(3, 6, 4);
            Matrix m4 = new Matrix(3, 4, 7, -3, 11, 10, 3, 1, 3);

            Console.WriteLine(m3);

            m3 = Matrix.TranslateMatrix(v5);

            Console.WriteLine(m3);
            Console.WriteLine(m3 * m4);
            Console.WriteLine(m3 - m4);
            Console.WriteLine(m3 + m4);

            // Define axes
            x_axis = new AxisX(3);
            y_axis = new AxisY(3);
            z_axis = new AxisZ(3);

            // Create object
            square = new Square(Color.Purple, 3);
            square1 = new Square(Color.Orange, 2);
            square2 = new Square(Color.Cyan, 1);
            square3 = new Square(Color.DarkBlue, 1);
            cube = new Cube(Color.Purple);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawValue(e);

            // Draw axes
            vb = ViewingPipeline(x_axis.vb);
            x_axis.Draw(e.Graphics, vb);

            vb = ViewingPipeline(y_axis.vb);
            y_axis.Draw(e.Graphics, vb);

            vb = ViewingPipeline(z_axis.vb);
            z_axis.Draw(e.Graphics, vb);

            // Add all features to mTotal matrix
            Matrix mS = Matrix.ScaleMatrix(scale);
            Matrix mX = Matrix.RotateMatrixAxisX(rotateX);
            Matrix mY = Matrix.RotateMatrixAxisY(rotateY);
            Matrix mZ = Matrix.RotateMatrixAxisZ(rotateZ);
            Matrix mT = Matrix.TranslateMatrix(vector);

            Matrix mTotal = mT * mX * mY * mZ * mS;

            // Draw cube
            vb = ViewingPipeline(GetNewVectors(mTotal, cube));
            cube.Draw(e.Graphics, vb);

        }

        public void DrawValue(PaintEventArgs e)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append("Scale:\t\t" + formatValue(scale) + "\tS / s\n");
            sb.Append("TranslateX:\t" + formatValue(vector.x) + "\t Left / Right\n");
            sb.Append("TranslateY:\t" + formatValue(vector.y) + "\t Up / Down\n");
            sb.Append("TranslateZ:\t" + formatValue(vector.z) + "\t PgDn / PgUp\n");
            sb.Append("RotateX:\t" + formatValue(rotateX) + "\t X / x\n");
            sb.Append("RotateY:\t\t" + formatValue(rotateY) + "\t Y / y\n");
            sb.Append("RotateZ:\t\t" + formatValue(rotateZ) + "\t Z / z\n");
            sb.Append("\n");
            sb.Append("r:\t" + formatValue(r) + "\t R / r\n");
            sb.Append("d:\t" + formatValue(d) + "\t D / d \n");
            sb.Append("phi:\t" + formatValue(phi) + "\t P / p \n");
            sb.Append("theta:\t" + formatValue(theta) + "\t T / t \n");
            sb.Append("Phase:\t " + formatValue(phaseCounter));

            // Draw the values
            Font font = new Font("Arial", 10);
            PointF p = new PointF(0, 0);
            e.Graphics.DrawString(sb.ToString(), font, Brushes.Black, p);
        }

        public string formatValue(float value)
        {
            string valueAsString = value.ToString("0.00");

            return valueAsString;
        }

        public static List<Vector> GetNewVectors(Matrix m, Cube cb)
        {
            List<Vector> results = new List<Vector>();

            foreach (Vector v in cb.vertexbuffer)
            {
                Vector v2 = v * m;
                results.Add(v2);
            }

            return results;
        }

        public static List<Vector> ViewingPipeline(List<Vector> vb)
        {
            List<Vector> result = new List<Vector>();
            Vector vp = new Vector();
            Matrix view = Matrix.ViewMatrix(r, phi, theta);

            foreach (Vector v in vb)
            {
                vp = view * v;

                Matrix proj = Matrix.ProjectionMatrix(d, vp.z);

                vp = proj * vp;

                result.Add(vp);
            }

            return ViewPortTransformation(result);
        }

        public static List<Vector> ViewPortTransformation(List<Vector> vb)
        {
            List<Vector> result = new List<Vector>();

            float delta_x = WIDTH / 2;
            float delta_y = HEIGHT / 2;

            foreach (Vector v in vb)
            {
                Vector v2 =  new Vector(v.x + delta_x, delta_y - v.y, 1);
                result.Add(v2);
            }

            return result;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }

            if (e.KeyCode == Keys.S)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    scale += 0.1f;
                }
                else
                {
                    if (scale > 0)
                    {
                        scale -= 0.1f;
                    }
                }
            }
            else if (e.KeyCode == Keys.X)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    rotateX += 0.1f;
                }
                else
                {
                    rotateX -= 0.1f;
                }
            }
            else if (e.KeyCode == Keys.Y)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    rotateY += 0.1f;
                }
                else
                {
                    rotateY -= 0.1f;
                }
            }
            else if (e.KeyCode == Keys.Z)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    rotateZ += 0.1f;
                }
                else
                {
                    rotateZ -= 0.1f;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                vector.x += 0.1f;
            }
            else if (e.KeyCode == Keys.Left)
            {
                vector.x -= 0.1f;
            }
            else if (e.KeyCode == Keys.Up)
            {
                vector.y += 0.1f;
            }
            else if (e.KeyCode == Keys.Down)
            {
                vector.y -= 0.1f;
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                vector.z -= 0.1f;
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                vector.z += 0.1f;
            }
            else if (e.KeyCode == Keys.R)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    r += 0.1f;
                }
                else
                {
                    r -= 0.1f;
                }
            }
            else if (e.KeyCode == Keys.D)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    d += 3f; // .01f is suuuper slow, so I left this to 3f.
                }
                else
                {
                    d -= 3f;
                }
            }
            else if (e.KeyCode == Keys.T)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    theta += 0.1f;
                }
                else
                {
                    theta -= 0.1f;
                }
            }
            else if (e.KeyCode == Keys.P)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    phi += 0.1f;
                }
                else
                {
                    phi -= 0.1f;
                }
            }
            else if (e.KeyCode == Keys.A)
            {
                if (!animation)
                {
                    animation = true;
                    startAnimationPhi = phi;
                    startAnimationTheta = theta;
                    setTimer();
                    timer.Start();
                }
            }
            else if (e.KeyCode == Keys.C)
            {
                ResetCube();
                if (animation)
                {
                    timer.Stop();
                    timer.Dispose();
                    animation = false;
                }
            }
            Invalidate();
        }

        // Pressing key C resets the cube back to its original values.
        public void ResetCube()
        {
            scale = 1f;
            rotateX = 0f;
            rotateY = 0f;
            rotateZ = 0f;
            d = 800f;
            r = 10f;
            theta = -100f;
            phi = -10f;
        }

        public void setTimer()
        {
            timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 50;
            timer.AutoReset = true;
            timer.Elapsed += Animate;
        }

        private void PhaseOne()
        {
                // Shrink
                if (scale >= 1.5f)
                {
                    stepSize = -.01f;
                    phaseLoop1 = false;
                }
                // Grow
                if (scale <= 1f)
                {
                    // If first loop then grow, otherwise end phase 1 and return to Animate()
                    if (phaseLoop1)
                    {
                        stepSize = .01f;
                    }
                    else
                    {
                        phaseCounter++;
                        return;
                    }
                }
            scale += stepSize;
            theta -= 1f;
        }

        private void PhaseTwo()
        {
                // Rotation on X-axis
                if (rotateX >= 45f)
                {
                    stepSize = -1f;
                    phaseLoop2 = false;
                }
                if (rotateX <= 0f)
                {
                    if (phaseLoop2)
                    {
                        stepSize = 1f;
                    }
                    else 
                    {
                        phaseCounter++;
                        return;
                    }
                }
                rotateX += stepSize;
                theta -= 1f;
        }

        private void PhaseThree()
        {
            // Rotation on Y-axis
            if (rotateY >= 45f)
            {
                stepSize = -1f;
                phaseLoop3 = false;
            }
            if (rotateY <= 0f)
            {
                if (phaseLoop3)
                {
                    stepSize = 1f;
                }
                else 
                {
                    phaseCounter++;
                    return;
                }
            }
            rotateY += stepSize;
            phi += 1f;
        }

        private void PhaseFour()
        {
            // Rotate on phi and theta back to the original position. When done, set phaseCounter to one and return to Animate to start the cycle all over again.
            if (phi >= startAnimationPhi)
            {
                stepSize = -1f;
                phi += stepSize;
            }
            else 
            {
                phiReset = true;
            }

            if (theta <= startAnimationTheta)
            {
                stepSize = 1f;
                theta += stepSize;
            }
            else 
            {
                thetaReset = true;
            }

            if (phiReset && thetaReset)
            {
                phaseCounter = 1;
                phaseLoop1 = true;
                phaseLoop2 = true;
                phaseLoop3 = true;
                phaseLoop4 = true;
                phiReset = false;
                thetaReset = false;
            }

        }

        public void Animate(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine(phaseCounter);
            if (phaseCounter == 1)
                PhaseOne();
            else if (phaseCounter == 2)
                PhaseTwo();
            else if (phaseCounter == 3)
                PhaseThree();
            else if (phaseCounter == 4)
                PhaseFour();

            Invalidate();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
