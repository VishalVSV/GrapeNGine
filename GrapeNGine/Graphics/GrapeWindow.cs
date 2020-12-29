using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrapeNGine.Graphics
{
    public class ScreenBuffer : Drawable
    {
        public Pixel[,] backBuffer;
        public GrapeGraphics graphics;
        public Vertex[] vertexArr;

        public bool parallel = true;

        public string updateTime = "";


        public ScreenBuffer(int width, int height, int cellX, int cellY, ref Pixel[,] buffer)
        {
            backBuffer = buffer;
            vertexArr = new Vertex[width * height * 4];
            int i = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    vertexArr[i] = new Vertex(new Vector2f(x * cellX, y * cellY), Color.White);
                    vertexArr[i + 1] = new Vertex(new Vector2f((x + 1) * cellX, y * cellY), Color.White);
                    vertexArr[i + 2] = new Vertex(new Vector2f((x + 1) * cellX, (y + 1) * cellY), Color.White);
                    vertexArr[i + 3] = new Vertex(new Vector2f(x * cellX, (y + 1) * cellY), Color.White);
                    i += 4;
                }
            }
            graphics = new GrapeGraphics(ref buffer);
        }

        private void UpdateVertexArray()
        {
            //Parallel.For(0, 100, (int h) => { });

            DateTime dt = DateTime.Now;
            int width = backBuffer.GetLength(0);
            int i = 0;

            if (!parallel)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < backBuffer.GetLength(1); y++)
                    {

                        vertexArr[i].Color = backBuffer[x, y].c;
                        vertexArr[i + 1].Color = backBuffer[x, y].c;
                        vertexArr[i + 2].Color = backBuffer[x, y].c;
                        vertexArr[i + 3].Color = backBuffer[x, y].c;
                        i += 4;
                    }
                }
            }
            else
            {
                Parallel.For(0, width, (int x) =>
                {
                    int k = x * backBuffer.GetLength(1) * 4;
                    for (int y = 0; y < backBuffer.GetLength(1); y++)
                    {
                        vertexArr[k].Color = backBuffer[x, y].c;
                        vertexArr[k + 1].Color = backBuffer[x, y].c;
                        vertexArr[k + 2].Color = backBuffer[x, y].c;
                        vertexArr[k + 3].Color = backBuffer[x, y].c;
                        k += 4;
                    }
                });
            }

            //Parallel.For(0, backBuffer.GetLength(1), (int y) =>
            //  {
            //      int k = y * width;
            //      for (int x = 0; x < width; x++)
            //      {
            //          int l = k + x;

            //          vertexArr[l].Color = backBuffer[x, y].c;
            //          vertexArr[l + 1].Color = backBuffer[x, y].c;
            //          vertexArr[l + 2].Color = backBuffer[x, y].c;
            //          vertexArr[l + 3].Color = backBuffer[x, y].c;
            //      }
            //  });

            updateTime = (DateTime.Now - dt).TotalMilliseconds.ToString();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            //UpdateVertexArray();
            target.Draw(vertexArr, PrimitiveType.Quads);
        }
    }


    public class AsyncWindow
    {
        protected Pixel[,] backBuffer;
        protected bool persistImage = false;

        protected VertexGraphics vecGraph;

        protected RenderWindow window;
        protected InputHandler input;

        protected Color clearColor = Color.White;

        protected double ElapsedTime = 1000 / 60.0;
        protected int FPS = 60;

        private bool Closed = false;

        private double timeAccumulator = 0;

        protected GrapeGraphics graphics;

        private Vector2i mousePos;
        private RectangleShape pix = new RectangleShape();
        private int cellX;
        private int cellY;
        protected ScreenBuffer screen;

        public Thread thread;

        public string TitleDebug;
        protected string Title;
        protected bool fullscreen;
        protected Vector2i resolution;

        protected Dictionary<string, object> DebugProperties;

        double th;

        public bool Construct(uint width, uint height, uint cellX, uint cellY, string title, bool fullscreen)
        {
            return Construct(width, height, cellX, cellY, title, Color.White, fullscreen);
        }

        public bool Construct(uint width, uint height, uint cellX, uint cellY, string title, Color clearColor, bool fullscreen)
        {

            Title = title;
            this.fullscreen = fullscreen;

            resolution = new Vector2i((int)width, (int)height);

            this.cellX = (int)cellX;
            this.cellY = (int)cellY;


            backBuffer = new Pixel[width / cellX, height / cellY];

            backBuffer.Populate(new Pixel().Construct(clearColor, cellX, cellY));

            this.clearColor = clearColor;

            input = new InputHandler();

            graphics = new GrapeGraphics(ref backBuffer);

            screen = new ScreenBuffer(backBuffer.GetLength(0), backBuffer.GetLength(1), (int)cellX, (int)cellY, ref backBuffer);
            vecGraph = new VertexGraphics(ref screen.vertexArr, backBuffer.GetLength(0), backBuffer.GetLength(1));

            thread = new Thread(new ThreadStart(Start));
            thread.Start();

            return true;
        }

        private void Start()
        {

            if (fullscreen)
                window = new RenderWindow(VideoMode.FullscreenModes[0], Title, Styles.Fullscreen);
            else
                window = new RenderWindow(new VideoMode((uint)resolution.X, (uint)resolution.Y), Title);


            window.KeyPressed += input.KeyPress;
            window.KeyReleased += input.KeyRelease;
            window.Closed += Window_Closed;



            OnCreate();
            Loop();
        }

        public void SetTitle(string title)
        {
            window.SetTitle(title);
        }

        public void ValidateVertex(ref Vertex ver)
        {
            if (ver.Position.X > backBuffer.GetLength(0))
            {
                ver.Position = new Vector2f(backBuffer.GetLength(0) - 1, ver.Position.Y);
            }
            if (ver.Position.Y > backBuffer.GetLength(1))
            {
                ver.Position = new Vector2f(ver.Position.X, backBuffer.GetLength(1) - 1);
            }
            if (ver.Position.X < 0)
            {
                ver.Position = new Vector2f(0, ver.Position.Y);
            }
            if (ver.Position.Y < 0)
            {
                ver.Position = new Vector2f(ver.Position.X, 0);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Closed = true;
        }

        public virtual void OnCreate()
        {

        }

        public virtual void OnUpdate(double ElapsedTime)
        {
        }

        public int GetWidth()
        {
            return backBuffer.GetLength(0);
        }

        public int GetHeight()
        {
            return backBuffer.GetLength(1);
        }

        public Vector2u GetResolution()
        {
            return window.Size;
        }

        public Vector2i GetMousePos()
        {
            return mousePos;
        }

        public void SetFramerate(int rate)
        {
            window.SetFramerateLimit((uint)rate);
        }

        private void Render()
        {

            window.Draw(screen);
        }

        public VertexGraphics CreateGraphics()
        {
            return vecGraph;
        }

        private void Loop()
        {
            window.SetActive(true);
            Clock clock = new Clock();
            while (window.IsOpen && thread.ThreadState != ThreadState.Aborted)
            {
                window.DispatchEvents();
                if (Closed)
                    window.Close();


                mousePos = Mouse.GetPosition(window);

                mousePos.X = mousePos.X / cellX;
                mousePos.Y = mousePos.Y / cellY;


                if (!persistImage)
                {
                    window.Clear(clearColor);
                    vecGraph.Fill(clearColor);
                }
                OnUpdate(ElapsedTime);


                DateTime dt = DateTime.Now;
                Render();
                double rt = (DateTime.Now - dt).TotalMilliseconds;


                window.Display();



                Time t = clock.Restart();
                th = t.AsMilliseconds();
                ElapsedTime = t.AsMilliseconds() / 1000.0;

                timeAccumulator += ElapsedTime * 1000;

                if (timeAccumulator >= 250)
                {
                    window.SetTitle(Title + " " + ((int)(1000.0 / th)).ToString() + " " + rt.ToString() + " Debug:" + TitleDebug);
                    timeAccumulator = 0;
                }
            }

        }
    }

    public class GrapeWindow
    {
        protected Pixel[,] backBuffer;
        protected bool persistImage = false;

        protected VertexGraphics vecGraph;

        protected RenderWindow window;
        protected InputHandler input;

        protected Color clearColor = Color.White;

        protected double ElapsedTime = 1000 / 60.0;
        protected int FPS = 60;

        private bool Closed = false;

        private double timeAccumulator = 0;

        protected GrapeGraphics graphics;

        private Vector2i mousePos;
        private RectangleShape pix = new RectangleShape();
        private int cellX;
        private int cellY;
        protected ScreenBuffer screen;

        public string TitleDebug;
        protected string Title;

        protected Dictionary<string, object> DebugProperties;

        double th;

        public bool Construct(uint width, uint height, uint cellX, uint cellY, string title, bool fullscreen)
        {
            return Construct(width, height, cellX, cellY, title, Color.White, fullscreen);
        }

        public bool Construct(uint width, uint height, uint cellX, uint cellY, string title, Color clearColor, bool fullscreen)
        {
            Title = title;

            if (fullscreen)
                window = new RenderWindow(VideoMode.FullscreenModes[0], title, Styles.Fullscreen);
            else
                window = new RenderWindow(new VideoMode(width, height), title);

            

            this.cellX = (int)cellX;
            this.cellY = (int)cellY;

            backBuffer = new Pixel[window.Size.X / cellX, window.Size.Y / cellY];

            backBuffer.Populate(new Pixel().Construct(clearColor, cellX, cellY));

            this.clearColor = clearColor;

            input = new InputHandler();

            graphics = new GrapeGraphics(ref backBuffer);

            window.KeyPressed += input.KeyPress;
            window.KeyReleased += input.KeyRelease;
            window.Closed += Window_Closed;

            screen = new ScreenBuffer(backBuffer.GetLength(0), backBuffer.GetLength(1), (int)cellX, (int)cellY, ref backBuffer);
            vecGraph = new VertexGraphics(ref screen.vertexArr, backBuffer.GetLength(0), backBuffer.GetLength(1));

            OnCreate();

            Loop();


            return true;
        }

        public void SetTitle(string title)
        {
            window.SetTitle(title);
        }

        public void ValidateVertex(ref Vertex ver)
        {
            if (ver.Position.X > backBuffer.GetLength(0))
            {
                ver.Position = new Vector2f(backBuffer.GetLength(0) - 1, ver.Position.Y);
            }
            if (ver.Position.Y > backBuffer.GetLength(1))
            {
                ver.Position = new Vector2f(ver.Position.X, backBuffer.GetLength(1) - 1);
            }
            if (ver.Position.X < 0)
            {
                ver.Position = new Vector2f(0, ver.Position.Y);
            }
            if (ver.Position.Y < 0)
            {
                ver.Position = new Vector2f(ver.Position.X, 0);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Closed = true;
        }

        public virtual void OnCreate()
        {

        }

        public virtual void OnUpdate(double ElapsedTime)
        {
        }

        public int GetWidth()
        {
            return backBuffer.GetLength(0);
        }

        public int GetHeight()
        {
            return backBuffer.GetLength(1);
        }

        public Vector2u GetResolution()
        {
            return window.Size;
        }

        public Vector2i GetMousePos()
        {
            return mousePos;
        }

        public void SetFramerate(int rate)
        {
            window.SetFramerateLimit((uint)rate);
        }

        private void Render()
        {

            window.Draw(screen);
        }

        public VertexGraphics CreateGraphics()
        {
            return vecGraph;
        }

        private void Loop()
        {
            Clock clock = new Clock();
            while (window.IsOpen)
            {
                window.DispatchEvents();
                if (Closed)
                    window.Close();


                mousePos = Mouse.GetPosition(window);

                mousePos.X = mousePos.X / cellX;
                mousePos.Y = mousePos.Y / cellY;


                if (!persistImage)
                {
                    window.Clear(clearColor);
                    vecGraph.Fill(clearColor);
                }
                OnUpdate(ElapsedTime);


                DateTime dt = DateTime.Now;
                Render();
                double rt = (DateTime.Now - dt).TotalMilliseconds;


                window.Display();



                Time t = clock.Restart();
                th = t.AsMilliseconds();
                ElapsedTime = t.AsMilliseconds() / 1000.0;

                timeAccumulator += ElapsedTime * 1000;

                if (timeAccumulator >= 250)
                {
                    window.SetTitle(Title + " " + ((int)(1000.0 / th)).ToString() + " " + rt.ToString() + " Debug:" + TitleDebug);
                    timeAccumulator = 0;
                }
            }

        }
    }
}
