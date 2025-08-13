﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Dialog;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Logging;
using CYR.Services;
using CYR.User;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.Invoice.InvoiceViewModels;

public partial class InvoiceListViewModel : ObservableRecipient
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IDialogService _dialogService;
    private readonly LoggingRepository _loggingRepository;
    private readonly UserContext _userContext;

    private string? _dialogResponse;

    public InvoiceListViewModel(IInvoiceRepository invoiceRepository,
        INavigationService navigationService,
        IDialogService dialogService,
        LoggingRepository loggingRepository,
        UserContext userContext)
    {
        _invoiceRepository = invoiceRepository;
        NavigationService = navigationService;
        Initialize();
        _dialogService = dialogService;
        _loggingRepository = loggingRepository;
        _userContext = userContext;
    }
    private async void Initialize()
    {
        IEnumerable<InvoiceModel> invoices = await _invoiceRepository.GetAllAsync();
        Invoices = [.. invoices];
    }

    [ObservableProperty]
    private ObservableCollection<InvoiceModel>? _invoices;

    public INavigationService NavigationService { get; }

    [ObservableProperty]
    private bool _isSelectAllInvoicesChecked;

    [RelayCommand]
    private void NavigateBack()
    {
        Messenger.Send(new NavigateBackSource(typeof(InvoiceListViewModel)));
    }

    [RelayCommand]
    private void SelectInvoice(object parameter)
    {
        InvoiceModel invoice = (InvoiceModel)parameter;
        NavigationService.NavigateTo<ShowInvoiceViewModel>(invoice.InvoiceNumber);
    }
    [RelayCommand]
    private void SetInvoiceStateClosed()
    {
        foreach (var item in Invoices)
        {
            if (item.State == InvoiceState.Open && (bool)item.IsSelected)
            {                    
                item.State = InvoiceState.Closed;
                _invoiceRepository.UpdateAsync(item);
            }
        }
    }
    [RelayCommand]
    private void SetInvoiceStateOpen()
    {
        foreach (var item in Invoices)
        {
            if (item.State == InvoiceState.Closed && (bool)item.IsSelected)
            {
                item.State = InvoiceState.Open;
                _invoiceRepository.UpdateAsync(item);
            }
        }
    }

    [RelayCommand]
    private void CreateNewInvoice()
    {
        NavigationService.NavigateTo<CreateInvoiceViewModel>();
    }

    [RelayCommand]
    private async Task DeleteInvoice()
    {
        if (Invoices is null || !Invoices.Any()) return;

        var selectedInvoices = Invoices?.Where(i => i.IsSelected == true).ToList();
        if (selectedInvoices is null) return;
        if (selectedInvoices.Count < 1) return;
        try
        {
            foreach (var invoice in selectedInvoices)
            {
                ShowNotificationDialog("Kunde löschen", $"Möchten Sie wirklich die Rechnung {invoice.InvoiceNumber} löschen? Das würde bedeuten, dass alle Rechnungen und Daten im Zusammenhang mit dieser Rechnung auch gelsöcht werden!",
                    "Nein", "Invoice", Visibility.Visible, "Ja");
                if (_dialogResponse != "True")
                {
                    return;
                }
                bool result = await _invoiceRepository.DeleteAsync(invoice);
                Invoices?.Remove(invoice);
                await _loggingRepository.InsertAsync(CreateHisModel(invoice));
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private HisModel CreateHisModel(InvoiceModel invoice)
    {
        HisModel hisModel = new();
        hisModel.LoggingType = LoggingType.InvoiceDeleted;
        hisModel.InvoiceId = invoice.InvoiceNumber;
        hisModel.UserId = _userContext.CurrentUser.Id;
        hisModel.Message = $@"Invoice: {invoice.InvoiceNumber} wurde gelöscht vom User: {_userContext.CurrentUser.Id}";
        return hisModel;
    }

    [RelayCommand]
    private void UpdateInvoice()
    {
        if (Invoices is null) return;
        InvoiceModel? invoiceToUpdate = Invoices.Where(i => i.IsSelected == true).FirstOrDefault();
        if (invoiceToUpdate is null) return;
        NavigationService.NavigateTo<ShowInvoiceViewModel>(invoiceToUpdate.InvoiceNumber);
    }

    private void ShowNotificationDialog(string title,
            string message,
            string cancelButtonText,
            string icon,
            Visibility okButtonVisibility, string okButtonText)
    {
        _dialogService.ShowDialog(result =>
        {
            _dialogResponse = result;
        },
        new Dictionary<Expression<Func<NotificationViewModel, object>>, object>
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