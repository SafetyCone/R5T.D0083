using System;


namespace R5T.D0083.I001
{
    /// <summary>
    /// Empty implementation as base for extension methods.
    /// </summary>
    public class VisualStudioProjectOperator : IVisualStudioProjectOperator
    {
        #region Static
        
        public static VisualStudioProjectOperator Instance { get; } = new();

        #endregion
    }
}