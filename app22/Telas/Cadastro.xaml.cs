using app22.Classes;

namespace app22.Telas;

public partial class Cadastro : ContentPage
{
    //private readonly string _usuarioLogado = nomeEntry.Text;
    private FileResult _fotoUsuario;
    private bool _atualizandoTelefone = false;

    public Cadastro()
    {
        InitializeComponent();
        //verificacamo();
    }

    //public async void verificacamo()
    //{
    //    await DisplayAlert("Mensag", $"->{_usuarioLogado}<-", "Ok");
    //}

    private async void TelefoneEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_atualizandoTelefone) return;

        var entry = sender as Entry;
        if (entry == null) return;

        try
        {
            _atualizandoTelefone = true;

            // Aguarda um pequeno delay para sincronizar com o input real
            await Task.Delay(50);

            // Extrai apenas n�meros
            string numeros = new string(entry.Text?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

            if (numeros.Length > 11)
                numeros = numeros.Substring(0, 11);

            string formatado = numeros;

            if (numeros.Length <= 2)
                formatado = $"({numeros}";
            else if (numeros.Length <= 6)
                formatado = $"({numeros.Substring(0, 2)}) {numeros.Substring(2)}";
            else if (numeros.Length <= 10)
                formatado = $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 4)}-{numeros.Substring(6)}";
            else
                formatado = $"({numeros.Substring(0, 2)}) {numeros.Substring(2, 5)}-{numeros.Substring(7)}";

            // Se mudou, atualiza o texto e reposiciona o cursor no final
            if (entry.Text != formatado)
            {
                entry.TextChanged -= TelefoneEntry_TextChanged;
                entry.Text = formatado;

                // Coloca o cursor no final do texto formatado
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        entry.CursorPosition = formatado.Length;
                    }
                    catch
                    {
                        // Ignora qualquer erro
                    }
                });

                entry.TextChanged += TelefoneEntry_TextChanged;
            }
        }
        finally
        {
            _atualizandoTelefone = false;
        }
    }

    private async void BtnFotoPerfil_Clicked(object sender, EventArgs e)
    {
        try
        {
            _fotoUsuario = await MediaPicker.Default.CapturePhotoAsync();
            if (_fotoUsuario != null)
                await DisplayAlert("Foto Capturada", "Foto de perfil registrada com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao tirar foto: {ex.Message}", "OK");
        }
    }

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

        // Verifica se o usu�rio j� est� cadastrado
        var usuarioExistente = await firebase.BuscarPessoaPorNomeAsync(nomeEntry.Text);
        if (usuarioExistente != null)
        {
            await DisplayAlert("Erro", "Usu�rio j� cadastrado. Por favor, utilize outro nome de usu�rio.", "OK");
            return;
        }

        string? fotoBase64 = null;
        if (_fotoUsuario != null)
        {
            using var stream = await _fotoUsuario.OpenReadAsync();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            fotoBase64 = Convert.ToBase64String(memory.ToArray());
        }

        var pessoa = new DadosUsuario
        {
            usuario = nomeEntry.Text,
            telefone = telefoneEntry.Text,
            endereco = enderecoEntry.Text,
            senha = senhaEntry.Text,
            fotoBase64 = fotoBase64
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
