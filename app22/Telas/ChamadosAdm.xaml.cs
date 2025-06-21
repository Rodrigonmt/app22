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
    private string _userlog;
    private string _usuarioretornar;
    private ObservableCollection<Chamado> _todosChamados = new ObservableCollection<Chamado>();
    private List<string> _usuariosDisponiveis = new List<string>();
    public ChamadosAdm(string usuarlogretorn)
	{
        InitializeComponent();

        StatusPicker.SelectedIndex = 0;
        EquipamentoPicker.SelectedIndex = 0;

        // Define intervalo de 30 dias antes e depois da data atual
        DataAgendadaInicio.Date = DateTime.Today.AddDays(-30);
        DataAgendadaFim.Date = DateTime.Today.AddDays(30);
        DataCriacaoInicio.Date = DateTime.Today.AddDays(-30);

        // ? Primeiro carrega os usu�rios e ent�o os chamados
        _ = InicializarDadosAsync();
        _usuarioretornar = usuarlogretorn;
    }



    private async Task InicializarDadosAsync()
    {
        await CarregarUsuariosAsync(); // Isso define "Todos" como padr�o
        await CarregarChamadosAsync(); // Agora esse m�todo vai buscar todos os chamados
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

    private async Task CarregarChamadosAsync()
    {
        _todosChamados.Clear();
        try
        {
            string url;

            if (UsuarioPicker.SelectedItem?.ToString() == "Todos")
            {
                // Buscar todos os usu�rios
                url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos.json";
            }
            else
            {
                _userlog = UsuarioPicker.SelectedItem?.ToString(); // atualiza localmente
                url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos/{_userlog}.json";
            }

            var response = await _httpClient.GetStringAsync(url);

            if (string.IsNullOrWhiteSpace(response) || response == "null")
            {
                await DisplayAlert("Aviso", "Nenhum chamado encontrado", "OK");
                return;
            }

            var jsonDoc = JsonDocument.Parse(response);

            if (UsuarioPicker.SelectedItem?.ToString() == "Todos")
            {
                foreach (var usuario in jsonDoc.RootElement.EnumerateObject())
                {
                    foreach (var item in usuario.Value.EnumerateObject())
                    {
                        var chamado = JsonSerializer.Deserialize<Chamado>(item.Value.ToString());
                        chamado.Id = item.Name;

                        if (DateTime.TryParse(chamado.DataSelecionada, out DateTime dataAgendada))
                            chamado.DataSelecionada = dataAgendada.ToString("dd/MM/yyyy");

                        if (DateTime.TryParse(chamado.DataAtual, out DateTime dataCriacao))
                            chamado.DataAtual = dataCriacao.ToString("dd/MM/yyyy");

                        _todosChamados.Add(chamado);
                    }
                }
            }
            else
            {
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
            }

            ChamadosCollectionView.ItemsSource = _todosChamados;

            // Aplica o filtro conforme status selecionado
            StatusPicker_SelectedIndexChanged(StatusPicker, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
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

            _usuariosDisponiveis.Insert(0, "Todos"); // ? adiciona a op��o "Todos" no topo

            UsuarioPicker.ItemsSource = _usuariosDisponiveis;
            UsuarioPicker.SelectedItem = "Todos"; // ? seleciona "Todos" como padr�o
            _userlog = null; // ? define para buscar todos os usu�rios

            await CarregarChamadosAsync(); // recarrega com todos os chamados
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
            _userlog = usuarioSelecionado;
            await CarregarChamadosAsync();
        }
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
}