using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapeNGine.Graphics
{
    public static class GraphicExtensions
    {
        public static byte[] ColorLerp(Color a, Color b, float t)
        {
            byte R, G, B;
            byte[] rgb = new byte[3];

            R = (byte)((int)(a.R + (b.R - a.R) * t));
            B = (byte)((int)(a.B + (b.B - a.B) * t));
            G = (byte)((int)(a.G + (b.G - a.G) * t));

            rgb[0] = R;
            rgb[1] = G;
            rgb[2] = B;

            return rgb;
        }

        public static Color ColorLrp(Color a, Color b, float t)
        {
            byte[] rgb = ColorLerp(a, b, t);
            return new Color(rgb[0], rgb[1], rgb[2]);
        }
    }

    public enum GraphicsMode
    {
        NORMALIZED,
        NON_NORMALIZED
    }

    public class VertexGraphics
    {
        private Vertex[] image;

        public GrapeImage Font;

        public int width, height;

        public GraphicsMode mode = GraphicsMode.NON_NORMALIZED;

        public VertexGraphics(ref Vertex[] image, int width, int height)
        {
            this.image = image;
            this.width = width;
            this.height = height;

            Font = new GrapeImage("./Resources/DefaultFont.png");
        }

        public void Fill(Color color)
        {
            Parallel.For(0, width, (int x) =>
            {
                int k = x * height * 4;
                for (int y = 0; y < height; y++)
                {
                    image[k].Color = color;
                    image[k + 1].Color = color;
                    image[k + 2].Color = color;
                    image[k + 3].Color = color;
                    k += 4;
                }
            });
        }

        public void Clear()
        {
            Fill(Color.White);
        }

        public void ValidateVertex(ref int x, ref int y)
        {
            if (x > width)
            {
                x = width - 1;
            }
            if (y > height)
            {
                y = height - 1;
            }
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
        }

        public void ValidateVertex(ref float x, ref float y)
        {
            if (x > width)
            {
                x = width - 1;
            }
            if (y > height)
            {
                y = height - 1;
            }
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
        }

        public void DrawPoint(Vertex point)
        {
            DrawPoint(point.Position.X, point.Position.Y, point.Color);

        }

        public void DrawPoint(float x, float y, Color color)
        {
            if (mode == GraphicsMode.NON_NORMALIZED)
            {
                DrawPoint((int)x, (int)y, color);
            }
            else
            {
                DrawPoint((int)((x * (width / 2)) + width / 2), (int)((y * height / 2) + height / 2), color);
            }
        }

        public void DrawPoint(int x, int y, Color color)
        {
            if (x < width && x >= 0 && y < height && y >= 0 && color.A > 0)
            {
                int k = (x * height + y) * 4;
                image[k].Color = color;
                image[k + 1].Color = color;
                image[k + 2].Color = color;
                image[k + 3].Color = color;
            }

        }

        public void DrawPoints(Vertex[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                DrawPoint(points[i]);
            }

        }

        public void DrawPoints(IList<Vertex> points)
        {
            foreach (Vertex point in points)
            {
                try
                {
                    int k = (int)(point.Position.X * width + point.Position.Y * 4);
                    image[k].Color = point.Color;
                }
                catch (IndexOutOfRangeException)
                {

                }
            }

        }

        public void DrawLine(float x, float y, float x2, float y2, Color color)
        {
            int x0 = (int)x, y0 = (int)y, x1 = (int)x2, y1 = (int)y2;
            DrawLine(x0, y0, x1, y1, color);
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                DrawPoint(x0, (float)y0, color);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x3, y3, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
        }

        public void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            DrawLine((int)x1, (int)y1, (int)x2, (int)y2, color);
            DrawLine((int)x3, (int)y3, (int)x2, (int)y2, color);
            DrawLine((int)x2, (int)y2, (int)x3, (int)y3, color);
        }

        private float EdgeFunction(float x1, float y1, float x2, float y2, float xP, float yP)
        {
            float res = 0;
            res = (xP - x1) * (y2 - y1) - (yP - y1) * (x2 - x1);
            return res;
        }

        public void FillTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            float minX = Math.Min(x1, Math.Min(x2, x3));
            float minY = Math.Min(y1, Math.Min(y2, y3));
            float maxX = Math.Max(x1, Math.Max(x2, x3));
            float maxY = Math.Max(y1, Math.Max(y2, y3));

            for (int x = (int)minX; x < maxX; x++)
            {
                for (int y = (int)minY; y < maxY; y++)
                {
                    if (EdgeFunction(x1, y1, x2, y2, x, y) > 0 && EdgeFunction(x2, y2, x3, y3, x, y) > 0 && EdgeFunction(x3, y3, x1, y1, x, y) > 0)
                    {
                        DrawPoint(x, y, color);
                    }
                }
            }
        }

        private double DegreesToRadians(double deg)
        {
            return deg * Math.PI / 180;
        }

        public void DrawCircle(float x, float y, float r, Color color)
        {
            double d = 0;
            int detail = 10;
            Vector2f[] points = new Vector2f[360 / detail];
            for (int i = 0; i < 360 / detail; i++)
            {
                float rx1 = (float)(Math.Cos(DegreesToRadians(d)) * r) + x;
                float ry1 = (float)(Math.Sin(DegreesToRadians(d)) * r) + y;
                points[i] = new Vector2f(rx1, ry1);
                d += detail;
            }
            for (int i = 0; i < points.Length; i++)
            {
                if (i + 1 < points.Length)
                {
                    DrawLine(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, color);
                }
                else
                {
                    DrawLine(points[i].X, points[i].Y, points[0].X, points[0].Y, color);
                }
            }
        }

        public void DrawString(string str, float x, float y)
        {
            float x0 = x;
            float y0 = y;
            for (int d = 0; d < str.Length; d++)
            {
                int i = str[d];
                int x1 = i % 16;
                int y1 = (int)(i / 16.0f);
                Font.DrawClipped(this, x0, y0, x1 * 32, y1 * 32, 32, 32);
                if (x0 + 32 < width)
                    x0 += 32;
                else
                {
                    x0 = x;
                    y0 += 32;
                }
            }
        }
    }

    public class GrapeGraphics
    {
        private Pixel[,] image;

        public GrapeGraphics(ref Pixel[,] image)
        {
            this.image = image;
        }

        public void Fill(Color color)
        {
            image.Fill(color);
        }

        public void ValidateVertex(ref int x, ref int y)
        {
            if (x > image.GetLength(0))
            {
                x = image.GetLength(0) - 1;
            }
            if (y > image.GetLength(1))
            {
                y = image.GetLength(1) - 1;
            }
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
        }

        public void ValidateVertex(ref float x, ref float y)
        {
            if (x > image.GetLength(0))
            {
                x = image.GetLength(0) - 1;
            }
            if (y > image.GetLength(1))
            {
                y = image.GetLength(1) - 1;
            }
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }
        }

        public void DrawPoint(Vertex point)
        {
            try
            {
                image[(int)Math.Round(point.Position.X), (int)Math.Round(point.Position.Y)].c = point.Color;
            }
            catch (IndexOutOfRangeException)
            {

            }

        }

        public void DrawPoint(int x, int y, Color color)
        {
            if (x < image.GetLength(0) && x >= 0 && y < image.GetLength(1) && y >= 0)
                image[x, y].c = color;

        }

        public void DrawPoints(Vertex[] points)
        {
            foreach (Vertex point in points)
            {
                try
                {
                    image[(int)Math.Round(point.Position.X), (int)Math.Round(point.Position.Y)].c = point.Color;
                }
                catch (IndexOutOfRangeException)
                {

                }
            }

        }

        public void DrawPoints(IList<Vertex> points)
        {
            foreach (Vertex point in points)
            {
                try
                {
                    image[(int)Math.Round(point.Position.X), (int)Math.Round(point.Position.Y)].c = point.Color;
                }
                catch (IndexOutOfRangeException)
                {

                }
            }

        }

        public void DrawLine(float x, float y, float x2, float y2, Color color)
        {
            int x0 = (int)x, y0 = (int)y, x1 = (int)x2, y1 = (int)y2;
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                DrawPoint(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                DrawPoint(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x3, y3, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
        }

        public void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            DrawLine((int)x1, (int)y1, (int)x2, (int)y2, color);
            DrawLine((int)x3, (int)y3, (int)x2, (int)y2, color);
            DrawLine((int)x2, (int)y2, (int)x3, (int)y3, color);
        }

        private float EdgeFunction(float x1, float y1, float x2, float y2, float xP, float yP)
        {
            float res = 0;
            res = (xP - x1) * (y2 - y1) - (yP - y1) * (x2 - x1);
            return res;
        }

        public void FillTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            float minX = Math.Min(x1, Math.Min(x2, x3));
            float minY = Math.Min(y1, Math.Min(y2, y3));
            float maxX = Math.Max(x1, Math.Max(x2, x3));
            float maxY = Math.Max(y1, Math.Max(y2, y3));

            for (int x = (int)minX; x < maxX; x++)
            {
                for (int y = (int)minY; y < maxY; y++)
                {
                    if (EdgeFunction(x1, y1, x2, y2, x, y) > 0 && EdgeFunction(x2, y2, x3, y3, x, y) > 0 && EdgeFunction(x3, y3, x1, y1, x, y) > 0)
                    {
                        DrawPoint(x, y, color);
                    }
                }
            }
        }

        public void FillTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color, float off = 0f)
        {
            float dist1 = (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            float dist2 = (float)Math.Sqrt((x1 - x3) * (x1 - x3) + (y1 - y3) * (y1 - y3));

            float offset;
            if (off != 0)
                offset = 1.0f / image[0, 0].width;
            else
                offset = off;
            float i = 0;
            while (i < 1)
            {
                float dx1 = (i * x2 + (1 - i) * x1);
                float dy1 = (i * y2 + (1 - i) * y1);
                float dx2 = (i * x3 + (1 - i) * x1);
                float dy2 = (i * y3 + (1 - i) * y1);

                DrawLine(dx1, dy1, dx2, dy2, color);
                DrawLine(dx1, dy1 + offset, dx2, dy2 + offset, color);
                DrawLine(dx1 + offset, dy1, dx2 + offset, dy2, color);

                i += 0.01f;
            }
        }
    }
}
