using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Invoice;
using System.Collections.ObjectModel;

namespace CYR.Dialog;
public enum CommentType
{
    Top,
    Bottom
}

public partial class SaveCommentsDialogViewModel : ObservableRecipient
{
    private readonly IXMLService _xMLService;
    public SaveCommentsDialogViewModel(IXMLService xMLService)
    {
        CommentsType = [CommentType.Top, CommentType.Bottom];
        _xMLService = xMLService;
    }
    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private string? _icon;
    [ObservableProperty]
    private string? _fileName;
    [ObservableProperty]
    private string? _textToSerialize;
    [ObservableProperty]
    public ObservableCollection<CommentType>? _commentsType;
    [ObservableProperty]
    private CommentType _selectedItem; 
    

    [RelayCommand]
    private void SaveTemplate()
    {
        if (TextToSerialize is null) return;
        if (FileName is null) return;
        if (SelectedItem == CommentType.Top)
        {
            _xMLService.SaveAsync(TextToSerialize,"Top",FileName);
        }
        else
        {
            _xMLService.SaveAsync(TextToSerialize, "Bottom", FileName);
        }
    }
}


