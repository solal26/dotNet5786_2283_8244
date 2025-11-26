using System;
using System.Numerics;
using BO;

namespace BlTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        }
    }
}
