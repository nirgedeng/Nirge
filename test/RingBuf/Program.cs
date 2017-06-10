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

            byte[] b1 = new byte[3];
            a.Read(b1, 0, 3);
            Console.WriteLine(a.UnusedCapacity);

            var d = new byte[] { 7, 8, 9, };
            a.Write(d, 0, 3);
            Console.WriteLine(a.UnusedCapacity);

            byte[] c1 = new byte[3];
            a.Read(c1, 0, 3);
            Console.WriteLine(a.UnusedCapacity);

            byte[] d1 = new byte[3];
            a.Read(d1, 0, 3);
            Console.WriteLine(a.UnusedCapacity);
        }
    }
}
