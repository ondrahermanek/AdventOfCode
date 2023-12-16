using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2023
{
    internal interface IAocTask
    {
        Task<List<string>> Run();
    }
}
