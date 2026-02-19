using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Messages;
using CYR.Services;
using System.Text;

namespace CYR.OrderItems.OrderItemCommand;

public class ImportOrderItemsCommand(IArticleImportService _articleImportService, 
    IOrderItemRepository _orderItemRepository) : ObservableRecipient
{
    public async Task Import(string method, string fileName)
    {
        int insertedRows = 0;
        var data = _articleImportService.Import(method, fileName);
        if (data.Count == 0 || data is null || data.Any(x => x.Name is null))
        {
            var errors = data?.Select(item => item.ErrorText).ToList();
            StringBuilder errorStringBuilder = new();
            if (errors is null || errors.Count == 0) return;
            foreach (var item in errors)
            {
                errorStringBuilder.Append(item);
            }
            Messenger.Send(new SnackbarMessage(errorStringBuilder.ToString(), "Warning"));
            return;
        }
        var orderItems = data.Select(d => d.ToOrderItem()).ToList();
        bool succes = await _orderItemRepository.InsertBulk(orderItems);

        if (succes)
            Messenger.Send(new SnackbarMessage("Import erfolgreich durchgeführt.", "Check"));

    }
}
