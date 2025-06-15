using app22.Classes;

namespace app22.Telas;

public partial class Cadastro : ContentPage
{
	public Cadastro()
	{
		InitializeComponent();
	}

    private async void BTNCadastro_Clicked(object sender, EventArgs e)
    {
        var pessoa = new DadosUsuario
        {
            usuario = nomeEntry.Text,
            telefone = telefoneEntry.Text,
            endereco = enderecoEntry.Text,
            senha = senhaEntry.Text,
        };

        try
        {
            var firebase = new GravaDadosUsuario();
            await firebase.GravarPessoaAsync(pessoa);
            await DisplayAlert("Sucesso", "Usuário cadastrado com sucesso!", "OK");
            App.Current.MainPage = new Login();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void BTNVoltar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new Login();
    }
}