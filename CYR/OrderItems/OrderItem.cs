using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.OrderItems
{
    public partial class OrderItem :ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string? _description;
        [ObservableProperty]
        private string? _name;
        [ObservableProperty]
        private decimal? _price;
    }
}
