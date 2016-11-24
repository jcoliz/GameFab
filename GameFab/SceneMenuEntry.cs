using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFab
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SceneMenuEntryAttribute : Attribute
    {
        public string Label { get; set; }
        public int Order { get; set; }
    }
}
