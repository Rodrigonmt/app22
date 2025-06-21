using System.Text.Json;
using Microsoft.Maui.Controls;
using app22.Classes;
using System.Net.Http;
using System.Collections.ObjectModel;
using System;
using System.Text;

namespace app22.Telas;

public partial class Chamados : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _usuarioLogado;
    private ObservableCollection<Chamado> _todosChamados = new ObservableCollection<Chamado>();

    public Chamados(string usuarioLogado)
	{

        InitializeComponent();
        _usuarioLogado = usuarioLogado;
        CarregarChamadosAsync();
    }

    private async void ChamadosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.CurrentSelection.FirstOrDefault() is Chamado chamadoSelecionado)
            {
                await Navigation.PushAsync(new ChamadoDetalhes(chamadoSelecionado.Id));
                ChamadosCollectionView.SelectedItem = null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void CancelarChamado_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Chamado chamado)
        {
            bool confirmar = await DisplayAlert("Confirmar", $"Deseja cancelar o chamado do equipamento '{chamado.Equipamento}'?", "Sim", "Não");

            if (!confirmar)
                return;

            try
            {
                string url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos/{_usuarioLogado}/{chamado.Id}/Status.json";
                var content = new StringContent("\"Cancelado\"", Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    chamado.Status = "Cancelado";

                    // Atualizar CollectionView
                    ChamadosCollectionView.ItemsSource = null;
                    ChamadosCollectionView.ItemsSource = _todosChamados;

                    await DisplayAlert("Sucesso", "Chamado cancelado com sucesso!", "OK");
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível cancelar o chamado.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao cancelar: {ex.Message}", "OK");
            }
        }
    }


    private async Task CarregarChamadosAsync()
    {
        _todosChamados.Clear();
        try
        {
            string url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos/{_usuarioLogado}.json";
            
            var response = await _httpClient.GetStringAsync(url);

            if (string.IsNullOrWhiteSpace(response) || response == "null")
            {
                await DisplayAlert("Aviso", "Usuário não possui chamados abertos", "OK");
                return;
            }

            var jsonDoc = JsonDocument.Parse(response);
            var chamados = new ObservableCollection<Chamado>();

            foreach (var item in jsonDoc.RootElement.EnumerateObject())
            {
                var chamado = JsonSerializer.Deserialize<Chamado>(item.Value.ToString());
                chamado.Id = item.Name;

                if (DateTime.TryParse(chamado.DataSelecionada, out DateTime dataAgendada))
                    chamado.DataSelecionada = dataAgendada.ToString("dd/MM/yyyy");

                if (DateTime.TryParse(chamado.DataAtual, out DateTime dataCriacao))
                    chamado.DataAtual = dataCriacao.ToString("dd/MM/yyyy");

                _todosChamados.Add(chamado);
            }

            ChamadosCollectionView.ItemsSource = _todosChamados;

            // Força o filtro conforme a seleção atual
            if (StatusPicker.SelectedItem != null)
            {
                StatusPicker_SelectedIndexChanged(StatusPicker, EventArgs.Empty);
            }
            else
            {
                // Caso ainda não tenha carregado o Picker (alternativa segura)
                StatusPicker.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void BTNVoltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NavegarMenus(_usuarioLogado));
    }
    private void StatusPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        string statusSelecionado = StatusPicker.SelectedItem?.ToString();

        if (string.IsNullOrEmpty(statusSelecionado) || statusSelecionado == "Todos")
        {
            ChamadosCollectionView.ItemsSource = _todosChamados;
        }
        else
        {
            var filtrados = _todosChamados
                .Where(c => c.Status?.Equals(statusSelecionado, StringComparison.OrdinalIgnoreCase) == true);
            ChamadosCollectionView.ItemsSource = new ObservableCollection<Chamado>(filtrados);
        }
    }

}