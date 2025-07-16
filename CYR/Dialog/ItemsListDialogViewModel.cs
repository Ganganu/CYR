using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Invoice;
using CYR.Services;

namespace CYR.Dialog;

public partial class ItemsListDialogViewModel : ObservableRecipient
{
    private readonly IXMLService _xmlService;
    public ItemsListDialogViewModel(IXMLService xmlService)
    {
        _xmlService = xmlService;
    }

    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private ObservableCollection<FileModel>? _files;

    [ObservableProperty]
    private string? _icon;

    [NotifyCanExecuteChangedFor(nameof(SendSelectedFileCommand))]
    [ObservableProperty]
    private FileModel? _selectedFile;

    [ObservableProperty]
    private string _xmlString;

    private bool CanSendFile()
    {
        return SelectedFile != null && SelectedFile.FileName.Length > 0;
    }

    [RelayCommand(CanExecute =nameof(CanSendFile))]
    private void SendSelectedFile()
    {
        if (SelectedFile == null)
        {
            return;
        }
        XmlString = _xmlService.LoadAsync(SelectedFile.FileName);


        Messenger.Send(this);
    }
    
}
