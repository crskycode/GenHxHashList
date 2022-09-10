using System.Text.RegularExpressions;

#pragma warning disable IDE0063

namespace GenHxHashList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  GenHxHashList \"Dump.log\"...");
                Console.WriteLine();
                Console.WriteLine("Note: Support multi input file,");
                Console.WriteLine("  output will created in tool directory.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            var path_map = new Dictionary<string, string>();
            var name_map = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                Console.WriteLine("Read file...");

                using (var stream = File.OpenText(arg))
                {
                    while (!stream.EndOfStream)
                    {
                        var line = stream.ReadLine();

                        if (line == null || line.Length == 0)
                            continue;

                        var match1 = Regex.Match(line, "PathHash: \"(.+?)\" \"(.+?)\" \"(.+?)\"");
                        if (match1.Success && match1.Groups.Count == 4)
                        {
                            var path_hash = match1.Groups[3].Value;
                            var path_str = match1.Groups[1].Value;
                            path_map[path_hash] = path_str;
                            continue;
                        }

                        var match2 = Regex.Match(line, "NameHash: \"(.+?)\" \"(.+?)\" \"(.+?)\"");
                        if (match2.Success && match2.Groups.Count == 4)
                        {
                            var name_hash = match2.Groups[3].Value;
                            var name_str = match2.Groups[1].Value;
                            name_map[name_hash] = name_str;
                            continue;
                        }
                    }
                }
            }

            Console.WriteLine("Write list...");

            using (var stream = File.CreateText("HxNames.lst"))
            {
                foreach (var item in path_map)
                {
                    stream.WriteLine($"{item.Key}:{item.Value}");
                }

                foreach (var item in name_map)
                {
                    stream.WriteLine($"{item.Key}:{item.Value}");
                }
            }
        }
    }
}