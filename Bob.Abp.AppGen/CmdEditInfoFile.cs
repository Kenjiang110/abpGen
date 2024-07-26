using EnvDTE;
using Bob.Abp.AppGen.Dialogs;
using Bob.Abp.AppGen.DteExtension;
using Bob.Abp.AppGen.Models;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Bob.Abp.AppGen
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CmdEditInfoFile : CmdBase
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0102;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdEditInfoFile"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CmdEditInfoFile(AsyncPackage package, OleMenuCommandService commandService) : base(package)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CmdEditInfoFile Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package, EnvDTE80.DTE2 dte)
        {
            BaseInitialize(package, dte);
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CmdEditInfoFile(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //get model from json file, or from cs file if no json file yet.
            ProjectItem prjItem = _dte.SelectedItems.GetSelectedItem();
            var entityModel = prjItem.GetEntityModel(true);

            var editor = new ExtraInfoEditor(entityModel);
            if (editor.ShowModal() == false) //eidt ui
            {
                return;
            }
            entityModel = editor.EntityModel;
            entityModel.SaveEntityModel();
        }
    }
}
