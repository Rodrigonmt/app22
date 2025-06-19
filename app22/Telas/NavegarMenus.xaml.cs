namespace app22.Telas;

public partial class NavegarMenus : ContentPage
{
    public string _usuarioLogado = null;//variavel aceita valor null com o ?
    public NavegarMenus(string usuariologado)
	{
        
        InitializeComponent();
        _usuarioLogado = usuariologado;
        CarregarUsuarioAsync();


    }

    private async void CarregarUsuarioAsync()
    {
        // Atualiza a saudação
        AtualizarMensagem();

        // Mostra o botão apenas para Luiz ou Rodrigo
        if (_usuarioLogado == "Luiz" || _usuarioLogado == "Rodrigo")
        {
            ChamadosAdmFrame.IsVisible = true;
        }
        else
        {
            ChamadosAdmFrame.IsVisible = false;
        }
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
        App.Current.MainPage = new AlteraCadastro(_usuarioLogado);
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


    private void OnChamadosAdm(object sender, EventArgs e)
    {
        App.Current.MainPage = new ChamadosAdm();
    }
}