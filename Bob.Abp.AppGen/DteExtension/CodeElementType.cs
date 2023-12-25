namespace Bob.Abp.AppGen.DteExtension
{
    /// <summary>
    /// Code element's type according to position in file code model.
    /// </summary>
    public enum CodeElementType
    {
        /// <summary>
        /// Directly in file code model.
        /// </summary>
        Root,

        /// <summary>
        /// Interface or Class directly in namespace.
        /// </summary>
        Main,

        /// <summary>
        /// Child of main element.
        /// </summary>
        Sub
    }
}
