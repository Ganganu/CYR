using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.OrderItem
{
    public partial class OrderItem :ObservableObject
    {
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
