namespace WslToolbox.UI.Core.Helpers;

public class DialogResult<T>
{
    public T Dialog;
    public DialogResult Result;
}

public class DialogHelper
{
    public static DialogResult<FolderBrowserDialog> ShowSelectFolderDialog(string initialDirectory)
    {
        using var dialog = new FolderBrowserDialog
        {
            InitialDirectory = initialDirectory
        };

        return new DialogResult<FolderBrowserDialog>
        {
            Result = dialog.ShowDialog(),
            Dialog = dialog
        };
    }

    public static DialogResult<SaveFileDialog> ShowSaveFileDialog()
    {
        return ShowSaveFileDialog(dialog: new SaveFileDialog
        {
            Site = null,
            Tag = null,
            AddExtension = false,
            AddToRecent = false,
            CheckFileExists = false,
            CheckPathExists = false,
            ClientGuid = null,
            DefaultExt = null,
            DereferenceLinks = false,
            FileName = null,
            Filter = null,
            FilterIndex = 0,
            InitialDirectory = null,
            RestoreDirectory = false,
            ShowHelp = false,
            ShowHiddenFiles = false,
            SupportMultiDottedExtensions = false,
            Title = null,
            ValidateNames = false,
            AutoUpgradeEnabled = false,
            OkRequiresInteraction = false,
            ShowPinnedPlaces = false,
            CheckWriteAccess = false,
            CreatePrompt = false,
            ExpandedMode = false,
            OverwritePrompt = false
        });
    }

    public static DialogResult<SaveFileDialog> ShowSaveFileDialog(SaveFileDialog dialog)
    {
        var result = new DialogResult<SaveFileDialog>();
        result.Result = dialog.ShowDialog();
        result.Dialog = dialog;
        return result;
    }

    public static DialogResult<OpenFileDialog> ShowOpenFileDialog()
    {
        return ShowOpenFileDialog(dialog: new OpenFileDialog());
    }

    public static DialogResult<OpenFileDialog> ShowOpenFileDialog(OpenFileDialog dialog)
    {
        return new DialogResult<OpenFileDialog>
        {
            Result = dialog.ShowDialog(),
            Dialog = dialog
        };
    }

    public static string ExtensionFilter(Dictionary<string, string> extensions)
    {
        return string.Join(separator: "|", value: extensions.Select(selector: kv => $"{kv.Key}|*{kv.Value}").ToArray());
    }
}