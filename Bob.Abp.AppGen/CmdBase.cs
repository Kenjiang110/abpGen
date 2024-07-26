using EnvDTE;
using Bob.Abp.AppGen.DteExtension;
using Bob.Abp.AppGen.Models;
using Bob.Abp.AppGen.Templates;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace Bob.Abp.AppGen
{
    /// <summary>
    /// Command handler base class
    /// </summary>
    internal abstract class CmdBase
    {
        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("7b60269c-eb3e-4599-a9ce-713ea716016e");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        protected readonly AsyncPackage package = null;

        protected static OutputWindowPane _outputPane = null;

        public static EnvDTE80.DTE2 _dte = null;

        protected CmdBase(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        protected static void OutputMessage(string message, string source = null, bool lineEnd = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _outputPane?.OutputString($"{source}{message}{(lineEnd ? Environment.NewLine : String.Empty)}");
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void BaseInitialize(AsyncPackage package, EnvDTE80.DTE2 dte)
        {
            if (_dte == null) _dte = dte;
            if (_outputPane == null) _outputPane = _dte.GetOrCreateOutputPane("Bob.Abp.AppGen");
        }
    }
}
