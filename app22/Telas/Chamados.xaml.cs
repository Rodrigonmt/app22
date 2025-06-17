using System.Text.Json;
using Microsoft.Maui.Controls;
using app22.Classes;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace app22.Telas;

public partial class Chamados : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    public Chamados()
	{

        InitializeComponent();
        CarregarChamadosAsync();
    }

    private async Task CarregarChamadosAsync()
    {
        try
        {
            string url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos/Amanda.json";
            var response = await _httpClient.GetStringAsync(url);

            var jsonDoc = JsonDocument.Parse(response);
            var chamados = new ObservableCollection<Chamado>();

            foreach (var item in jsonDoc.RootElement.EnumerateObject())
            {
                var chamado = JsonSerializer.Deserialize<Chamado>(item.Value.ToString());
                chamado.Id = item.Name; // salva o ID (chave do Firebase)
                chamados.Add(chamado);
            }

            ChamadosCollectionView.ItemsSource = chamados;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}