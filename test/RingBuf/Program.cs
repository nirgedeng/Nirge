using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nirge.Core;

namespace RingBuf
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new CRingBuf(7);

            var b = new byte[] { 1, 2, 3, };
            a.Write(b, 0, 3);
            Console.WriteLine(a.UnusedCapacity);

            var c = new byte[] { 4, 5, 6, };
            a.Write(c, 0, 3);
            Console.WriteLine(a.UnusedCapacity);

            byte[] b1;
            a.Read(3, out b1);
            var d = new byte[] { 7, 8, 9, };
            Console.WriteLine(a.UnusedCapacity);

            a.Write(d, 0, 3);
            Console.WriteLine(a.UnusedCapacity);

            byte[] c1;
            a.Read(3, out c1);
            Console.WriteLine(a.UnusedCapacity);

            byte[] d1;
            a.Read(3, out d1);
            Console.WriteLine(a.UnusedCapacity);
        }
    }
}
