using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Yodisoft.Utils.CheckVersion
{
    internal class Hff
    {
        public Version Version;

        public List<string> Projects;
    }
    
    internal static class Program
    {
        static void Main(string[] args)
        {
            try
            {
//                if (args.Length != 1)
//                {
//                    return;
//                }
                //var dir = Path.GetDirectoryName( Assembly.GetEntryAssembly().Location);//args[0]; //SolutionFolder;

                var dir = @"D:/gitdp/";
                
                Console.WriteLine("## work folder=" + dir);
                
                var totalPkg = new Dictionary<string, List<Hff>>();

//                dir = @"c:\gitlab";
                var sb2 = new StringBuilder();

                
                var ai = new List<AssemblyItem>();
                foreach (var fileName in GetFilteredFiles(dir, "*.csproj", SearchOption.AllDirectories))
                {
                    var k = fileName.Split('\\');
                    var h3 = k[k.Length - 1];
                    var pn = h3.Replace(".csproj", "");

                    var tecAi = new AssemblyItem();
                    tecAi.Name = pn;
                    tecAi.Ver = new Version(0, 0, 0);

                    var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                    fs.Seek(0, SeekOrigin.Begin);
                    var sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {

                            if (line.Contains("<GeneratePackageOnBuild>true</GeneratePackageOnBuild>"))
                            {
                                tecAi.IsPkg = true;
                            }


                            if (line.Contains("<Version>") && line.Contains("</Version>"))
                            {
                                tecAi.Ver = Version.Parse(line.Replace("<Version>", "").Replace("</Version>", "").Replace(" ", ""));
                            }

                            if (tecAi.Ver == Version.Parse("4.0.9"))
                            {

                            }

                            if (line.Contains("<PackageReference Include"))
                            {
                                var h = line.Split('"');
                                var vv = h[3].Split('-');
                                var tecPkg = new Pkg {Name = h[1], Ver = Version.Parse(vv[0])};
                                tecAi.Pkgs.Add(tecPkg);

                                var key = h[1];
                                if (totalPkg.TryGetValue(key, out var listA))
                                {
                                    var hh = listA.FirstOrDefault(o => o.Version == tecPkg.Ver);
                                    if (hh == null)
                                    {
                                        var jjj = new Hff();
                                        jjj.Version = tecPkg.Ver;
                                        jjj.Projects = new List<string>();
                                        jjj.Projects.Add(pn);
                                        listA.Add(jjj);
                                    }
                                    else
                                    {
                                        if (!hh.Projects.Contains(pn))
                                        {
                                            hh.Projects.Add(pn);
                                        }
                                    }
                                }
                                else
                                {
                                    var www = new Hff();
                                    www.Version = tecPkg.Ver;
                                    www.Projects = new List<string>();
                                    www.Projects.Add(pn);

                                    var kkk = new List<Hff>();
                                    kkk.Add(www);

                                    totalPkg.Add(key, kkk);
                                }
                            }

                            if (line.Contains("<ProjectReference Include"))
                            {
                                var h = line.Split('"');
                                var g = h[1].Split('\\');
                                var dep = g[2].Replace(".csproj", "");
                                tecAi.Depends.Add(new Pkg {Name = dep, Ver = Version.Parse("0.0.0")});
                            }
                        }
                        else
                        {
                            //sb.AppendLine(line);
                        }
                    }
                    var ttt = tecAi.Name + ":" + tecAi.Ver.Major + "." + tecAi.Ver.Minor + "." + tecAi.Ver.Build;
                    sb2.AppendLine(ttt);
                    ai.Add(tecAi);
                    fs.Close();
                }

                foreach (var ff in ai)
                {
                    foreach (var cc in ff.Depends)
                    {
                        if (cc.Ver == Version.Parse("0.0.0"))
                        {
                            var tmpx = ai.FirstOrDefault(o => o.Name == cc.Name);
                            if (tmpx != null)
                            {
                                cc.Ver = tmpx.Ver;
                            }
                        }
                    }
                }

                var tmp = FindTop(ai);
                var ggg = new List<AssemblyItem>();
                var ggg2 = new List<AssemblyItem>();
                foreach (var t in tmp)
                {
                    var h = t.Name.Split('.');
                    if (h[h.Length - 1] != "Test")
                    {
                        ggg2.Add(t);
                    }
                    else
                    {
                        ggg.Add(t);
                    }
                }
                
                
                var nm01 = "1_top.log";
                if (File.Exists(nm01))
                {
                    File.Delete(nm01);
                }
                using (StreamWriter sw = File.CreateText(nm01))
                {
                    sw.WriteLine("Top(" + ggg2.Count + "):");
                    foreach (var t in ggg2)
                    {
                        sw.WriteLine("-> " + t.Name + " ver=" + t.Ver);
                    }
                    sw.Close();
                }

                var nm012 = "1_test.log";
                if (File.Exists(nm012))
                {
                    File.Delete(nm012);
                }
                using (StreamWriter sw = File.CreateText(nm012))
                {
                    sw.WriteLine("Test(" + ggg.Count + "):");
                    foreach (var t in ggg)
                    {
                        sw.WriteLine("-> " + t.Name + " ver=" + t.Ver);
                    }
                    sw.Close();
                }

                var items = new List<Item>();
                foreach (var t in tmp)
                {
                    AddSub2(ai, "", t, items);
                }

                foreach (var item in items)
                {
                    AddToParent(items, item, item.Name);
                    MakeSelfSub(items, item);
                }


                // Console.WriteLine("Redundantly");
                
                var nm0 = "1_redundantly.log";
                if (File.Exists(nm0))
                {
                    File.Delete(nm0);
                }
                using (StreamWriter sw = File.CreateText(nm0))
                {

                    foreach (var item in items)
                    {
                        var list = new List<string>();
                        foreach (var sub in item.SelfSub)
                        {
                            var subInst = items.FirstOrDefault(o => o.Name == sub);
                            if (subInst != null)
                            {
                                foreach (var sub2 in item.SelfSub)
                                {
                                    foreach (var kk in subInst.AllSub)
                                    {
                                        if (sub2 == kk)
                                        {
                                            list.Add(kk + "(" + sub + ")");
                                        }
                                    }
                                }
                            }
                        }

                        if (list.Any())
                        {
                            sw.WriteLine(item.Name);
                            foreach (var s in list)
                            {
                                sw.WriteLine("--- " + s);
                            }
                        }
                    }

                    sw.Close();
                }

//                 
//                 foreach (var item in items)
//                 {
//                     var list = new List<string>();
//                     foreach (var sub in item.SelfSub)
//                     {
//                         var subInst = items.FirstOrDefault(o => o.Name == sub);
//                         if (subInst != null)
//                         {
//                             foreach (var sub2 in item.SelfSub)
//                             {
//                                 foreach (var kk in subInst.AllSub)
//                                 {
//                                     if (sub2 == kk)
//                                     {
//                                         list.Add(kk + "(" + sub + ")");
//                                     }
//                                 }
//                             }
//                         }
// //                        
// //                        
// //                        var tmp2 = item.AllSub.Where(o => o == sub).ToArray();
// //                        if (tmp2.Length > 1)
// //                        {
// //                            list.Add(sub);
// //                            //Console.WriteLine(item.Name + " redundantly " + sub);
// //                        }
//                     }
//
//                     if (list.Any())
//                     {
//                         Console.WriteLine(item.Name);
//                         foreach (var s in list)
//                         {
//                             Console.WriteLine("--- " + s);
//                         }
//                     }
//                 }
//                Console.WriteLine("---------------------------------");

                var toChange = new List<Pkg>();

                foreach (var item in items)
                {
                    var hh = ai.FirstOrDefault(o => o.Name == item.Name);
                    if (hh != null)
                    {
                        if (hh.Ver != item.Version)
                        {
                            var mm = toChange.FirstOrDefault(o => o.Name == hh.Name);
                            if (mm == null)
                            {
                                toChange.Add(new Pkg() {Name = hh.Name, Ver = hh.Ver});
                            }
                        }
                    }
                }

                foreach (var t in toChange)
                {
                    Console.WriteLine("Using version " + t.Name + "  need change to  " + t.Ver);
                }

                foreach (var t in toChange)
                {
                    SetToUp(items, t.Name);
                }

//                Console.WriteLine("---------------------------------");

                var toUp = new List<string>();
                foreach (var t in items.Where(o => o.ToUp))
                {
                    var hh = toUp.FirstOrDefault(o => o == t.Name);
                    if (hh == null)
                    {
                        var dd = toChange.FirstOrDefault(o => o.Name == t.Name);
                        if (dd == null)
                        {
                            toUp.Add(t.Name);
                        }
                    }
                }

                foreach (var t in toUp)
                {
                    var bb = ai.FirstOrDefault(o => o.Name == t);
                    if (bb != null)
                    {
                        toChange.Add(new Pkg() {Name = bb.Name, Ver = UpVer(bb.Ver)});
                    }
                }

                foreach (var t in toChange)
                {
                    Console.WriteLine(t.Name + " up version to " + t.Ver);
                }

//                Console.WriteLine("---------------------------------");


                foreach (var fileName in GetFilteredFiles(dir, "*.csproj", SearchOption.AllDirectories))
                {
                    Pkg gg = null;
                    foreach (var hh in toChange)
                    {
                        if (fileName.Contains(hh.Name + ".csproj"))
                        {
                            gg = hh;
                            break;
                        }
                    }

                    if (gg != null)
                    {
                        var sb3 = new StringBuilder();

                        Console.WriteLine("In work: " + fileName);
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
                                    line = "    <Version>" + gg.Ver + "</Version>";
                                    Console.WriteLine("--> Up version project to " + gg.Ver);
                                }

                                foreach (var hh in toChange)
                                {
                                    if (line.Contains("<PackageReference Include"))
                                    {
                                        if (line.Contains("\"" + hh.Name + "\""))
                                        {
                                            line = "    <PackageReference Include=\"" + hh.Name + "\" Version=\"" + hh.Ver + "\" />";
                                            Console.WriteLine("--> Up version PackageReference  " + hh.Name + "  to " + hh.Ver);
                                        }
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    sb3.AppendLine(line);
                                }
                            }
                            else
                            {
                                sb3.AppendLine(line);
                            }
                        }

                        fs.Close();
                        File.Delete(fileName);

                        using (StreamWriter sw = File.CreateText(fileName))
                        {
                            sw.Write(sb3.ToString());
                            sw.Close();
                        }
                    }
                }
                
                var files4 = new List<string>();
                files4.AddRange(Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories));
                foreach (var fileName in files4)
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

                    var sb5 = new StringBuilder();
                    foreach (var ss in sb)
                    {
                        sb5.AppendLine(ss);
                    }


                    fs.Close();
                    File.Delete(fileName);

                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine(sb5.ToString());
                        sw.Close();
                    }
                }

                
                
                
                foreach (var fileName in GetFilteredFiles(dir, "*.csproj", SearchOption.AllDirectories))
                {

                        var sb3 = new StringBuilder();

                        Console.WriteLine("In work: " + fileName);
                        var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                        fs.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(fs);
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {



                                if (line.Contains("<Project") ||
                                    line.Contains("</Project") ||
                                    line.Contains("<?xml"))
                                {
                                    
                                }
                                else
                                {
                                    if (line.Contains("<PropertyGroup") ||
                                        line.Contains("</PropertyGroup") ||
                                        line.Contains("<ItemGroup") ||
                                        line.Contains("</ItemGroup"))
                                    {
                                        line = line.Trim();
                                        line = "  " + line;
                                    }
                                    else
                                    {
                                        line = line.Trim();
                                        line = "    " + line;
                                    }
                                }


                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    sb3.AppendLine(line);
                                }
                            }
                            else
                            {
                               // sb3.AppendLine(line);
                            }
                        }

                        fs.Close();
                        File.Delete(fileName);

                        using (StreamWriter sw = File.CreateText(fileName))
                        {
                            sw.Write(sb3.ToString());
                            sw.Close();
                        }
                }
                
                
                
                
                
                
                
                
//                Console.WriteLine("--------------tec_rebuild.cmd-------------------");

                using (StreamWriter sw = File.CreateText("tec_rebuild.cmd"))
                {
                    sw.Write(MakeD(items, toChange, dir, false).ToString());
                    sw.Close();
                }
//                Console.WriteLine("-------------ggggg--------------------");

//                var toB = new List<Pkg>();
//                
//                foreach (var gg in ai.Select(o=>o.Name).Distinct())
//                {
//                    toB.Add(new Pkg(){Name = gg});
//                }
                using (StreamWriter sw = File.CreateText("auto_build_all_proj.cmd"))
                {
                    sw.Write(MakeD(items, null, dir, false).ToString());
                    sw.Close();
                }
                using (StreamWriter sw = File.CreateText("auto_build_all_test.cmd"))
                {
                    sw.Write(MakeD(items, null, dir, true).ToString());
                    sw.Close();
                }
//                if (false)
//                {
//                    var nm = "list_tec.txt";
//                    if (File.Exists(nm))
//                    {
//                        File.Delete(nm);
//                    }
//                    using (StreamWriter sw = File.CreateText(nm))
//                    {
//                        var fff = MakeE(items, toChange, dir, false).ToString();
//                        sw.Write(fff);
//                        sw.Close();
//                    }
//
//                    nm = "list.txt";
//                    if (File.Exists(nm))
//                    {
//                        File.Delete(nm);
//                    }
//                    using (StreamWriter sw = File.CreateText(nm))
//                    {
//                        var fff = MakeE(items, null, dir, false).ToString();
//                        sw.Write(fff);
//                        sw.Close();
//                    }
//                }

//                if (false)
//                {
//                    var nm = "images.txt";
//                    if (File.Exists(nm))
//                    {
//                        File.Delete(nm);
//                    }
//                    using (StreamWriter sw = File.CreateText(nm))
//                    {
//                        foreach (var fileName in GetFilteredFiles(dir, "Dockerfile", SearchOption.AllDirectories))
//                        {
//                            var d = Path.GetDirectoryName(fileName);
//                            var kk = d.IndexOf(@"\src", StringComparison.Ordinal);
//                            var gg = d.Substring(kk);
//                            d = gg.Replace(@"\", "/");
//                            sw.WriteLine(d);
//                        }
//                        sw.Close();
//                    }
//                }


//                Console.WriteLine("---------------unique-------------------");
                var nm = "1_unique.log";
                if (File.Exists(nm))
                {
                    File.Delete(nm);
                }
                using (StreamWriter sw = File.CreateText(nm))
                {


                    var sortedDict = from entry in totalPkg orderby entry.Key ascending select entry;
                    foreach (var sss in sortedDict)
                    {
                        if (sss.Value.Count == 1)
                        {
                            var sx2 = string.Empty;
                            foreach (var nn in sss.Value)
                            {
//                            var sx = string.Empty;
//                            var j = 0;
//                            while (j < nn.Projects.Count)
//                            {
//                                if (!string.IsNullOrEmpty(sx))
//                                {
//                                    sx += ", ";
//                                }
//                                sx +=nn.Projects[j];
//                                j++;
//                            }

                                //foreach (var vvv in nn)
                                {
                                    sw.WriteLine(sss.Key + " [" + nn.Version + "] (" + nn.Projects.Count + ")");
                                }

                                foreach (var pr in nn.Projects)
                                {
                                    sw.WriteLine(Dfd(sss.Key + " [" + nn.Version + "] )") + pr);
                                }
                            }


                            //Console.WriteLine(sss.Key+ "  "+sx2);
                        }
                    }
                    sw.Close();
                }
//                Console.WriteLine("---------------different------------------");
                nm = "1_different.log";
                if (File.Exists(nm))
                {
                    File.Delete(nm);
                }
                using (StreamWriter sw = File.CreateText(nm))
                {
                    foreach (var sss in totalPkg)
                    {
                        if (sss.Value.Count > 1)
                        {
                            sw.WriteLine(sss.Key);

                            foreach (var vvv in sss.Value)
                            {
                                var sx = string.Empty;
                                var j = 0;
                                while (j < vvv.Projects.Count)
                                {
                                    if (!string.IsNullOrEmpty(sx))
                                    {
                                        sx += ", ";
                                    }
                                    sx += vvv.Projects[j];
                                    j++;
                                }
                                sw.WriteLine("   [" + vvv.Version + "]  " + sx);
                            }

                        }
                    }
                    sw.Close();
                }
//                Console.WriteLine("------------5555---------------------");


//                foreach (var pk in totalPkg)
//                {
//                    IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
//                    //IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("http://192.168.99.100:5055/");//("https://packages.nuget.org/api/v2");
//                    IQueryable<IPackage> packages = repo.FindPackagesById(pk.Key).AsQueryable();
//
//                    var packages2 = packages
//                        .Where(item => (item.IsReleaseVersion() == true))
//                        .Where(o => o.IsLatestVersion)
//                        .ToArray();
//
//                    foreach (var vv in packages2)
//                    {
////                        Console.WriteLine(vv.Id + "["+vv.Version+"]");
////                        foreach (var ds in vv.DependencySets)
////                        {
////                            Console.WriteLine("   "+ds.TargetFramework);
////                            foreach (var dds in ds.Dependencies)
////                            {
////                                Console.WriteLine("      "+dds.Id  + "["+dds.VersionSpec+"]");
////                            }
////                            
////                        }
//                        if (pk.Value.Count == 1)
//                        {
//                            foreach (var hh in pk.Value)
//                            {
//                                if (hh.Version.ToString() != vv.Version.ToString())
//                                {
//                                    Console.WriteLine(pk.Key + " > [" + hh.Version + "] ... [" + vv.Version + "]");
//                                }
//                            }
//                        }
//                    }
//                    
////                    foreach (var vv in packages2)
////                    {
////                        Console.WriteLine(vv.Id + "["+vv.Version+"]");
////                        foreach (var ds in vv.DependencySets)
////                        {
////                            Console.WriteLine("   "+ds.TargetFramework);
////                            foreach (var dds in ds.Dependencies)
////                            {
////                                Console.WriteLine("      "+dds.Id  + "["+dds.VersionSpec+"]");
////                            }
////                            
////                        }
////                    }
//
//                }


//                Console.WriteLine("-------------------list_tec.txt--------------------------");
                nm = "list_tec.txt";
                if (File.Exists(nm))
                {
                    File.Delete(nm);
                }
                using (StreamWriter sw = File.CreateText(nm))
                {
                    var fff = MakeE(items, toChange, dir, false).ToString();
                    sw.Write(fff);
                    sw.Close();
                }
//                Console.WriteLine("------------------list.txt---------------------------");
                nm = "list.txt";
                if (File.Exists(nm))
                {
                    File.Delete(nm);
                }
                using (StreamWriter sw = File.CreateText(nm))
                {
                    var fff = MakeE(items, null, dir, false).ToString();
                    sw.Write(fff);
                    sw.Close();
                }  
                //---------------------------------------------
                
                
                
//                Console.WriteLine("Stop");

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.WriteLine("Press any key");
                Console.ReadKey();
            }
        }

        private static string Dfd(string dd)
        {
            var s = string.Empty;
            var j = 0;
            while (j < dd.Length)
            {
                s += " ";
                j++;
            }
            return s;
        }

        private static void SetToUp(List<Item> items, string name)
        {
            var tmp = items.Where(o => o.Name == name).ToArray();
            foreach (var t in tmp)
            {
                t.ToUp = true;
                SetToUp(items, t.UpName);
            }
        }
        
                private static StringBuilder MakeE(List<Item> items2, List<Pkg> toChange2, string dir, bool isTest)
        {
                            var sb4 = new StringBuilder();
//                            
//                            sb4.AppendLine("Yodisoft.Utils.PrepareBuild.exe .");
//
//                sb4.AppendLine("dotnet nuget locals all --clear");
//                sb4.AppendLine("");

//                var tec = new List<Pkg>();
//                    tec.AddRange(toChange2);

                    var hhhh = new List<Item>();
                    hhhh.AddRange(items2);
                    
                    //Console.WriteLine("tec > " +tec.Count);
                    //Console.WriteLine("hhhh> " +hhhh.Count);
                    
                var sort = new List<Item>();

                while (hhhh.Count>0)
                {
                    Item mm = null;
                    foreach (var ss in hhhh)
                    {
                        var oo = hhhh.Where(o => o.UpName == ss.Name).ToArray();
                        if (!oo.Any())
                        {
                            //Console.WriteLine("0> " + ss.Name);
                            mm = ss;
                            break; 
                        }
                    }

                    if (mm != null)
                    {
                        //Console.WriteLine("1> "+hhhh.Count);
                        var jjj = hhhh.Where(o => o.Name == mm.Name).ToArray();
                        foreach (var h in jjj)
                        {
                            hhhh.Remove(h);
                        }
                        //Console.WriteLine("2> "+hhhh.Count);
                        //Console.WriteLine("3> "+tec.Count);
//                        var aaa = tec.Where(o => o.Name == mm.Name).ToArray();
//                        foreach (var h in aaa)
//                        {
//                            tec.Remove(h);
//                        }
                        //Console.WriteLine("4> "+tec.Count);
                        
                        sort.Add(mm);
                    }
                    //Console.WriteLine("5> "+tec.Count);
                }
                //Console.WriteLine("sort ========="+sort.Count);

                var fnms = new List<string>();
                foreach (var fileName in GetFilteredFiles(dir, "*.csproj", SearchOption.AllDirectories))
                {
                    fnms.Add(fileName);
                }

                if (isTest)
                {
                    fnms = fnms.Where(o => o.Contains(".Test.")).ToList();
                }
                else
                {
                    fnms = fnms.Where(o => !o.Contains(".Test.")).ToList();
                }
                
                //Console.WriteLine("fnms ========="+fnms.Count);

                var dirs = new List<string>();

                if (toChange2 != null)
                {
                    var sort2 = new List<Item>();
                    foreach (var ff in sort)
                    {
                        foreach (var aa in toChange2)
                        {
                            if (ff.Name == aa.Name)
                            {
                                sort2.Add(ff);
                            }
                        }
                    }

                    sort = sort2;
                }
                
                
                foreach (var sss in sort)
                {
                    var hhh = fnms.FirstOrDefault(o => o.Contains(sss.Name + ".csproj"));
                    if (!string.IsNullOrEmpty(hhh))
                    { 
                        //Console.WriteLine(hhh);
                        hhh = hhh.Replace("\\" + sss.Name + ".csproj", "");
                        //var k = hhh.LastIndexOf("\\", StringComparison.InvariantCulture);
                        //hhh = hhh.Substring(0, k);
                        if (!dirs.Contains(hhh))
                        {
                            //Console.WriteLine(hhh);
                            dirs.Add(hhh);
                        }
                    }
                }

//                if (isTest)
//                {
//                    foreach (var d in dirs)
//                    {
//                        sb4.AppendLine(d);
////                        sb4.AppendLine("cd " + d);
////                        sb4.AppendLine("call c:\\build\\test.cmd");
////                        sb4.AppendLine("dotnet build");
////                        sb4.AppendLine("set fl=errors.log");
////                        sb4.AppendLine("@set Arg1=%dr%%fl%");
////                        sb4.AppendLine("@for %%i in (%Arg1%) do (set /a size1=%%~Zi)");
////                        sb4.AppendLine("if %size1% gtr 0 exit 1");
////                        sb4.AppendLine("dotnet test");
////                        sb4.AppendLine("");
//                    }     
//                }
//                else
                {
                    // foreach (var d in dirs)
                    // {
                    //     if (!string.IsNullOrEmpty(d))
                    //     {
                    //         var kk = d.IndexOf(@"\src", StringComparison.Ordinal);
                    //         var gg = d.Substring(kk);
                    //         sb4.AppendLine(gg.Replace(@"\", "/") .Replace("/src/", ""));
                    //     }
                    // }                    
                }
                //sb4.AppendLine("");
                return sb4;
        }

        private static StringBuilder MakeD(List<Item> items2, List<Pkg> toChange2, string dir, bool isTest)
        {
                            var sb4 = new StringBuilder();
                            
                            sb4.AppendLine("Yodisoft.Utils.PrepareBuild.exe .");

                sb4.AppendLine("dotnet nuget locals all --clear");
                sb4.AppendLine("");

//                var tec = new List<Pkg>();
//                    tec.AddRange(toChange2);

                    var hhhh = new List<Item>();
                    hhhh.AddRange(items2);
                    
                    //Console.WriteLine("tec > " +tec.Count);
                    //Console.WriteLine("hhhh> " +hhhh.Count);
                    
                var sort = new List<Item>();

                while (hhhh.Count>0)
                {
                    Item mm = null;
                    foreach (var ss in hhhh)
                    {
                        var oo = hhhh.Where(o => o.UpName == ss.Name).ToArray();
                        if (!oo.Any())
                        {
                            //Console.WriteLine("0> " + ss.Name);
                            mm = ss;
                            break; 
                        }
                    }

                    if (mm != null)
                    {
                        //Console.WriteLine("1> "+hhhh.Count);
                        var jjj = hhhh.Where(o => o.Name == mm.Name).ToArray();
                        foreach (var h in jjj)
                        {
                            hhhh.Remove(h);
                        }
                        //Console.WriteLine("2> "+hhhh.Count);
                        //Console.WriteLine("3> "+tec.Count);
//                        var aaa = tec.Where(o => o.Name == mm.Name).ToArray();
//                        foreach (var h in aaa)
//                        {
//                            tec.Remove(h);
//                        }
                        //Console.WriteLine("4> "+tec.Count);
                        
                        sort.Add(mm);
                    }
                    //Console.WriteLine("5> "+tec.Count);
                }
                //Console.WriteLine("sort ========="+sort.Count);

                var fnms = new List<string>();
                foreach (var fileName in GetFilteredFiles(dir, "*.csproj", SearchOption.AllDirectories))
                {
                    fnms.Add(fileName);
                }

                if (isTest)
                {
                    fnms = fnms.Where(o => o.Contains(".Test.")).ToList();
                }
                else
                {
                    fnms = fnms.Where(o => !o.Contains(".Test.")).ToList();
                }
                
                //Console.WriteLine("fnms ========="+fnms.Count);

                var dirs = new List<string>();

                if (toChange2 != null)
                {
                    var sort2 = new List<Item>();
                    foreach (var ff in sort)
                    {
                        foreach (var aa in toChange2)
                        {
                            if (ff.Name == aa.Name)
                            {
                                sort2.Add(ff);
                            }
                        }
                    }

                    sort = sort2;
                }
                
                
                foreach (var sss in sort)
                {
                    var hhh = fnms.FirstOrDefault(o => o.Contains(sss.Name + ".csproj"));
                    if (!string.IsNullOrEmpty(hhh))
                    { 
                        //Console.WriteLine(hhh);
                        hhh = hhh.Replace("\\" + sss.Name + ".csproj", "");
                        //var k = hhh.LastIndexOf("\\", StringComparison.InvariantCulture);
                        //hhh = hhh.Substring(0, k);
                        if (!dirs.Contains(hhh))
                        {
                            //Console.WriteLine(hhh);
                            dirs.Add(hhh);
                        }
                    }
                }

                if (isTest)
                {
                    foreach (var d in dirs)
                    {
                        sb4.AppendLine("cd " + d);
                        sb4.AppendLine("call d:\\build\\test.cmd");
//                        sb4.AppendLine("dotnet build");
//                        sb4.AppendLine("set fl=errors.log");
//                        sb4.AppendLine("@set Arg1=%dr%%fl%");
//                        sb4.AppendLine("@for %%i in (%Arg1%) do (set /a size1=%%~Zi)");
//                        sb4.AppendLine("if %size1% gtr 0 exit 1");
//                        sb4.AppendLine("dotnet test");
                        sb4.AppendLine("");
                    }     
                }
                else
                {
                    foreach (var d in dirs)
                    {
                        sb4.AppendLine("cd " + d);
                        sb4.AppendLine("call d:\\build\\build.cmd");
//                        sb4.AppendLine("set fl=errors.log");
//                        sb4.AppendLine("@set Arg1=%dr%%fl%");
//                        sb4.AppendLine("@for %%i in (%Arg1%) do (set /a size1=%%~Zi)");
//                        sb4.AppendLine("if %size1% gtr 0 exit 1");
                        sb4.AppendLine("");
                    }                    
                }


                sb4.AppendLine("echo ===================== OK =========================");
                sb4.AppendLine("pause");

                return sb4;
        }
        
//        private static void MakeParents(List<Item> items, string name, Item ownerItem)
//        {
//            var tmp = items.Where(o => o.Name == name).Select(o=>o.UpName).ToArray();
//            if (tmp.Any())
//            {
//                ownerItem.Parents.AddRange(tmp);
//                foreach (var nm in tmp)
//                {
//                    MakeParents(items, nm, ownerItem);
//                }
//            }
//        }

        private static void MakeSelfSub(List<Item> items, Item item)
        {
            var tmp = items.Where(o => o.UpName == item.Name).Select(o=>o.Name).ToArray();
            if (tmp.Any())
            {
                item.SelfSub.AddRange(tmp);
            }
        }

        private static void AddToParent(List<Item> items,  Item item, string addon)
        {
            var tmp = items.FirstOrDefault(o => o.Name == item.UpName);
            if (tmp != null)
            {
                tmp.AllSub.Add(addon);
                AddToParent(items, tmp, addon);
            }
        }
        
        private static void AddSub2(List<AssemblyItem> ai, string upName,  AssemblyItem m, List<Item> items)
        {
            if (m.Ver == Version.Parse("4.0.9"))
            {
                                    
            }
            var hh = items.FirstOrDefault(o => o.Name == m.Name && o.UpName == upName);
            if (hh == null)
            {
                var version = Version.Parse("3.0.0");
                var kk = ai.FirstOrDefault(o => o.Name == upName);
                if (kk != null)
                {
                    var nn = kk.Pkgs.FirstOrDefault(o => o.Name == m.Name);
                    if (nn != null)
                    {
                        version = nn.Ver;
                    }
                    var nn3 = kk.Depends.FirstOrDefault(o => o.Name == m.Name);
                    if (nn3 != null)
                    {
                        version = nn3.Ver;

                    }
                }

                if (upName == "")
                {
                    var kk5 = ai.FirstOrDefault(o => o.Name == m.Name);
                    if (kk5 != null)
                    {
                        version = kk5.Ver;
                    }
                }
                
                items.Add(new Item() {UpName = upName, Name = m.Name, Version = version});
                foreach (var t in m.Depends)
                {
                    var tmp = ai.FirstOrDefault(o => o.Name == t.Name);
                    if (tmp != null)
                    {
                        AddSub2(ai, m.Name, tmp, items);
                    }
                }
                foreach (var t in m.Pkgs)
                {
                    var tmp = ai.FirstOrDefault(o => o.Name == t.Name);
                    if (tmp != null)
                    {
                        AddSub2(ai, m.Name, tmp, items);
                    }
                    else
                    {
                        var version2 = Version.Parse("2.0.0");
                        var kk2 = ai.FirstOrDefault(o => o.Name == upName);
                        if (kk2 != null)
                        {
                            var nn = kk2.Pkgs.FirstOrDefault(o => o.Name == m.Name);
                            if (nn != null)
                            {
                                version2 = nn.Ver;
                            }
                            var nn3 = kk2.Depends.FirstOrDefault(o => o.Name == m.Name);
                            if (nn3 != null)
                            {
                                version2 = nn3.Ver;
                            }
                        }
                        var hh3 = items.FirstOrDefault(o => o.Name == t.Name && o.UpName == m.Name);
                        if (hh3 == null)
                        {
                            items.Add(new Item() {UpName = m.Name, Name = t.Name, Version = version2});
                        }
                    }
                }
            }
        }

        private static void AddSub(List<AssemblyItem> ai, AssemblyItem m, StringBuilder sb, int tecLevel)
        {
            sb.AppendLine(GetShift(tecLevel) + m.Name);
            var level = tecLevel + 1;
            foreach (var t in m.Depends)
            {
                var tmp = ai.FirstOrDefault(o => o.Name == t.Name);
                if (tmp != null)
                {
                    AddSub(ai, tmp, sb, level);
                }
            }
            foreach (var t in m.Pkgs)
            {
                var tmp = ai.FirstOrDefault(o => o.Name == t.Name);
                if (tmp != null)
                {
                    AddSub(ai, tmp, sb, level);
                }
                else
                {
                    sb.AppendLine(GetShift(level) + t.Name);
                }
            }
        }

        private static string GetShift(int tecLevel)
        {
            var res = string.Empty;
            var j = 0;
            while (j < tecLevel)
            {
                res += "   ";
                j++;
            }
            return res;
        }
        
        

        private static AssemblyItem[] FindTop(List<AssemblyItem> ai)
        {
            var res = new List<AssemblyItem>();
            foreach (var t in ai)
            {
                if (!FindUp(ai, t))
                {
                    res.Add(t);
                }
            }
            return res.ToArray();
        }

        private static bool FindUp(List<AssemblyItem> ai, AssemblyItem m)
        {
            var res = false;
            foreach (var t in ai)
            {
                var g = t.Depends.FirstOrDefault(o => o.Name==m.Name);
                if (g != null)
                {
                    res = true;
                    break;
                }

                var f = t.Pkgs.FirstOrDefault(o => o.Name == m.Name);
                if (f != null)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        private static void CallRecurs(List<AssemblyItem> ai, string name)
        {
            
            var t = ai.FirstOrDefault(o => o.Name == name);
            if (t == null) return;
            if (!t.Flag)
            {
                //if (t != null)
                //{
                    t.Flag = true;
                //}

                foreach (var item in ai)
                {
                    var g = item.Depends.FirstOrDefault(o => o.Name==name);
                    if (g != null)
                    {
                        Console.WriteLine("проектная зависимость - "+item.Name);
                        CallRecurs(ai, item.Name);
                    }

                    var f = item.Pkgs.FirstOrDefault(o => o.Name == name);
                    if (f != null)
                    {
                        Console.WriteLine("пакетная зависимость  - " +item.Name);
                        CallRecurs(ai, item.Name);
                    }
                }
            }
        }

        private static Version UpVer(Version ver)
        {
            return new Version(ver.Major, ver.Minor, ver.Build + 1);
        }

        private static Guid[] FindGuids(string str)
        {
            var res = new List<Guid>();
            var tt = str.Split('"');

            foreach (var s in tt)
            {
                Guid rr;
                if (Guid.TryParse(s, out rr))
                {
                    res.Add(rr);
                }
            }

            return res.ToArray();
        }

        private static string[] GetFilteredFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption).Where(o=>!o.Contains(@"\ignore\")).ToArray();
        }


        private static void Recur(Item item, List<Item> items)
        {
            
        }
    }
}
