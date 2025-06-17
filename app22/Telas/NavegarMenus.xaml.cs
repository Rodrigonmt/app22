namespace app22.Telas;

public partial class NavegarMenus : ContentPage
{
	public NavegarMenus()
	{
        
        InitializeComponent();
        AtualizarMensagem();


    }

    private async void AtualizarMensagem()
    {
        var mainPage = Application.Current.MainPage as MainPage;
        string _usuarioLog = mainPage?._usuarioLogado;
        BemVindoLabel.Text = $"Bem-vindo(a), {_usuarioLog}!";
    }

    private async void OnCriarChamadoClicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new MainPage();
    }

    private async void OnHistoricoClicked(object sender, EventArgs e)
    {
        DisplayAlert("Esperar", "Esperar", "Esperar");
        App.Current.MainPage = new Chamados();
    }

    private async void OnVenderClicked(object sender, EventArgs e)
    {
        
    }

    private async void OnComprarClicked(object sender, EventArgs e)
    {
        
    }

    private async void OnAlterarCadastroClicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new Cadastro();
    }

    private async void Desconnectar_Clicked(object sender, EventArgs e)
    {
        bool conf = await DisplayAlert("Tem certeza?", "Sair do app", "Sim", "Não");
        if (conf)
        {
            SecureStorage.Default.Remove("usuario_logado");
            App.Current.MainPage = new Login();
        }
    }
}