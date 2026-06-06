namespace EnterprisePOS.Features.Users.Views;

public partial class CreateUserPage : ContentPage
{
    public CreateUserPage()
    {
        try
        {
            InitializeComponent();
            BindingContext = new ViewModels.CreateUserViewModel();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CreateUserPage init error: {ex}");
            throw;
        }
    }
}
