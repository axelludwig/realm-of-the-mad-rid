using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utils
{
    public static int SafeCount<T>(IEnumerable<T> list) => list == null ? 0 : list.Count();
}
