using app22.Classes;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace app22.Telas;

public partial class ComprarProdutos : ContentPage
{
    private readonly FirebaseService _firebaseService = new();
    private List<Produto> _todosProdutos = new();
    public ComprarProdutos()
	{
		InitializeComponent();
        CarregarProdutos();
    }

    private View CriarCardProduto(Produto produto)
    {
        return new Frame
        {
            BorderColor = Colors.Gray,
            CornerRadius = 10,
            Padding = 10,
            HasShadow = true,
            Content = new StackLayout
            {
                Spacing = 5,
                Children =
            {
                new Label { Text = produto.NomeProduto, FontAttributes = FontAttributes.Bold, FontSize = 18 },
                new Label { Text = $"Marca: {produto.Marca}" },
                new Label { Text = $"Valor: R$ {produto.Valor}" },
                new Label { Text = $"Estado: {produto.Estado}" },
                new Label { Text = $"Vendedor: {produto.NomeVendedor}" },
                new Label { Text = $"Telefone: {produto.Telefone}" },
                new Label { Text = $"Publicado em: {produto.DataPublicacao}", FontSize = 12, TextColor = Colors.Gray },
                new Label { Text = produto.Descricao },
                CriarFotosLayout(produto.Fotos)
            }
            }
        };
    }

    private async void CarregarProdutos()
    {
        try
        {
            Loader.IsVisible = true;
            Loader.IsRunning = true;
            LoaderStack.IsVisible = true;
            ProdutosStack.IsVisible = false;

            _todosProdutos = await _firebaseService.ObterProdutosAsync();

            AplicarFiltros(); // Mostra todos os produtos após carregar
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao carregar produtos: {ex.Message}", "OK");
        }
    }

    private View CriarFotosLayout(List<string>? fotosBase64)
    {
        var layout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 10 };

        if (fotosBase64 == null || fotosBase64.Count == 0)
            return layout;

        for (int i = 0; i < fotosBase64.Count; i++)
        {
            var base64 = fotosBase64[i];

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                ImageSource imgSrc = ImageSource.FromStream(() => new MemoryStream(bytes));

                var img = new Image
                {
                    Source = imgSrc,
                    WidthRequest = 80,
                    HeightRequest = 80,
                    Aspect = Aspect.AspectFill
                };

                int index = i; // necessário para capturar corretamente o índice

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    await Navigation.PushModalAsync(new FotoTelaCheia(fotosBase64, index));
                };
                img.GestureRecognizers.Add(tapGesture);

                layout.Children.Add(img);
            }
            catch
            {
                // Ignora imagem inválida
            }
        }

        return layout;
    }


    private void AplicarFiltros_Clicked(object sender, EventArgs e)
    {
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        ProdutosStack.Children.Clear();

        var filtrados = _todosProdutos.Where(p =>
        {
            bool nomeOk = string.IsNullOrWhiteSpace(FiltroNomeProduto.Text) || p.NomeProduto?.ToLower().Contains(FiltroNomeProduto.Text.ToLower()) == true;
            bool vendedorOk = string.IsNullOrWhiteSpace(FiltroNomeVendedor.Text) || p.NomeVendedor?.ToLower().Contains(FiltroNomeVendedor.Text.ToLower()) == true;
            bool estadoOk = true;
            if (FiltroEstado.SelectedItem != null && FiltroEstado.SelectedItem.ToString() != "Todos")
            {
                estadoOk = p.Estado?.ToLower() == FiltroEstado.SelectedItem.ToString().ToLower();
            }
            bool valorMinOk = true;
            if (decimal.TryParse(FiltroValorMin.Text, out decimal min))
                valorMinOk = decimal.TryParse(p.Valor, out decimal val) && val >= min;

            bool valorMaxOk = true;
            if (decimal.TryParse(FiltroValorMax.Text, out decimal max))
                valorMaxOk = decimal.TryParse(p.Valor, out decimal val) && val <= max;

            bool dataOk = true;
            if (FiltroDataIni.Date != FiltroDataFim.Date)
            {
                if (DateTime.TryParse(p.DataPublicacao, out var dataPub))
                    dataOk = dataPub.Date >= FiltroDataIni.Date.Date && dataPub.Date <= FiltroDataFim.Date.Date;
            }

            return nomeOk && vendedorOk && estadoOk && valorMinOk && valorMaxOk && dataOk;
        }).ToList();

        foreach (var produto in filtrados)
        {
            var card = CriarCardProduto(produto);
            ProdutosStack.Children.Add(card);
        }

        Loader.IsRunning = false;
        Loader.IsVisible = false;
        LoaderStack.IsVisible = false;
        ProdutosStack.IsVisible = true;
    }

}