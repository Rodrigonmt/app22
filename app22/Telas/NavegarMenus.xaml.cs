using app22.Classes;

namespace app22.Telas;

public partial class NavegarMenus : ContentPage
{
    public string _usuarioLogado = null;//variavel aceita valor null com o ?
    public NavegarMenus(string usuariologado)
	{
        
        InitializeComponent();
        _usuarioLogado = usuariologado;
        CarregarUsuarioAsync(); // já chama AtualizarMensagem internamente

    }
    private async void CarregarUsuarioAsync()
    {
        AtualizarMensagem();

        var firebase = new FirebaseService();
        var usuario = await firebase.BuscarPessoaPorNomeAsync(_usuarioLogado);

        if (usuario?.fotoBase64 != null)
        {
            var imageBytes = Convert.FromBase64String(usuario.fotoBase64);
            ImagemUsuario.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }

        ChamadosAdmFrame.IsVisible = (_usuarioLogado == "Luiz" || _usuarioLogado == "Rodrigo");
    }

    private void AtualizarMensagem()
    {
        BemVindoLabel.Text = $"Bem-vindo(a), {_usuarioLogado}!";
    }

    private async void OnCriarChamadoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage(_usuarioLogado));
    }

    private async void OnHistoricoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Chamados(_usuarioLogado));
    }

    private async void OnVenderClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AnunciarProduto(_usuarioLogado));
    }

    private async void OnComprarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ComprarProdutos());
    }

    private async void OnAlterarCadastroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AlteraCadastro(_usuarioLogado));
    }

    private async void OnChamadosAdm(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChamadosAdm(_usuarioLogado));
    }
}