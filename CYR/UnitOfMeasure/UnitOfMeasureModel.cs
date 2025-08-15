using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Core;

namespace CYR.UnitOfMeasure;

public partial class UnitOfMeasureModel : ObservableRecipientWithValidation
{
    [ObservableProperty]
    private bool _isSelected;
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
