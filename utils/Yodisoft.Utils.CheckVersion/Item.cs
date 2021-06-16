using System;
using System.Collections.Generic;

namespace Yodisoft.Utils.CheckVersion
{
    public class Item
    {
        public string Name { get; set; }
        
        public Version Version { get; set; }
        
        public string UpName { get; set; }
        
        public List<string> AllSub = new List<string>();
        
        public List<string> SelfSub = new List<string>();
        
        public bool ToUp { get; set; }
    }
}