using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace GrapeNGine.Graphics
{
    public struct Pixel
    {
        public Color c;
        public uint width;
        public uint height;
    }

    public static class Extensions
    {
        public static void Populate(this Pixel[,] array,Pixel Value)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = Value;
                }
            }
        }

        public static void Populate<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public static void Populate<T>(this List<T> array,T value)
        {
            for (int i = 0; i < array.Count; i++)
            {
                array[i] = value;
            }
        }

        public static void Populate<T>(this List<T> array, Func<T> value)
        {
            for (int i = 0; i < array.Count; i++)
            {
                array[i] = value.Invoke();
            }
        }

        public static void Populate<T>(this T[,] array, T value)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = value;
                }
            }
        }

        public static void Fill(this Pixel[,] array,Color color)
        {
            int w = array.GetLength(0);
            int h = array.GetLength(1);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (x == 3)
                    {

                    }
                    array[x, y].c = color;
                }
            }
        }

        public static void Fill(this Pixel[,] array, Color color,int width,int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    array[x, y].c = color;
                }
            }
        }

        public static void Fill(this Pixel[,] array, Color color,int startX,int startY, int width, int height)
        {
            for (int x = startX; x < startX+width; x++)
            {
                for (int y = startY; y < startY+height; y++)
                {
                    array[x, y].c = color;
                }
            }
        }

        public static Pixel Construct(this Pixel p,Color color,uint width,uint height)
        {
            p.c = color;
            p.width = width;
            p.height = height;
            return p;
        }
    }
}
