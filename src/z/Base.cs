using System;

namespace MHTools
{
    public abstract class Base
    {
        public event Action<string>? LogEvent;

        protected virtual void OnLog(string text)
        {
            LogEvent?.Invoke(text);
        }
    }
}
