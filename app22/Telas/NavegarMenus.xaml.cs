namespace app22.Telas;

public partial class NavegarMenus : ContentPage
{
    public string _usuarioLogado = null;//variavel aceita valor null com o ?
    public NavegarMenus()
	{
        
        InitializeComponent();

        CarregarUsuarioAsync();


    }

    private async void CarregarUsuarioAsync()
    {
        _usuarioLogado = await SecureStorage.Default.GetAsync("usuario_logado");

        // Agora sim: após o valor ser carregado, atualiza a mensagem
        AtualizarMensagem();
    }

    private void AtualizarMensagem()
    {
        BemVindoLabel.Text = $"Bem-vindo(a), {_usuarioLogado}!";
    }

    private async void OnCriarChamadoClicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new MainPage(_usuarioLogado);
    }

    private async void OnHistoricoClicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new Chamados(_usuarioLogado);
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