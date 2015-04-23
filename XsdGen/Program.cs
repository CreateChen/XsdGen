using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XsdGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //string dllPath = GetDllPath(args);
            var dllPath = @"D:\Projects\XsdGen\TestDLL\bin\Debug\TestDLL.dll";
            var assembly = Assembly.LoadFile(dllPath);

            var builder = new XsdBuilder();
            builder.Build(assembly);
        }

        private static string GetDllPath(string[] args)
        {
            string dllPath = null;
            while (string.IsNullOrEmpty(dllPath) || !dllPath.EndsWith(".dll") || !File.Exists(dllPath))
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("请传入正确的dll文件地址:");
                    dllPath = Console.ReadLine();
                }
                else
                {
                    dllPath = args[0];
                }
            }
            return dllPath;
        }
    }
}