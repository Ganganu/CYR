using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CYR.OrderItems;

public partial class OrderItem :ObservableRecipient
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
    private string? _price;

    partial void OnIsSelectedChanged(bool value)
    {
        Messenger.Send(new OrderItemIsSelectedChangedMessage(this));
    }
}

record OrderItemIsSelectedChangedMessage(OrderItem orderItem);
