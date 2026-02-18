using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Messages;
using CYR.Services;
using CYR.ViewModel;
using Microsoft.Win32;
using System.Text;

namespace CYR.OrderItems.OrderItemCommand;

public class ImportOrderItemsCommand(IArticleImportService _articleImportService, 
    IOrderItemRepository _orderItemRepository) : ObservableRecipient
{
    public async Task Import()
    {
        OpenFileDialog fileDialog = new OpenFileDialog { Filter = "JSON | *.json" };
        if (fileDialog.ShowDialog() != true)
            return;
        int insertedRows = 0;
        var data = _articleImportService.Import("Json", fileDialog.FileName);
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
        foreach (var item in data)
        {
            insertedRows = await _orderItemRepository.InsertAsync(new OrderItem() { Name = item.Name, Description = item.Description, Price = item.Price });
        }

        Messenger.Send(new SnackbarMessage($"{data.Count} Zeilen wurden gelesen. \n {insertedRows} Zeilen wurden in die Datenbank eigefügt!", "Check"));
    }
}
