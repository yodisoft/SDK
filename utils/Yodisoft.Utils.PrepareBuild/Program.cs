using System;
using System.IO;
using System.Linq;

namespace Yodisoft.Utils.PrepareBuild
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // foreach (var folder in System.IO.Directory.GetDirectories(@"d:\gitdp"))
                //     Console.WriteLine(folder);
                
                
                if (args.Length != 1)
                {
                    return;
                }

                var path = args[0]; //SolutionFolder;

                foreach (var a in Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories))
                {
                    if (a.Contains(@"\bin\") 
                        || a.Contains(@"\obj\") 
                        || a.Contains(@"\.idea")
                        || a.EndsWith(@"\obj")
                        || a.EndsWith(@"\bin")
                        || a.Contains(@"\.vs"))
                    {
                        try
                        {
                            if (Directory.Exists(a))
                            {
                                Console.WriteLine("Delete: " + a);
                                Directory.Delete(a, true);
                            }
                        }

                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                        }
                    }
                }

                foreach (var a in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".user") || s.EndsWith(".log")))
                {
                    //var b = a.ToLower();
                    //if (b.Contains(@"\bin\") || b.Contains(@"\obj\"))
                    {
                        try
                        {
                            Console.WriteLine("Delete: " + a);
                            File.Delete(a);
                        }

                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
