using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CYR.User;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardActivityViewModel : ObservableRecipient
{
    private readonly DashboardActivityRepository _dashboardActivityRepository;
    private readonly UserContext _userContext;

    [ObservableProperty]
    private ObservableCollection<ActivityModel> _activityModels = [];

    public DashboardActivityViewModel(DashboardActivityRepository dashboardActivityRepository, UserContext userContext)
    {
        _dashboardActivityRepository = dashboardActivityRepository;
        _userContext = userContext;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        if (!int.TryParse(_userContext.CurrentUser?.Id?.ToString(), out int id))
        {
            ActivityModels = [];
            return;
        }

        try
        {
            var activities = await _dashboardActivityRepository.GetUnifiedActivityFeedAsync(id, 5);
            ActivityModels = new ObservableCollection<ActivityModel>(activities);
        }
        catch (Exception)
        {
            ActivityModels = [];
        }
    }

    public async Task RefreshActivitiesAsync()
    {
        await InitializeAsync();
    }
}