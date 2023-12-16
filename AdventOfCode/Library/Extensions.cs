using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class Extensions
    {
        public static string MkString(this IEnumerable<string> values, string delimitter)
        {
            return string.Join(delimitter, values);
        }
    }
}
