using app22.Classes;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
//using Microsoft.Maui.Essentials;

namespace app22.Telas;

public partial class AnunciarProduto : ContentPage
{
    //private List<Image> _fotos = new();
    private readonly string _usuarioLogado;
    private List<FileResult> _fotos = new(); // Alterado para armazenar arquivos reais
    public AnunciarProduto(string usuarioLogado)
	{
		InitializeComponent();
        _usuarioLogado = usuarioLogado;
        AdicionarBotaoAdicionarFoto();
    }

    private void AdicionarBotaoAdicionarFoto()
    {
        var botao = new Button
        {
            Text = "Tirar Fotos",
            WidthRequest = 120,
            HeightRequest = 60,
            BackgroundColor = Colors.LightGray,
            CornerRadius = 10
        };
        botao.Clicked += async (s, e) => await TirarCincoFotos();

        FotoStack.Children.Add(botao);
    }
    private async Task TirarCincoFotos()
    {
        _fotos.Clear();
        FotoStack.Children.Clear();
        AdicionarBotaoAdicionarFoto(); // Reinsere o botão após limpar

        for (int i = 0; i < 5; i++)
        {
            try
            {
                var foto = await MediaPicker.CapturePhotoAsync();

                if (foto != null)
                {
                    _fotos.Add(foto);

                    var image = new Image
                    {
                        Source = ImageSource.FromFile(foto.FullPath),
                        WidthRequest = 60,
                        HeightRequest = 60,
                        Aspect = Aspect.AspectFill
                    };

                    FotoStack.Children.Insert(FotoStack.Children.Count - 1, image);
                }
                else
                {
                    // O usuário pode cancelar — nesse caso, interrompe o loop
                    break;
                }
            }
            catch (Exception)
            {
                // Se o usuário cancelar ou der erro, sai do loop
                break;
            }
        }

        if (_fotos.Count == 0)
        {
            await DisplayAlert("Atenção", "Nenhuma foto foi tirada.", "OK");
        }
    }
    private async void OnPublicarClicked(object sender, EventArgs e)
    {
        var fotosBase64 = new List<string>();

        foreach (var foto in _fotos)
        {
            if (foto != null)
            {
                using var stream = await foto.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                fotosBase64.Add(base64);
            }
        }

        var produto = new Produto
        {
            NomeProduto = NomeProdutoEntry.Text,
            NomeVendedor = NomeVendedorEntry.Text,
            Marca = MarcaEntry.Text,
            Descricao = DescricaoEditor.Text,
            Estado = EstadoPicker.SelectedItem?.ToString(),
            Valor = ValorEntry.Text,
            Telefone = TelefoneEntry.Text,
            Fotos = fotosBase64, // Agora base64
            DataPublicacao = DateTime.Now.ToString("yyyy-MM-dd")
        };

        try
        {
            var firebaseService = new FirebaseService();
            await firebaseService.EnviarProdutoAsync(produto);
            await DisplayAlert("Sucesso", "Produto anunciado com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao publicar: {ex.Message}", "OK");
        }
    }



}