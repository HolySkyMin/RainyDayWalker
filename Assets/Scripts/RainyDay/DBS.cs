using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace RainyDay
{
    public class DBS<T>
    {
        public T Value { get; set; }
    }

    public class DBS<T1, T2>
    {
        public T1 Value1 { get; set; }

        public T2 Value2 { get; set; }
    }
}
