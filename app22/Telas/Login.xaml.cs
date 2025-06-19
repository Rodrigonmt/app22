using app22.Classes;

namespace app22.Telas;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
        StartMarquee();

    }

    private async void StartMarquee()
    {
        while (true)
        {
            double larguraTexto = teste1.Width;
            double larguraTela = this.Width;

            // Começa fora da tela à direita
            teste1.TranslationX = larguraTela;

            // Move para a esquerda até fora da tela
            await teste1.TranslateTo(-larguraTexto, 0, 8000, Easing.Linear);
        }
    }

    private async void BTN_Entrar_Login_Clicked(object sender, EventArgs e)
    {
    string nomeDigitado = TXTUsuario.Text;
    string senhaDigitado = TXTSenha.Text;

    try
    {
        var firebase = new FirebaseService();
        var pessoa = await firebase.BuscarPessoaPorNomeAsync(nomeDigitado);

        if (pessoa == null)
        {
            await DisplayAlert("Mensagem", "Usuário não encontrado", "OK");
            return;
        }

        if (pessoa.senha == senhaDigitado)
        {
            //await SecureStorage.Default.SetAsync("usuario_logado", TXTUsuario.Text);
            App.Current.MainPage = new NavegarMenus(TXTUsuario.Text);
        }
        else
        {
            await DisplayAlert("Mensagem", "Senha não está correta!", "OK");
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Erro", ex.Message, "OK");
    }

    }

    private async void BTN_ir_cadastro_Clicked(object sender, EventArgs e)
    {
        try
        {
            App.Current.MainPage = new Cadastro(TXTUsuario.Text);

        }
        catch (Exception ex)
        {

            await DisplayAlert("Erro", ex.Message, "Ok");
        }
    }
}