using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// Where to insert codes according to anchor element?
    /// </summary>
    [Flags]
    public enum Positions
    {
        /// <summary>
        /// Unkown positoin
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// At beginning of the anchor element.
        /// </summary>
        Start = 1,

        /// <summary>
        /// At end of the anchor element.
        /// </summary>
        End = 2,

        /// <summary>
        /// Before the start or after the end.
        /// </summary>
        BeforeOrAfter = 4,

        /// <summary>
        /// Move one line in the direction.
        /// </summary>
        ExtraMove = 8,

        /// <summary>
        /// Before Start
        /// </summary>
        Before = Start | BeforeOrAfter,

        /// <summary>
        /// After End
        /// </summary>
        After = End | BeforeOrAfter
    }
}
