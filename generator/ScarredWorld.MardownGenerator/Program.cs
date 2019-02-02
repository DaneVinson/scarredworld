using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScarredWorld.MardownGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                GenerateMarkdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} - {1}", ex.GetType(), ex.Message);
                Console.WriteLine(ex.StackTrace ?? String.Empty);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("...");
                Console.ReadKey();
            }
        }


        private static void GenerateMarkdown()
        {
            var source = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            var target = GetScarredWorldDirectory()
                            .GetDirectories("markdown", SearchOption.AllDirectories)
                            .FirstOrDefault();

            //GenerateMarkdown(source, target);
        }

        private static void GenerateMarkdown(DirectoryInfo source, DirectoryInfo target)
        {
            var generator = new DirectoryInfo(Directory.GetCurrentDirectory());

            throw new NotImplementedException();
        }

        private static DirectoryInfo GetScarredWorldDirectory()
        {
            var parts = Directory.GetCurrentDirectory().Split('\\');
            int generatorIndex = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                if ("scarredworld".Equals(parts[i]))
                {
                    generatorIndex = i;
                    break;
                }
            }
            return new DirectoryInfo(String.Join('\\', parts.Take(generatorIndex)));
        }

        private static readonly Dictionary<string, string> Entities = new Dictionary<string, string>()
        {
            { "city", "Nexus" },
            { "city-nickname", "City of Coins" },
            { "city-markdown", "city-of-coins.md" }
        };
    }
}
