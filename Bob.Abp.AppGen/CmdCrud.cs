using Bob.Abp.AppGen.DteExtension;
using Bob.Abp.AppGen.Models;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace Bob.Abp.AppGen
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CmdCrud : CmdBase
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Initializes a new instance of the <see cref="CmdCrud"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        /// <param name="outputPane">Pane used to show prompt text.</param>
        private CmdCrud(AsyncPackage package, OleMenuCommandService commandService) : base(package)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CmdCrud Instance
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
            Instance = new CmdCrud(package, commandService);
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
            //get model from cs or json file.
            ProjectItem prjItem = _dte.SelectedItems.GetSelectedItem();
            var entityModel = prjItem.GetEntityModel();
            //safe mode?
            var message = $@"About to generate codes for ""{entityModel.Name}"", do you want to use safe mode?
In safe mode, we will backup files to be modified and skip files to be overwritten.
Choose [Yes] to use safe mode, [No] to OVERWRITE all files and your manually modified code will LOST!";
            var action = VsShellUtilities.ShowMessageBox(this.package, message, title, OLEMSGICON.OLEMSGICON_WARNING,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            if (action == Utils.OLEMS_CANCEL) return; //CANCEL
            var safeMode = action == Utils.OLEMS_YES;
            //generate codes
            _outputPane.Clear();
            OutputMessage($"-----Generating code for {entityModel.Name}------", lineEnd: true);
            var abpGenerator = new AbpCodeGenerator(entityModel, safeMode);
            //    localization
            if (!entityModel.SkipSettings.ToSkip(AbpMiscFile.Shared_Localization))
            {
                OutputMessage("Modify localization file...");
                abpGenerator.CreateOrUpdateLocalization();
            }
            // Main files
            foreach (AbpMainFile abpFile in Enum.GetValues(typeof(AbpMainFile)))
            {
                if (!entityModel.SkipSettings.ToSkip(abpFile))
                {
                    OutputMessage($"Generate {abpFile}...");
                    abpGenerator.CreateOrEditFiles(abpFile);
                }
            }

            // Show a message box to prove we were here
            OutputMessage($"====End of generating code for {entityModel.Name}====", lineEnd: true);
            VsShellUtilities.ShowMessageBox(this.package, "Code Generated!", title, OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
