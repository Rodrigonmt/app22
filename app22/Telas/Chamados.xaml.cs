using System.Text.Json;
using Microsoft.Maui.Controls;
using app22.Classes;
using System.Net.Http;
using System.Collections.ObjectModel;
using System;

namespace app22.Telas;

public partial class Chamados : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _usuarioLogado;

    public Chamados(string usuarioLogado)
	{

        InitializeComponent();
        _usuarioLogado = usuarioLogado;
        CarregarChamadosAsync();
        
    }

    private async Task CarregarChamadosAsync()
    {

        try
        {
            string url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos/{_usuarioLogado}.json";
            
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

    private async void BTNVoltar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavegarMenus();
    }
}