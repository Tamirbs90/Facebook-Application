using System;

namespace DP_EX1
{
    public class Operation
    {
        public Action Command { get; set; }

        public void Execute()
        {
            Command.Invoke();
        }
    }
}
