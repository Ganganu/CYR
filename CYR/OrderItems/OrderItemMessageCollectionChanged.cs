using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CYR.OrderItems
{
    public class OrderItemMessageCollectionChanged : ValueChangedMessage<bool>
    {
        public OrderItemMessageCollectionChanged(bool value) :base(value)
        {
            
        }        
    }
}
