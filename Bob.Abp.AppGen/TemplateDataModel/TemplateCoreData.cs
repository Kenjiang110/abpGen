using Bob.Abp.AppGen.Models;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Templates
{
    /// <summary>
    /// Template main data.
    /// </summary>
    public class TemplateCoreData
    {
        /// <summary>
        /// Root namespace.
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// Module name.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Entity name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Group Path == entity files' relative path
        /// </summary>
        public string[] EntityGroups { get; set; }

        /// <summary>
        /// Group path. For example, if relativePath is "IdentityServer/Identities" then entity belong to group "Identities"
        /// and group "Identities" belongs to "IdentityServer", and this property should be (null, "IdentityServer"), ((null, "IdentityServer"), "Identities").
        /// </summary>
        public ChainNode[] EntityGroupChain { get; set; }

        /// <summary>
        /// The last enttiy group.
        /// </summary>
        public ChainNode LastEntityGroup { get; set; }

        /// <summary>
        /// The first enttiy group.
        /// </summary>
        public ChainNode FirstEntityGroup { get; set; }

        public TemplateCoreData()
        {
        }

        public TemplateCoreData(string rootNamesapce, string moduleName, string entityName, string[] entityGroups)
        {
            RootNamespace = rootNamesapce;
            ModuleName = moduleName;
            EntityName = entityName;
            EntityGroups = entityGroups;
            EntityGroupChain = GetEntityGroupChain(entityGroups);
            LastEntityGroup = EntityGroupChain.LastOrDefault();
            FirstEntityGroup = EntityGroupChain.FirstOrDefault();
        }

        protected ChainNode[] GetEntityGroupChain(string[] entityGroups)
        {
            var groups = new List<ChainNode>();
            if (!entityGroups.IsEmpty())
            {
                groups.Add(new ChainNode(null, entityGroups[0]));
                for (int i = 1; i < entityGroups.Length; i++)
                {
                    groups.Add(new ChainNode(groups[i - 1], entityGroups[i]));
                }
            }

            return groups.ToArray();
        }
    }
}
