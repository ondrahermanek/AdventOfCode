using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace AoC2023
{
    internal class Task1 : IAocTask
    {
        public async Task<List<string>> Run()
        {
            return await Utilities.ReadFile("Task1/Sample1.txt");
        }
    }
}
