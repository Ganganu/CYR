using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Invoice;
using CYR.Services;

namespace CYR.Dialog;

public partial class ItemsListDialogViewModel(IXMLService xmlService) : ObservableRecipient
{

    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private ObservableCollection<FileModel>? _files;
    [ObservableProperty]
    private CommentType _commentType;
    [ObservableProperty]
    private string? _icon;

    [NotifyCanExecuteChangedFor(nameof(SendSelectedFileCommand))]
    [ObservableProperty]
    private FileModel? _selectedFile;
    [ObservableProperty]
    private string? _folderPath;

    [ObservableProperty]
    private string _xmlString;

    private bool CanSendFile()
    {
        return SelectedFile != null && SelectedFile.FileName.Length > 0;
    }

    [RelayCommand(CanExecute =nameof(CanSendFile))]
    private async Task SendSelectedFile()
    {
        if (SelectedFile == null)
        {
            return;
        }
        XmlString = await xmlService.LoadAsync(SelectedFile.FileName, FolderPath);


        Messenger.Send(this);
    }
    
}
