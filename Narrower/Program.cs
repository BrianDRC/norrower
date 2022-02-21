using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Narrower
{
    public class Program
    {
        private static Utils _utils;
        static void Main(string[] args)
        {
            _utils = new Utils() { FilePath = "./Resources/MOCK_DATA.csv" };
            _utils.ConvertFile();
        }
    }
}
