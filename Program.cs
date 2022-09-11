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
                Console.WriteLine("  GenHxHashList Dump.log ...");
                Console.WriteLine();
                Console.WriteLine("Clear mode: Clear all duplicate entries.");
                Console.WriteLine("  GenHxHashList -c Names.lst");
                Console.WriteLine();
                Console.WriteLine("Note: Support multi input file,");
                Console.WriteLine("  output will created in current directory.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            var path_map = new Dictionary<string, string>();
            var name_map = new Dictionary<string, string>();

            if (args.Length > 1)
            {
                if (args[0] == "-c")
                {
                    Console.WriteLine("Clear mode...");

                    using (var stream = File.OpenText(args[1]))
                    {
                        while (!stream.EndOfStream)
                        {
                            var line = stream.ReadLine();

                            if (line == null || line.Length == 0)
                                continue;

                            var entry = line.Split(':');

                            if (entry.Length != 2)
                                continue;

                            if (entry[0].Length == 16)
                            {
                                if (entry[1].Length > 0)
                                {
                                    path_map[entry[0]] = entry[1];
                                    continue;
                                }
                            }
                            if (entry[0].Length == 64)
                            {
                                if (entry[1].Length > 0)
                                {
                                    name_map[entry[0]] = entry[1];
                                    continue;
                                }
                            }
                        }
                    }

                    if (path_map.Count > 0 || name_map.Count > 0)
                    {
                        goto write_result;
                    }
                }
            }

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

write_result:

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