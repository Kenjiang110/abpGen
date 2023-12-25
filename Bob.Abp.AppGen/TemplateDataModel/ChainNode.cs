using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Templates
{
    /// <summary>
    /// Chain node. For example, if entity relativePath is "IdentityServer/Identities" 
    /// then entity belong to group "Identities" and group "Identities" belongs to "IdentityServer"
    /// </summary>
    public class ChainNode : SimpleChainNode
    {
        /// <summary>
        /// Level of this group. Root group's level is 0.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// This group's relativePath including itself.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// RelativePath but use "." as seperator char.
        /// </summary>
        public string RelativeNsPath { get; set; }

        public ChainNode() { }

        public ChainNode(ChainNode parent, string name)
        {
            Name = name;
            if (parent == null)
            {
                Level = 0;
                ParentName = null;
                RelativePath = name;
                RelativeNsPath = name;
            }
            else
            {
                Level = parent.Level + 1;
                ParentName = parent.Name;
                RelativePath = $"{parent.RelativePath}/{name}";
                RelativeNsPath = $"{parent.RelativeNsPath}.{name}";
            }
        }
    }
}
