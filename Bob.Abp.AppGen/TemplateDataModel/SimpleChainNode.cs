using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Templates
{
    /// <summary>
    /// Chain node. Chain : for example, if entity relativePath is "IdentityServer/Identities" 
    /// then entity belong to group "Identities" and group "Identities" belongs to "IdentityServer"
    /// </summary>
    public class SimpleChainNode
    {
        /// <summary>
        /// Parent group name, null means no parent.
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// Group name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Last one is leaf.
        /// </summary>
        public bool IsLast { get; set; }
    }

    public static class SimpleChainNodeExtension
    {
        public static void AddNode(this List<SimpleChainNode> nodes, string parentName, string name)
        {
            if (nodes == null) nodes = new List<SimpleChainNode>();
            if (nodes.FirstOrDefault(t => t.Name == name) == null)
            {
                if (nodes.Count > 0)
                {
                    nodes[nodes.Count - 1].IsLast = false;
                }
                nodes.Add(new SimpleChainNode { ParentName = parentName, Name = name, IsLast = true });
            }
        }
    }
}
