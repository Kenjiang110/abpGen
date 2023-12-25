﻿using EnvDTE;
using Bob.Abp.AppGen.Dialogs;
using Bob.Abp.AppGen.DteExtension;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Bob.Abp.AppGen
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CmdExtractInfoFile : CmdBase
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0101;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdExtractInfoFile"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CmdExtractInfoFile(AsyncPackage package, OleMenuCommandService commandService) : base(package)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CmdExtractInfoFile Instance
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
            await BaseInitializeAsync(package, dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CmdExtractInfoFile(package, commandService);
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

            string title = "BOB ABP Assistant";

            ProjectItem prjItem = _dte.SelectedItems.GetSelectedItem();
            var entityModel = prjItem.ToEntityModel();

            int action = Utils.OLEMS_YES;
            if (entityModel.SavedEntity != null)
            {
                var message = "Entity information file already exists, do you want to merge it?";
                action = VsShellUtilities.ShowMessageBox(this.package, message, title, OLEMSGICON.OLEMSGICON_QUERY,
                    OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                if (action == Utils.OLEMS_CANCEL) return;
                if (action == Utils.OLEMS_YES)
                {
                    entityModel.MergeSavedEntity();
                }

                message = "Do you want to backup it? \nChoose [Yes] to backup it, [No] to overwrite it.";
                action = VsShellUtilities.ShowMessageBox(this.package, message, title, OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                if (action == Utils.OLEMS_CANCEL) return;
            }
            else  //provide modifying chance for new info file.
            {
                ExtraInfoEditor editor = new ExtraInfoEditor(entityModel);
                if (editor.ShowModal() == true)
                {
                    entityModel = editor.EntityModel;
                }
            }

            entityModel.SaveEntityModel(action == Utils.OLEMS_YES);

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(this.package, "Entity extra information file created.", title,
                OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
