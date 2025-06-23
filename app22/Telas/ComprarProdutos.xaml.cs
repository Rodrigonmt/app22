using app22.Classes;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace app22.Telas;

public partial class ComprarProdutos : ContentPage
{
    private readonly FirebaseService _firebaseService = new();
    public ComprarProdutos()
	{
		InitializeComponent();
        CarregarProdutos();
    }
    private async void CarregarProdutos()
    {
        try
        {
            var produtos = await _firebaseService.ObterProdutosAsync();

            foreach (var produto in produtos)
            {
                var card = new Frame
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

                            // Container de fotos
                            CriarFotosLayout(produto.Fotos)
                        }
                    }
                };

                ProdutosStack.Children.Add(card);
            }
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

        foreach (var base64 in fotosBase64)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                ImageSource imgSrc = ImageSource.FromStream(() => new MemoryStream(bytes));

                layout.Children.Add(new Image
                {
                    Source = imgSrc,
                    WidthRequest = 80,
                    HeightRequest = 80,
                    Aspect = Aspect.AspectFill
                });
            }
            catch
            {
                // Ignora imagens inválidas
            }
        }

        return layout;
    }

}