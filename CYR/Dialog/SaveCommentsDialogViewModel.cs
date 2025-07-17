using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Invoice;
using System.Collections.ObjectModel;

namespace CYR.Dialog;

public partial class SaveCommentsDialogViewModel : ObservableRecipient
{
    private readonly IXMLService _xMLService;
    public SaveCommentsDialogViewModel(IXMLService xMLService)
    {
        CommentsType = ["Kommentaren Oben", "Kommentaren Unten"];
        _xMLService = xMLService;
    }
    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private string? _icon;
    [ObservableProperty]
    private string? _textToSerialize;
    [ObservableProperty]
    public ObservableCollection<string>? _commentsType;
    [ObservableProperty]
    private string? _selectedItem; 
    

    [RelayCommand]
    private void SaveTemplate()
    {
        if (SelectedItem is null) return;
        if (TextToSerialize is null) return;
        if (SelectedItem.Contains("oben", StringComparison.CurrentCultureIgnoreCase))
        {
            _xMLService.SaveAsync(TextToSerialize);
        }
        else
        {
            
        }
    }




    private enum CommentType
    {
        Top,
        Bottom
    }
}


