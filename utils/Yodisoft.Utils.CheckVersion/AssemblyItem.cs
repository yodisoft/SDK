using System;
using System.Collections.Generic;

namespace Yodisoft.Utils.CheckVersion
{
    public class AssemblyItem
    {
        public string Name { get; set; }
        
        public Version Ver { get; set; }
        
        public List<Pkg> Pkgs = new List<Pkg>();
        
        public List<Pkg> Depends = new List<Pkg>();
        
//        public DateTime LastEdit { get; set; }

        public bool IsPkg { get; set; }
        
        public bool Flag { get; set; }
    }
}