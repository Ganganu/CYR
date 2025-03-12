using CYR.Dialog;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.Invoice.InvoiceServices
{
    public class OpenImageService : IOpenImageService
    {
        private readonly IDialogService _dialogService;
        private string? _dialogResponse;

        public OpenImageService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }
        public void OpenImage(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    ShowErrorDialog("Fehler", ex.ToString(), "Abbrechen", "Error", Visibility.Collapsed, "");
                }
            }
        }
        private void ShowErrorDialog(string title,
            string message,
            string cancelButtonText,
            string icon,
            Visibility okButtonVisibility, string okButtonText)
        {
            _dialogService.ShowDialog(result =>
            {
                _dialogResponse = result;
            },
            new Dictionary<Expression<Func<ErrorDialogViewModel, object>>, object>
            {
                { vm => vm.Title, title },
                { vm => vm.Message,  message},
                { vm => vm.CancelButtonText, cancelButtonText },
                { vm => vm.Icon,icon },
                { vm => vm.OkButtonText, okButtonText },
                { vm => vm.IsOkVisible, okButtonVisibility}
            });
        }
    }
}
