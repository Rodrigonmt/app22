using System.Text.Json;
using Microsoft.Maui.Controls;
using app22.Classes;
using System.Net.Http;
using System.Collections.ObjectModel;
using System;
using System.Text;

namespace app22.Telas;

public partial class ChamadosAdm : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string _usuarioLogado = "Amanda";
    private ObservableCollection<Chamado> _todosChamados = new ObservableCollection<Chamado>();
    private List<string> _usuariosDisponiveis = new List<string>();
    public ChamadosAdm()
	{
		InitializeComponent();
        _ = CarregarUsuariosAsync(); // adiciona essa linha
        _ = CarregarChamadosAsync();
    }

    private void AplicarFiltros_Clicked(object sender, EventArgs e)
    {
        string statusSelecionado = StatusPicker.SelectedItem?.ToString();
        string equipamentoSelecionado = EquipamentoPicker.SelectedItem?.ToString();

        /*
         string statusSelecionado = StatusPicker.SelectedItem?.ToString();
        string equipamentoFiltro = EquipamentoEntry.Text?.ToLower()?.Trim();
         */

        DateTime? dataAgendadaIni = DataAgendadaInicio.Date;
        DateTime? dataAgendadaFim = DataAgendadaFim.Date;
        DateTime? dataCriacaoIni = DataCriacaoInicio.Date;
        DateTime? dataCriacaoFim = DataCriacaoFim.Date;

        var filtrados = _todosChamados.Where(c =>
        {
            bool statusOK = statusSelecionado == "Todos" || c.Status?.Equals(statusSelecionado, StringComparison.OrdinalIgnoreCase) == true;
            bool equipamentoOK = equipamentoSelecionado == "Todos" ||
                     c.Equipamento?.Equals(equipamentoSelecionado, StringComparison.OrdinalIgnoreCase) == true;


            bool agendadaOK = DateTime.TryParse(c.DataSelecionada, out DateTime dataAgendada)
                              && dataAgendada >= dataAgendadaIni && dataAgendada <= dataAgendadaFim;

            bool criacaoOK = DateTime.TryParse(c.DataAtual, out DateTime dataCriacao)
                             && dataCriacao >= dataCriacaoIni && dataCriacao <= dataCriacaoFim;

            return statusOK && equipamentoOK && agendadaOK && criacaoOK;
        });

        ChamadosCollectionView.ItemsSource = new ObservableCollection<Chamado>(filtrados);
    }

    private void Filtro_TextChanged(object sender, TextChangedEventArgs e)
    {
        AplicarFiltros_Clicked(null, null);
    }

    private void FiltroEquipamento_Changed(object sender, EventArgs e)
    {
        AplicarFiltros_Clicked(null, null);
    }

    private async void CancelarChamado_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Chamado chamado)
        {
            bool confirmar = await DisplayAlert("Confirmar", $"Deseja cancelar o chamado do equipamento '{chamado.Equipamento}'?", "Sim", "N�o");

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
                    await DisplayAlert("Erro", "N�o foi poss�vel cancelar o chamado.", "OK");
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
                await DisplayAlert("Aviso", "Usu�rio n�o possui chamados abertos", "OK");
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

            // For�a o filtro conforme a sele��o atual
            if (StatusPicker.SelectedItem != null)
            {
                StatusPicker_SelectedIndexChanged(StatusPicker, EventArgs.Empty);
            }
            else
            {
                // Caso ainda n�o tenha carregado o Picker (alternativa segura)
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
        App.Current.MainPage = new NavegarMenus(_usuarioLogado);
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
    private async Task CarregarUsuariosAsync()
    {
        try
        {
            string url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos.json";
            var response = await _httpClient.GetStringAsync(url);

            if (string.IsNullOrWhiteSpace(response) || response == "null")
                return;

            var jsonDoc = JsonDocument.Parse(response);
            _usuariosDisponiveis = jsonDoc.RootElement.EnumerateObject().Select(u => u.Name).ToList();

            UsuarioPicker.ItemsSource = _usuariosDisponiveis;
            UsuarioPicker.SelectedItem = _usuarioLogado; // Seleciona usu�rio atual por padr�o
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Erro ao carregar usu�rios: " + ex.Message, "OK");
        }
    }

    private async void UsuarioPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (UsuarioPicker.SelectedItem is string usuarioSelecionado)
        {
            _usuarioLogado = usuarioSelecionado;
            await CarregarChamadosAsync();
        }
    }
}