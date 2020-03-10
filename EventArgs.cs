using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Daniels.Common
{
    public class ReadOnlyEventArgs<T1, T2> : EventArgs
    {
        public readonly T1 Parameter1;// { get; private set; }
        public readonly T2 Parameter2;// { get; private set; }

        public ReadOnlyEventArgs(T1 param1, T2 param2)
        {
            Parameter1 = param1;
            Parameter2 = param2;
        }
    }

    public class ReadOnlyEventArgs<T> : EventArgs
    {
        public readonly T Parameter;// { get; private set; }

        public ReadOnlyEventArgs(T input)
        {
            Parameter = input;
        }
    }

    public class EventArgs<T> : EventArgs
    {
        public T Parameter { get; set; }

        public EventArgs(T input)
        {
            Parameter = input;
        }
    }
}