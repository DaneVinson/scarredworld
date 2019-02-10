using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScarredWorld.MardownGenerator
{
    class Program
    {
        private static readonly Entity[] CampaignEntities = new Entity[]
        {
            new Entity("bankers", "Commerce Guild", "Bankers", alignment: "LN"),
            new Entity("city", "Nexus", "City of Coins", "Nexus, City of Coins", alignment: "NG"),
            new Entity("company", "Maqamir Trading Company", alignment: "CG"),
            new Entity("contract", "Employment Contract", fullName: "Intial Employment Contract"),
            new Entity("deity-evil", "Seethisat", markdownName: "pantheon.md", alignment: "NE"),
            new Entity("deity-good", "Raya", markdownName: "pantheon.md", alignment: "NG"),
            new Entity("deity-neutral", "Jarl-Kahn", markdownName: "pantheon.md", alignment: "N"),
            new Entity("expulsion", "Expulsion"),
            new Entity("feeders", "Benevolent Benefactors", "Feeders", alignment: "LG"),
            new Entity("green", "The Green"),
            new Entity("judges", "Adjudicators", alignment: "LG"),
            new Entity("merchants", "Merchant-Traders", alignment: "CN"),
            new Entity("pantheon", "Pantheon", fullName: "Pantheon of the Scarred World"),
            new Entity("poof", "Sustenance Wafers", "Poof", "Poof Wafers"),
            new Entity("prices", "Prices"),
            new Entity("paladins", "Radiant Arms", alignment: "LG"),
            new Entity("scarred-world", "Scarred World"),
            new Entity("steel-paste", "Steel Paste"),
            new Entity("street-judges", "Justicars", "Street Judges", alignment: "LN"),
            new Entity("trade-partner-1", "Spire", alignment: "LE"),
            new Entity("trade-partner-2", "Karrgerra", alignment: "NG"),
            new Entity("tradesmen", "Tradesmen's Guild", alignment: "NG"),
            new Entity("wizards", "Magnus Arcana", alignment: "N")
        };


        static void Main(string[] args)
        {
            try
            {
                CleanTarget();
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

        private static void CleanTarget()
        {
            MarkdownTarget.GetFiles("*.md", SearchOption.AllDirectories)
                            .ToList()
                            .ForEach(f => f.Delete());
            var readme = MarkdownTarget.Parent.GetFiles("readme.md").FirstOrDefault();
            if (readme != null) { readme.CopyTo(Path.Combine(MarkdownTarget.FullName, readme.Name)); }
        }

        private static void GenerateMarkdown()
        {
            GenerateIndex();
            GenerateEntities(ScarredWorldSource);
        }

        private static void GenerateEntities(DirectoryInfo directory)
        {
            var crumbEntities = directory.FullName
                                            .Split('\\')
                                            .Skip(ScarredWorldDirectoryIndex)
                                            .Select(s => EntityDictionary[s])
                                            .ToArray();

            foreach (var file in directory.GetFiles("*.md"))
            {
                var entity = EntityDictionary.FirstOrDefault(kv => kv.Key == file.Name.Split('.').First()).Value;
                int take = crumbEntities.Length;
                if (entity.Key == crumbEntities.Last().Key) { --take; }
                var crumbs = crumbEntities.Take(take)
                                            .Select(e => e.NameLink)
                                            .ToList();
                crumbs.Add(entity.FullName);
                WriteMarkdown(entity, crumbs, file);
            }

            foreach (var childDirectory in directory.GetDirectories())
            {
                GenerateEntities(childDirectory);
            }
        }

        private static void WriteMarkdown(Entity entity, IEnumerable<string> crumbs, FileInfo sourceFile)
        {
            using (var reader = new StreamReader(sourceFile.FullName))
            using (var writer = new StreamWriter(Path.Combine(MarkdownTarget.FullName, entity.MarkdownName)))
            {
                string alignment = String.IsNullOrWhiteSpace(entity.Alignment) ? String.Empty : $"[{entity.Alignment}]";
                writer.WriteLine($"# {entity.FullName} {alignment}");
                var crumbsArray = crumbs.ToArray();
                if (crumbsArray.Length > 1) { writer.WriteLine(String.Join(" > ", crumbs)); }
                writer.WriteLine();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('^');
                    if (parts.Length == 1) { writer.WriteLine(line); }
                    else
                    {
                        var lineBuilder = new StringBuilder();
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (i % 2 == 0) { lineBuilder.Append(parts[i]); }
                            else
                            {
                                var metadataParts = parts[i].Split('.');
                                var ent = EntityDictionary[metadataParts[0]];
                                switch (metadataParts[1])
                                {
                                    case "Alignment":
                                        lineBuilder.Append(ent.Alignment);
                                        break;
                                    case "FullName":
                                        lineBuilder.Append(ent.FullName);
                                        break;
                                    case "FullNameLink":
                                        lineBuilder.Append(ent.FullNameLink);
                                        break;
                                    case "MarkdownName":
                                        lineBuilder.Append(ent.MarkdownName);
                                        break;
                                    case "Name":
                                        lineBuilder.Append(ent.Name);
                                        break;
                                    case "NameLink":
                                        lineBuilder.Append(ent.NameLink);
                                        break;
                                    case "Nickname":
                                        lineBuilder.Append(ent.Nickname);
                                        break;
                                    case "NicknameLink":
                                        lineBuilder.Append(ent.NicknameLink);
                                        break;
                                }
                            }
                        }
                        writer.WriteLine(lineBuilder.ToString());
                    }
                }
            }
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

        private static DirectoryInfo GetScarredWorldTopDirectory()
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
            EntityDictionary = CampaignEntities.ToDictionary(e => e.Key);
            MarkdownTarget = GetScarredWorldTopDirectory().GetDirectories("markdown", SearchOption.AllDirectories).FirstOrDefault();
            ScarredWorldSource = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "scarred-world"));
            ScarredWorldDirectoryIndex = ScarredWorldSource.FullName.Split('\\').Count() - 1;
        }

        private static readonly Dictionary<string, Entity> EntityDictionary;
        private static readonly DirectoryInfo MarkdownTarget;
        private static readonly int ScarredWorldDirectoryIndex;
        private static readonly DirectoryInfo ScarredWorldSource;
    }
}
