using Doozy.Runtime.Signals;

namespace _src.Scripts.Utils
{
    public static class DoozyUtils
    {
        public static void SendBackButtonSignal()
        {
            SignalStream.Get("Input", "BackButton").SendSignal();
        }
    }
}