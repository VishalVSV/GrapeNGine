using SFML.Window;

namespace GrapeNGine
{
    public class InputHandler
    {
        private bool[] keys = new bool[256];

        public void KeyPress(object sender, KeyEventArgs e)
        {
            keys[(int)e.Code] = true;
        }

        public void KeyRelease(object sender, KeyEventArgs e)
        {
            keys[(int)e.Code] = false;
        }

        public bool GetKey(int key)
        {
            return keys[key];
        }
    }
}
