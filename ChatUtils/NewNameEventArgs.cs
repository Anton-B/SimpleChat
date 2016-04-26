using System;

namespace ChatUtils
{
    public class NewNameEventArgs : EventArgs
    {
        public string NewName { get; }

        public NewNameEventArgs(string newName)
        {
            this.NewName = newName;
        }
    }
}
