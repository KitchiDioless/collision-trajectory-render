using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ElasticCollisionSimulation
{
    public class Ball
    {
        public float Mass { get; set; }
        public PointF Position { get; set; }
        public PointF Velocity { get; set; }
        public float Radius { get; set; }

        public Ball(float mass, PointF position, PointF velocity, float radius)
        {
            Mass = mass;
            Position = position;
            Velocity = velocity;
            Radius = radius;
        }

        public void UpdatePosition(float timeStep)
        {
            Position = new PointF(Position.X + Velocity.X * timeStep, 
                Position.Y + Velocity.Y * timeStep);
        }

        public void CheckWallCollision(RectangleF bounds)
        {
            if (Position.X - Radius < bounds.Left || Position.X + Radius > bounds.Right)
            {
                Velocity = new PointF(-Velocity.X, Velocity.Y);
                Position = new PointF(
                    Math.Max(bounds.Left + Radius, Math.Min(bounds.Right - Radius, Position.X)),
                    Position.Y
                );
            }

            if (Position.Y - Radius < bounds.Top || Position.Y + Radius > bounds.Bottom)
            {
                Velocity = new PointF(Velocity.X, -Velocity.Y);
                Position = new PointF(
                    Position.X,
                    Math.Max(bounds.Top + Radius, Math.Min(bounds.Bottom - Radius, Position.Y))
                );
            }
        }
    }

    public class SimulationForm : Form
    {
        private Ball ball1, ball2;
        private Stopwatch stopwatch;
        private RectangleF bounds;

        private TextBox mass1Input, posX1Input, posY1Input, speed1Input, angle1Input, radius1Input;
        private TextBox mass2Input, posX2Input, posY2Input, speed2Input, angle2Input, radius2Input;
        private Button startButton;

        private bool isSimulationRunning = false;

        public SimulationForm()
        {
            this.Width = 800;
            this.Height = 500;
            this.DoubleBuffered = true;

            this.BackColor = Color.FromArgb(30, 30, 40);

            ball1 = new Ball(1.0f, new PointF(100, 100), new PointF(200, 150), 20);
            ball2 = new Ball(2.0f, new PointF(300, 200), new PointF(-150, 100), 30);

            bounds = new RectangleF(5, 5, this.ClientSize.Width - 300, this.ClientSize.Height - 10);

            stopwatch = new Stopwatch();

            Application.Idle += (s, e) => Invalidate();

            CreateInputFields(this.ClientSize.Width - 250, 10, "Ball 1", out mass1Input, out posX1Input,
                out posY1Input, out speed1Input, out angle1Input, out radius1Input);
            CreateInputFields(this.ClientSize.Width - 250, 200, "Ball 2", out mass2Input, out posX2Input,
                out posY2Input, out speed2Input, out angle2Input, out radius2Input);

            startButton = new Button
            {
                Text = "Start",
                Location = new Point(this.ClientSize.Width - 250, 400),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(55, 55, 65)
            };
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);
        }

        private void CreateInputFields(int x, int y, string label, out TextBox massInput, out TextBox posXInput,
            out TextBox posYInput, out TextBox speedInput, out TextBox angleInput, out TextBox radiusInput)
        {
            Label massLabel = new Label { Text = $"{label} Mass:",
                Location = new Point(x, y), ForeColor = Color.White };
            massInput = new TextBox { PlaceholderText = "kg",
                Location = new Point(x + 100, y), ForeColor = Color.White,
                BackColor = Color.FromArgb(55, 55, 65) };

            this.Controls.Add(massLabel);
            this.Controls.Add(massInput);

            Label posLabel = new Label { Text = $"{label} Position:", Location = new Point(x, y + 30),
                ForeColor = Color.White };
            posXInput = new TextBox { PlaceholderText = "X", Location = new Point(x + 100, y + 30), 
                ForeColor = Color.White, BackColor = Color.FromArgb(55, 55, 65) };
            posYInput = new TextBox { PlaceholderText = "Y", Location = new Point(x + 100, y + 60),
                ForeColor = Color.White, BackColor = Color.FromArgb(55, 55, 65) };

            this.Controls.Add(posLabel);
            this.Controls.Add(posXInput);
            this.Controls.Add(posYInput);

            Label speedLabel = new Label { Text = $"{label} Speed:", Location = new Point(x, y + 90),
                ForeColor = Color.White };
            speedInput = new TextBox { PlaceholderText = "m/sec", Location = new Point(x + 100, y + 90),
                ForeColor = Color.White, BackColor = Color.FromArgb(55, 55, 65) };

            this.Controls.Add(speedLabel);
            this.Controls.Add(speedInput);

            Label angleLabel = new Label { Text = $"{label} Angle:", Location = new Point(x, y + 120),
                ForeColor = Color.White };
            angleInput = new TextBox { PlaceholderText = "degrees", Location = new Point(x + 100, y + 120),
                ForeColor = Color.White, BackColor = Color.FromArgb(55, 55, 65) };

            this.Controls.Add(angleLabel);
            this.Controls.Add(angleInput);

            Label radiusLabel = new Label { Text = $"{label} Radius:", Location = new Point(x, y + 150),
                ForeColor = Color.White };
            radiusInput = new TextBox { PlaceholderText = "m", Location = new Point(x + 100, y + 150),
                ForeColor = Color.White, BackColor = Color.FromArgb(55, 55, 65) };

            this.Controls.Add(radiusLabel);
            this.Controls.Add(radiusInput);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            float mass1 = float.Parse(mass1Input.Text);
            float posX1 = float.Parse(posX1Input.Text);
            float posY1 = float.Parse(posY1Input.Text);
            float speed1 = float.Parse(speed1Input.Text);
            float angle1 = float.Parse(angle1Input.Text) * (float)Math.PI / 180f;
            float radius1 = float.Parse(radius1Input.Text);

            float mass2 = float.Parse(mass2Input.Text);
            float posX2 = float.Parse(posX2Input.Text);
            float posY2 = float.Parse(posY2Input.Text);
            float speed2 = float.Parse(speed2Input.Text);
            float angle2 = float.Parse(angle2Input.Text) * (float)Math.PI / 180f;
            float radius2 = float.Parse(radius2Input.Text);

            PointF velocity1 = new PointF(speed1 * (float)Math.Cos(angle1), speed1 * (float)Math.Sin(angle1));
            PointF velocity2 = new PointF(speed2 * (float)Math.Cos(angle2), speed2 * (float)Math.Sin(angle2));

            float initialDistance = (float)Math.Sqrt(Math.Pow(posX2 - posX1, 2) + Math.Pow(posY2 - posY1, 2));
            float minDistance = radius1 + radius2;

            if (initialDistance <= minDistance)
            {
                float angle = (float)Math.Atan2(posY2 - posY1, posX2 - posX1);
                posX2 = posX1 + minDistance * (float)Math.Cos(angle);
                posY2 = posY1 + minDistance * (float)Math.Sin(angle);
            }

            ball1 = new Ball(mass1, new PointF(posX1, posY1), velocity1, radius1);
            ball2 = new Ball(mass2, new PointF(posX2, posY2), velocity2, radius2);

            isSimulationRunning = true;
            stopwatch.Start();
        }

        private void CheckBallCollision()
        {
            float dx = ball1.Position.X - ball2.Position.X;
            float dy = ball1.Position.Y - ball2.Position.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            float minDistance = ball1.Radius + ball2.Radius;

            if (distance < minDistance)
            {
                PointF normal = new PointF(dx / distance, dy / distance);
                PointF tangent = new PointF(-normal.Y, normal.X);

                float dpTan1 = ball1.Velocity.X * tangent.X + ball1.Velocity.Y * tangent.Y;
                float dpTan2 = ball2.Velocity.X * tangent.X + ball2.Velocity.Y * tangent.Y;

                float dpNorm1 = ball1.Velocity.X * normal.X + ball1.Velocity.Y * normal.Y;
                float dpNorm2 = ball2.Velocity.X * normal.X + ball2.Velocity.Y * normal.Y;

                float m1 = (dpNorm1 * (ball1.Mass - ball2.Mass) + 2 * ball2.Mass * dpNorm2) 
                    / (ball1.Mass + ball2.Mass);
                float m2 = (dpNorm2 * (ball2.Mass - ball1.Mass) + 2 * ball1.Mass * dpNorm1) 
                    / (ball1.Mass + ball2.Mass);

                ball1.Velocity = new PointF(tangent.X * dpTan1 + normal.X * m1,
                    tangent.Y * dpTan1 + normal.Y * m1);
                ball2.Velocity = new PointF(tangent.X * dpTan2 + normal.X * m2,
                    tangent.Y * dpTan2 + normal.Y * m2);

                float overlap = minDistance - distance;

                ball1.Position = new PointF(ball1.Position.X + normal.X * overlap 
                    / 2, ball1.Position.Y + normal.Y * overlap / 2);
                ball2.Position = new PointF(ball2.Position.X - normal.X * overlap 
                    / 2, ball2.Position.Y - normal.Y * overlap / 2);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.Clear(Color.FromArgb(30, 30, 40));

            if (isSimulationRunning)
            {
                float elapsedTime = (float)stopwatch.Elapsed.TotalSeconds;

                float timeStep = elapsedTime;
                stopwatch.Restart();

                ball1.UpdatePosition(timeStep);
                ball2.UpdatePosition(timeStep);

                CheckBallCollision();

                ball1.CheckWallCollision(bounds);
                ball2.CheckWallCollision(bounds);
            }

            g.DrawRectangle(Pens.White, bounds.X, bounds.Y, bounds.Width, bounds.Height);

            g.FillEllipse(Brushes.Coral, ball1.Position.X - ball1.Radius,
                ball1.Position.Y - ball1.Radius, ball1.Radius * 2, ball1.Radius * 2);
            g.FillEllipse(Brushes.SkyBlue, ball2.Position.X - ball2.Radius,
                ball2.Position.Y - ball2.Radius, ball2.Radius * 2, ball2.Radius * 2);
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new SimulationForm());
        }
    }
}
