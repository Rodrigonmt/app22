namespace app22.Telas;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private async void BTN_Entrar_Login_Clicked(object sender, EventArgs e)
    {
        if (TXTUsuario.Text == "Ro" && TXTSenha.Text == "123")
        {
            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            await DisplayAlert("Erro de Login", "Usu�rio ou senha incorretos!", "OK");
        }
    }

    private async void BTN_ir_cadastro_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Cadastro());
    }
}