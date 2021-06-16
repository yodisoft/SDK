using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Yodisoft.Utils.Rename
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var flagCopyright = false;
                
                var str_from = "ContainerWrapper.Resolve"; 
                var str_to = "ContainerWrapper.GetRequiredService";  //

                //Include="Autofac" Version="4.9.4"
                //Version="1.4.0-beta1"
                
                
                  // var mm = "Akka.Serialization.Hyperion";
                  // var str_from = "Include=\"" + mm + "\" Version=\"1.4.19";
                  // var str_to = "Include=\"" + mm + "\" Version=\"1.4.20";
                  //

//                var str_from = " Version=\"1.4.0-beta1";
//                var str_to = " Version=\"1.4.0-beta3";

                //var dir =   @"C:\1";//  @"C:\gitdp";
                var dir = @"d:\gitdp";
                var listChangedProj = new List<string>();
                var files2 = new List<string>();
                files2.AddRange(Directory.GetDirectories(dir, "*", SearchOption.AllDirectories));
                foreach (var fileName in files2.Where(o => !o.Contains(".idea")))
                {
                    if (!fileName.Contains(@"\ignore\"))
                    {
                        if (fileName.Contains(str_from))
                        {
                            var nn = fileName.Replace(str_from, str_to);
                            try
                            {
                                Directory.Move(fileName, nn);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                files2.Clear();
                files2.AddRange(Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories));
                foreach (var fileName in files2)
                {
                    if (!fileName.Contains(@"\ignore\"))
                    {
                        if (fileName.Contains(str_from))
                        {
                            var nn = fileName.Replace(str_from, str_to);
                            try
                            {
                                File.Move(fileName, nn);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                files2.Clear();
                files2.AddRange(Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories));
                foreach (var fileName in files2)
                {
                    if (!fileName.Contains(@"\ignore\"))
                    {
                        var flag = false;
                        var sb = new List<string>();
                        var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        fs.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(fs);
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (line.Contains(str_from))
                                {
                                    line = line.Replace(str_from, str_to);
                                    if (!listChangedProj.Contains(fileName))
                                    {
                                        listChangedProj.Add(fileName);
                                    }
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
                            sw.Write(sb2.ToString());
                            sw.Close();
                        }
                    }
                }
                //clean string containt 
                // <Copyright>
                //     <Company>
                //     <Authors>

                if (flagCopyright)
                {
                    files2.Clear();
                    files2.AddRange(Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories));
                    foreach (var fileName in files2)
                    {
                        if (!fileName.Contains(@"\ignore\"))
                        {
                            var sb = new List<string>();
                            var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                            fs.Seek(0, SeekOrigin.Begin);
                            var sr = new StreamReader(fs);
                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine();
                                if (!string.IsNullOrEmpty(line))
                                {
                                    var fff = false;
                                    if (line.Contains("<Copyright>"))
                                    {
                                        line = "        <Copyright>~</Copyright>";
                                        fff = true;
                                    }
                                    if (line.Contains("<Company>"))
                                    {
                                        line = "        <Company>~</Company>";
                                        fff = true;
                                    }
                                    if (line.Contains("<Authors>"))
                                    {
                                        line = "        <Authors>~</Authors>";
                                        fff = true;
                                    }
                                    if (!string.IsNullOrWhiteSpace(line))
                                    {
                                        sb.Add(line);
                                    }

                                    if (fff)
                                    {
                                        if (!listChangedProj.Contains(fileName))
                                        {
                                            listChangedProj.Add(fileName);
                                        }
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
                                sw.Write(sb2.ToString());
                                sw.Close();
                            }
                        }
                    }
                }

                //remove string containt 
                //<ProjectGuid>

                
                files2.Clear();
                files2.AddRange(Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories));
                foreach (var fileName in files2)
                {
                    if (!fileName.Contains(@"\ignore\"))
                    {
                        var sb = new List<string>();
                        var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        fs.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(fs);
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (line.Contains("<ProjectGuid>"))
                                {
                                    line = string.Empty;
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
                            sw.Write(sb2.ToString());
                            sw.Close();
                        }
                    }
                }
                

                //----add ProjectGuid
                if (false)
                {
                    files2.Clear();
                    files2.AddRange(Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories));
                    foreach (var fileName in files2)
                    {
                        var sb2 = new StringBuilder();
                        var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        fs.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(fs);
                        while (!sr.EndOfStream)
                        {
                            var flag = false;
                            var line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (line.Contains("<Version>") && line.Contains("</Version>"))
                                {
                                    flag = true;
                                }
                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    sb2.AppendLine(line);
                                }
                                if (flag)
                                {
                                    sb2.AppendLine("    <ProjectGuid>" + Guid.NewGuid().ToString().ToUpper() + "</ProjectGuid>");
                                    if (!listChangedProj.Contains(fileName))
                                    {
                                        listChangedProj.Add(fileName);
                                    }
                                }
                            }
                            else
                            {
                                sb2.AppendLine(line);
                            }
                        }
                        fs.Close();
                        File.Delete(fileName);
                        var tmp = sb2.ToString();
                        var j = tmp.IndexOf("</Project>") + 10;
                        var res = tmp.Substring(0, j);
                        using (StreamWriter sw = File.CreateText(fileName))
                        {
                            sw.Write(res);
                            sw.Close();
                        }
                    }
                }
                //-------------------
                foreach (var fileName in listChangedProj)
                {
                    Console.WriteLine(">> " + fileName);
                    var flag = false;
                    var sb = new List<string>();
                    var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs.Seek(0, SeekOrigin.Begin);
                    var sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line.Contains("<Version>") && line.Contains("</Version>"))
                            {
                                var vv = line.Replace("<Version>", "");
                                vv = vv.Replace("</Version>", "");
                                vv = vv.Replace(" ", "");
                                var newVer = Version.Parse(vv);
                                newVer = UpVer(newVer);
                                line = "    <Version>" + newVer + "</Version>";
                                Console.WriteLine("--> Up version project to " + newVer);
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
                        sw.Write(sb2.ToString());
                        sw.Close();
                    }
                }
                files2 = new List<string>();
                files2.AddRange(Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories));
                foreach (var fileName in files2)
                {
                    if (!fileName.Contains(@"\ignore\"))
                    {
                        var flag = false;
                        var sb = new List<string>();
                        var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        fs.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(fs);
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (line.Contains(str_from))
                                {
                                    line = line.Replace(str_from, str_to);
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
                            sw.Write(sb2.ToString());
                            sw.Close();
                        }
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

        private static Version UpVer(Version ver)
        {
            return new Version(ver.Major, ver.Minor, ver.Build + 1);
        }
    }
}