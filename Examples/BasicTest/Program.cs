using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrapeNGine.Graphics;
using SFML.Graphics;
using SFML.System;

namespace BasicTest
{
    class Window : GrapeWindow
    {
        Vertex[] vertices;
        Vector2f[] velocities;

        float time = 0;

        Vector2f gravity = new Vector2f(0, 0.098f);

        float speed = 6;
        int numberOfParticles = 100;

        Random rand = new Random(1);

        public override void OnCreate()
        {
            vertices = new Vertex[numberOfParticles];
            velocities = new Vector2f[numberOfParticles];
            for (int i = 0; i < numberOfParticles; i++)
            {
                vertices[i] = new Vertex(new Vector2f(64, 48), Color.Black);
                velocities[i] = new Vector2f(((float)rand.NextDouble() - 0.5f) * speed, ((float)rand.NextDouble() - 0.5f) * speed);
            }
        }

        public override void OnUpdate(double deltaTime)
        {
            graphics.DrawPoints(vertices);

            if (time * 1000 > 15)
            {
                for (int i = 0; i < numberOfParticles; i++)
                {
                    vertices[i].Position += velocities[i];
                    velocities[i] += gravity;
                    velocities[i].X *= 0.9f;
                    ValidateVertex(ref vertices[i]);
                }
                time = 0;
            }

            time += (float)deltaTime;
        }

        float lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Window w = new Window();

            w.Construct(640, 480, 2, 2, "Hello");
        }
    }
}
