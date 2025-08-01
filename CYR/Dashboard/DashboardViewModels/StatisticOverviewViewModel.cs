using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dashboard.DashboardViewModels;

public partial class StatisticOverviewViewModel : ObservableRecipient
{
    private readonly StatisticOverviewRepository _repository;
    public StatisticOverviewViewModel(StatisticOverviewRepository repository)
    {
        _repository = repository;
        Initialize();
    }

    private async Task Initialize()
    {
        PaidInvoices = await _repository.GetNumberOfPaidInvoices();
        UnpaidInvoices = await _repository.GetNumberOfUnpaidInvoices();
        SalesYear = await _repository.GetSales("2025");
    }

    [ObservableProperty]
    private int? _paidInvoices;
    [ObservableProperty]
    private int? _unpaidInvoices;
    [ObservableProperty]
    private decimal? _salesYear;
}
