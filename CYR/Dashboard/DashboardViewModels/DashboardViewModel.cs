using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly StatisticOverviewViewModel _statisticOverviewViewModel;

    public DashboardViewModel(StatisticOverviewViewModel statisticOverviewViewModel)
    {
        _statisticOverviewViewModel = statisticOverviewViewModel;
    }

    public StatisticOverviewViewModel StatisticOverviewVM => _statisticOverviewViewModel;

}
