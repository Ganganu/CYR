using System.Windows;
using System.Windows.Input;

namespace CYR.Login;

/// <summary>
/// Interaktionslogik für LoginView.xaml
/// </summary>
public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void btnExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void bTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        txtusername.Focus();
    }
}
