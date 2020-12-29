using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapeNGine.Graphics
{

    public class GrapeImage
    {
        SFML.Graphics.Color[,] image;
        public int Width
        {
            get
            {
                return image.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return image.GetLength(1);
            }
        }

        int w, h;

        public GrapeImage(int width, int height)
        {
            image = new SFML.Graphics.Color[width, height];
            w = width;
            h = height;
        }

        public GrapeImage(string path)
        {
            Bitmap img = new Bitmap(path);
            image = new SFML.Graphics.Color[img.Width, img.Height];
            w = img.Width;
            h = img.Height;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    System.Drawing.Color r = img.GetPixel(x, y);
                    SFML.Graphics.Color color = new SFML.Graphics.Color(r.R, r.G, r.B, r.A);
                    image[x, y] = color;

                }
            }
        }

        public SFML.Graphics.Color GetPixel(int x,int y)
        {
            return image[x, y];
        }

        public void Draw(VertexGraphics g)
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    g.DrawPoint(x, y, image[x, y]);
                }
            }
        }

        public void DrawClipped(VertexGraphics g, float x1, float y1, float x, float y, float w, float h)
        {
            if (x >= 0 && x < this.w && y >= 0 && y < this.h && x + w < this.w && x + w >= 0 && y + h < this.h && y + h >= 0)
            {
                for (int y0 = (int)y; y0 < y + h; y0++)
                {
                    for (int x0 = (int)x; x0 < x + w; x0++)
                    {
                        g.DrawPoint(x1 + (x0 - x), y1 + (y0 - y), image[x0, y0]);
                    }
                }
            }
        }
    }
}
