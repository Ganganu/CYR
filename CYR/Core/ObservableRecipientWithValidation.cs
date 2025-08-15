using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CYR.Core;

public abstract class ObservableRecipientWithValidation : ObservableValidator
{
    protected ObservableRecipientWithValidation() : this(WeakReferenceMessenger.Default)
    {
    }

    protected ObservableRecipientWithValidation(IMessenger messenger)
    {
        Messenger = messenger;
    }
    protected IMessenger Messenger { get; }
}
