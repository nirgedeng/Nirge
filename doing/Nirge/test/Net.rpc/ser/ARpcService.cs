/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Nirge.Core
{
    public class CARpcService : IARpcService
    {
        public void a(int channel)
        {
            //Console.WriteLine("a");
        }

        public void b(int channel)
        {
            //Console.WriteLine("b");
        }

        public void c(int channel, cargs args)
        {
            //Console.WriteLine($"cargs {args}");
        }

        public void d(int channel, dargs args)
        {
            //Console.WriteLine($"dargs {args}");
        }

        public void e(int channel)
        {
            //Console.WriteLine("e");
        }

        public fret f(int channel)
        {
            //Console.WriteLine("f");
            return new fret()
            {
                A = 1,
                B = 1,
                C = "f",
            };
        }

        public void g(int channel, gargs args)
        {
            //Console.WriteLine($"gargs {args}");
        }

        public hret h(int channel, hargs args)
        {
            //Console.WriteLine($"hargs {args}");
            return new hret()
            {
                A = 1,
                B = 1,
                C = "h",
            };
        }
    }
}
