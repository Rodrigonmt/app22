using app22.Classes;

namespace app22.Telas;

public partial class AlteraCadastro : ContentPage
{
    private readonly string _usuarioLogado = null;
    public AlteraCadastro(string usuarioLogado)
    {
        InitializeComponent();
        _usuarioLogado = usuarioLogado;
        CarregaPreencheUsuario();
        //verificacamo();
    }

    public async void CarregaPreencheUsuario()
    {
        if (string.IsNullOrWhiteSpace(_usuarioLogado))
        {
            await DisplayAlert("Erro", "Usuário logado não informado.", "OK");
            return;
        }

        try
        {
            var firebase = new FirebaseService();
            var usuario = await firebase.BuscarPessoaPorNomeAsync(_usuarioLogado);

            if (usuario == null)
            {
                await DisplayAlert("Aviso", "Usuário não encontrado no banco de dados.", "OK");
                return;
            }

            // Preenche os campos com os dados do Firebase
            nomeEntry.Text = usuario.usuario;
            telefoneEntry.Text = usuario.telefone;
            enderecoEntry.Text = usuario.endereco;
            senhaEntry.Text = usuario.senha;
            senhaConf.Text = usuario.senha;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao carregar os dados do usuário: {ex.Message}", "OK");
        }
    }

    //public async void verificacamo()
    //{
    //    await DisplayAlert("Mensag", $"->{_usuarioLogado}<-", "Ok");
    //}

    private async void BTNCadastro_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nomeEntry.Text) ||
        string.IsNullOrWhiteSpace(telefoneEntry.Text) ||
        string.IsNullOrWhiteSpace(enderecoEntry.Text) ||
        string.IsNullOrWhiteSpace(senhaEntry.Text) ||
        string.IsNullOrWhiteSpace(senhaConf.Text))
        {
            await DisplayAlert("Atenção", "Por favor, preencha todos os campos antes de continuar.", "OK");
            return;
        }

        if (senhaEntry.Text != senhaConf.Text)
        {
            await DisplayAlert("Erro", "As senhas digitadas não coincidem.", "OK");
            return;
        }

        var pessoaAtualizada = new DadosUsuario
        {
            usuario = nomeEntry.Text,
            telefone = telefoneEntry.Text,
            endereco = enderecoEntry.Text,
            senha = senhaEntry.Text
        };

        try
        {
            var firebase = new FirebaseService();
            await firebase.AtualizarPessoaAsync(_usuarioLogado, pessoaAtualizada);
            await DisplayAlert("Sucesso", "Cadastro atualizado com sucesso!", "OK");
            App.Current.MainPage = new NavegarMenus(nomeEntry.Text);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao atualizar o cadastro: {ex.Message}", "OK");
        }


    }

}