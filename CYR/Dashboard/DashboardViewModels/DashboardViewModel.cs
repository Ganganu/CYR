using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly StatisticOverviewViewModel _statisticOverviewViewModel;
    private readonly StatisticChartViewModel _statisticChartViewModel;
    private readonly DashboardUserViewModel _dashboardUserViewModel;
    private readonly DashboardInvoiceViewModel _dashboardInvoiceViewModel;

    public DashboardViewModel(StatisticOverviewViewModel statisticOverviewViewModel, StatisticChartViewModel statisticChartViewModel, DashboardUserViewModel dashboardUserViewModel, DashboardInvoiceViewModel dashboardInvoiceViewModel)
    {
        _statisticOverviewViewModel = statisticOverviewViewModel;
        _statisticChartViewModel = statisticChartViewModel;
        _dashboardUserViewModel = dashboardUserViewModel;
        _dashboardInvoiceViewModel = dashboardInvoiceViewModel;
    }

    public StatisticOverviewViewModel StatisticOverviewVM => _statisticOverviewViewModel;
    public StatisticChartViewModel StatisticChartVM => _statisticChartViewModel;
    public DashboardUserViewModel DashboardUserVM => _dashboardUserViewModel;
    public DashboardInvoiceViewModel DashboardInvoiceVM => _dashboardInvoiceViewModel;

}
