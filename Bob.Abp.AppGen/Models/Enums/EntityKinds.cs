using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// Entity Kinds, for example Flat/Hierarcy, 
    /// </summary>
    [Flags]
    public enum EntityKinds
    {
        /// <summary>
        /// Not an entity.
        /// </summary>
        None = 0,

        /// <summary>
        /// Every entity has this flag.
        /// </summary>
        Entity = 1,

        /// <summary>
        /// Entity has Guid ParentId property
        /// </summary>
        Hierarcy = 2,

        /// <summary>
        /// This is an extensible entity (Descendant of AggregateRoot)
        /// </summary>
        Extensible = 4,

        /// <summary>
        /// Inherited from IMultiTenant.
        /// </summary>
        MultiTenant = 8
    }
}
