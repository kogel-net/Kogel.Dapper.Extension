using System;
using System.Collections.Generic;
using System.Text;

namespace Kogel.Dapper.Extension.Attributes
{
    public class BaseAttrbute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public BaseAttrbute(string Name = null, string Description = null)
        {
            this.Name = Name;
            this.Description = Description;
        }
    }
}
