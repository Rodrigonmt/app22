using app22.Classes;

namespace app22.Telas;

public partial class Cadastro : ContentPage
{
    //private readonly string _usuarioLogado = nomeEntry.Text;
    public Cadastro()
	{
		InitializeComponent();
        //verificacamo();
    }

    //public async void verificacamo()
    //{
    //    await DisplayAlert("Mensag", $"->{_usuarioLogado}<-", "Ok");
    //}

    private async void BTNCadastro_Clicked(object sender, EventArgs e)
    {
        // Verifica se algum campo est� vazio
        if (string.IsNullOrWhiteSpace(nomeEntry.Text) ||
        string.IsNullOrWhiteSpace(telefoneEntry.Text) ||
        string.IsNullOrWhiteSpace(enderecoEntry.Text) ||
        string.IsNullOrWhiteSpace(senhaEntry.Text) ||
        string.IsNullOrWhiteSpace(senhaConf.Text))
        {
            await DisplayAlert("Aten��o", "Por favor, preencha todos os campos antes de continuar.", "OK");
            return;
        }

        if (senhaEntry.Text != senhaConf.Text)
        {
            await DisplayAlert("Erro", "As senhas digitadas n�o coincidem. Verifique e tente novamente.", "OK");
            return;
        }

        var firebase = new FirebaseService();

        // ?? Verifica se o usu�rio j� est� cadastrado
        var usuarioExistente = await firebase.BuscarPessoaPorNomeAsync(nomeEntry.Text);
        if (usuarioExistente != null)
        {
            await DisplayAlert("Erro", "Usu�rio j� cadastrado. Por favor, utilize outro nome de usu�rio.", "OK");
            return;
        }

        var pessoa = new DadosUsuario
        {
            usuario = nomeEntry.Text,
            telefone = telefoneEntry.Text,
            endereco = enderecoEntry.Text,
            senha = senhaEntry.Text,
        };

        try
        {
            await firebase.GravarPessoaAsync(pessoa);
            await DisplayAlert("Sucesso", "Usu�rio cadastrado com sucesso!", "OK");
            //await SecureStorage.Default.SetAsync("usuario_logado", nomeEntry.Text);
            await Navigation.PushAsync(new NavegarMenus(nomeEntry.Text));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }


    }

}