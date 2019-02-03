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


        private static void AddToIndex(DirectoryInfo directory, List<string> markdownLines, int depth)
        {
            var mainFile = directory.GetFiles($"{directory.Name}.md").First();
            var entity = EntityDictionary[directory.Name];
            var pad = "".PadLeft(depth * 4);
            markdownLines.Add($"{pad}* [{entity.FullName}](./{entity.MarkdownName})");

            SortedList<string, object> items = new SortedList<string, object>();
            directory.GetDirectories()
                        .ToList()
                        .ForEach(d => items.Add(EntityDictionary[d.Name].FullName, d));
            directory.GetFiles("*.md")
                        .Where(f => f.Name != mainFile.Name)
                        .ToList()
                        .ForEach(f => items.Add(EntityDictionary[f.Name.Split('.').First()].FullName, f));
            foreach (var item in items)
            {
                if (item.Value is FileInfo)
                {
                    entity = EntityDictionary[((FileInfo)item.Value).Name.Split('.').First()];
                    pad = "".PadLeft((depth + 1) * 4);
                    markdownLines.Add($"{pad}* [{entity.FullName}](./{entity.MarkdownName})");
                }
                else
                {
                    AddToIndex(item.Value as DirectoryInfo, markdownLines, depth + 1);
                }
            }
        }

        private static void GenerateMarkdown()
        {
            GenerateIndex();
        }

        private static void GenerateIndex()
        {
            var markdownLines = new List<string>();
            markdownLines.Add($"## {EntityDictionary["scarred-world"].FullName} Index");
            AddToIndex(ScarredWorldSource, markdownLines, 0);
            using (var writer = new StreamWriter($"{Path.Combine(MarkdownTarget.FullName, "index.md")}"))
            {
                markdownLines.ForEach(l => writer.WriteLine(l));
            }
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


        static Program()
        {
            EntityDictionary = Entities.ToDictionary(e => e.Key);
            MarkdownTarget = GetScarredWorldDirectory().GetDirectories("markdown", SearchOption.AllDirectories).FirstOrDefault();
            ScarredWorldSource = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "scarred-world"));
        }

        private static readonly Entity[] Entities = new Entity[]
        {
            new Entity("bankers", "Commerce Guild", "Bankers"),
            new Entity("city", "Nexus", "City of Coins"),
            new Entity("company", "Maqamir Trading Company"),
            new Entity("contract", "Employment Contract", fullName: "Intial Employment Contract"),
            new Entity("deity-evil", "Seethisat", markdownName: "pantheon.md"),
            new Entity("deity-good", "Raya", markdownName: "pantheon.md"),
            new Entity("deity-neutral", "Jarl-Kahn", markdownName: "pantheon.md"),
            new Entity("expulsion", "Expulsion"),
            new Entity("feeders", "Benevolent Benefactors", "Feeders"),
            new Entity("green", "The Green"),
            new Entity("judges", "Adjudicators"),
            new Entity("merchants", "Merchant-Traders"),
            new Entity("pantheor", "Scarred World Pantheon"),
            new Entity("poof", "Sustenance Wafers", "Poof", "Poof Wafers"),
            new Entity("pantheon", "Pantheon", fullName: "Pantheon of the Scarred World"),
            new Entity("prices", "Prices"),
            new Entity("paladins", "Radiant Arms"),
            new Entity("scarred-world", "Scarred World"),
            new Entity("steel-paste", "Steel Paste"),
            new Entity("street-judges", "Justicars", "Street Judges"),
            new Entity("trade-partner-1", "Spire"),
            new Entity("trade-partner-2", "Karrgerra"),
            new Entity("tradesmen", "Tradesmen's Guild"),
            new Entity("wizards", "Magnus Arcana")
        };
        private static readonly Dictionary<string, Entity> EntityDictionary;
        private static readonly DirectoryInfo MarkdownTarget;
        private static readonly DirectoryInfo ScarredWorldSource;
    }
}
