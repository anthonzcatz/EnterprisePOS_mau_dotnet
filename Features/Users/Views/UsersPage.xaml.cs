using EnterprisePOS.Features.Users.ViewModels;

namespace EnterprisePOS.Features.Users.Views;

public partial class UsersPage : ContentPage
{
	public UsersPage(UsersViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
