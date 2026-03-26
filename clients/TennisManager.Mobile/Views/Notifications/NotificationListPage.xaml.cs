using TennisManager.Mobile.ViewModels.Notifications;

namespace TennisManager.Mobile.Views.Notifications;

public partial class NotificationListPage : ContentPage
{
    private readonly NotificationListViewModel _viewModel;

    public NotificationListPage(NotificationListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadNotificationsCommand.Execute(null);
    }
}
