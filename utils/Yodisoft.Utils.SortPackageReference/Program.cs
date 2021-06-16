using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Yodisoft.Utils.SortPackageReference
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    return;
                }

                var dir = args[0]; 


                var files2 = new List<string>();
                files2.AddRange(Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories));

                foreach (var fileName in files2)
                {
                    var sb = new List<string>();

                    var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                    var tecH = new List<string>();
                    fs.Seek(0, SeekOrigin.Begin);
                    var sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line.Contains("<PackageReference"))
                            {
                                var inx = line.IndexOf('<');
                                tecH.Add(line.Substring(inx));
                                line = "";
                                Console.WriteLine("->" + fileName);
                            }
                            else
                            {
                                if (tecH.Any())
                                {
                                    tecH.Sort((x, y) => string.Compare(x, y, StringComparison.CurrentCulture));
                                    foreach (var s in tecH)
                                    {
                                        sb.Add("    " + s);
                                    }
                                }

                                tecH.Clear();
                            }

                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                sb.Add(line);
                            }
                        }
                        else
                        {
                            sb.Add(line);
                        }
                    }

                    var sb2 = new StringBuilder();
                    foreach (var ss in sb)
                    {
                        sb2.AppendLine(ss);
                    }


                    fs.Close();
                    File.Delete(fileName);

                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(sb2.ToString());
                        sw.Close();
                    }
                }

                Console.WriteLine("Stop");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.WriteLine("Press any key");
                Console.ReadKey();
            }
        }
    }
}