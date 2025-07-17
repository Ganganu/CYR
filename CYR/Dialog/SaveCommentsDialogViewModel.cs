using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CYR.Dialog;

public partial class SaveCommentsDialogViewModel : ObservableRecipient
{
    public SaveCommentsDialogViewModel()
    {
        CommentsType = ["Kommentaren Oben", "Kommentaren Unten"];
    }
    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private string? _icon;
    public ObservableCollection<string>? CommentsType;

}
