namespace app22.Telas;

[QueryProperty(nameof(UsuarioLogado), "usuarioLogado")]
public partial class NavegarMenus : ContentPage
{
    public string UsuarioLogado { get; set; }//variavel aceita valor null com o ?
    public NavegarMenus()
	{
        
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarMensagem();

        ChamadosAdmFrame.IsVisible = UsuarioLogado == "Luiz" || UsuarioLogado == "Rodrigo";
    }

   

    private void AtualizarMensagem()
    {
        BemVindoLabel.Text = $"Bem-vindo(a), {UsuarioLogado}!";
    }

    private async void OnCriarChamadoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(MainPage)}?usuarioLogado={UsuarioLogado}");
    }

    private async void OnHistoricoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(Chamados)}?usuarioLogado={UsuarioLogado}");
    }

    private async void OnVenderClicked(object sender, EventArgs e)
    {
        
    }

    private async void OnComprarClicked(object sender, EventArgs e)
    {
        
    }

    private async void OnAlterarCadastroClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AlteraCadastro)}?usuarioLogado={UsuarioLogado}");
    }

    private async void Desconnectar_Clicked(object sender, EventArgs e)
    {
        bool conf = await DisplayAlert("Tem certeza?", "Sair do app", "Sim", "Não");
        if (conf)
        {
            SecureStorage.Default.Remove("usuario_logado");
            await Shell.Current.GoToAsync("//Login"); // volta para o início
        }
    }


    private async void OnChamadosAdm(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ChamadosAdm)}?usuarioLogado={UsuarioLogado}");
    }
}