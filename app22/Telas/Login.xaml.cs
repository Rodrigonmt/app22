namespace app22.Telas;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private async void BTN_Entrar_Login_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (TXTUsuario.Text == "ro" && TXTSenha.Text == "123")
            {
                await SecureStorage.Default.SetAsync("usuario_logado", "Rodrigo");
                App.Current.MainPage = new MainPage();
            }
            else
            {
                throw new Exception("Senha incorreta");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "Fechar");
        }

    }

    private async void BTN_ir_cadastro_Clicked(object sender, EventArgs e)
    {
        try
        {
            App.Current.MainPage = new Cadastro();

        }
        catch (Exception ex)
        {

            await DisplayAlert("Erro", ex.Message, "Ok");
        }
    }
}