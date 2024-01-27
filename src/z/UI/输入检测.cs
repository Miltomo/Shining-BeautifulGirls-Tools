using System.Windows.Input;

namespace MHTools
{
    public static class UI
    {
        public static bool IsKeyNumeric(Key k)
        {
            if ((k >= Key.NumPad0 && k <= Key.NumPad9) ||
                (k >= Key.D0 && k <= Key.D9) || k == Key.Back)
                return true;

            return false;
        }
    }
}
