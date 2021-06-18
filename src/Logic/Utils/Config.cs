using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Utils
{
    public sealed class Config
    {
        public int NumberOfRetries { get;}

        public Config(int numberOfRetries)
        {
            NumberOfRetries = numberOfRetries;
        }
    }

}
