using GrapeNGine.Graphics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomata
{
    class CellularAutomata : GrapeWindow
    {
        Random rand;
        Color Tree = Color.Green;
        int[,] lifetimes;
        int[,] trees;
        int defaultLife = 30;

        public override void OnCreate()
        {
            lifetimes = new int[GetWidth(), GetHeight()];
            lifetimes.Populate(defaultLife);
            rand = new Random(314);

            trees = new int[GetWidth(), GetHeight()];
            trees.Populate(0);

            //trees[50, 50] = 1;
        }

        public override void OnUpdate(double ElapsedTime)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                Vector2i mousePos = GetMousePos();
                trees[GetMousePos().X, GetMousePos().Y] = 1;
            }

            for (int i = 0; i < GetWidth(); i++)
            {
                for (int j = 0; j < GetHeight(); j++)
                {
                    if (trees[i,j] == 1)
                    {
                        
                        if (lifetimes[i,j] <= 0)
                        {
                            trees[i, j] = 0;
                            continue;
                        } else
                        {
                            lifetimes[i, j] -= 1;
                        }

                        if (rand.Next(100) > 92)
                        {
                            int dir = rand.Next(4);
                            if (dir == 0 && i >=1)
                            {
                                trees[i - 1, j] = 1;
                                lifetimes[i - 1, j] = defaultLife;
                            }
                            else if (dir == 1 && i < GetWidth()- 1)
                            {
                                trees[i+1, j] = 1;
                                lifetimes[i + 1, j] = defaultLife;
                            }
                            else if (dir == 2 && j >= 1)
                            {
                                trees[i, j - 1] = 1;
                                lifetimes[i, j - 1] = defaultLife;
                            }
                            else if (dir == 3 && j < GetHeight() - 1)
                            {
                                trees[i, j + 1] = 1;
                                lifetimes[i, j + 1] = defaultLife;
                            }
                        }
                    }

                    if (trees[i,j] == 1)
                    {
                        backBuffer[i, j].c = Tree;
                    }
                }
            }
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            CellularAutomata cellularAutomata = new CellularAutomata();
            cellularAutomata.Construct(640, 480, 5, 5, "Cellular Automata");
        }
    }
}
