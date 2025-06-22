using System.Text;
using System.Text.Json;
using app22.Classes;

namespace app22.Telas;

public partial class ChamadoDetalhes : ContentPage
{
    private readonly string _idChamado;
    private readonly FirebaseService _firebaseService = new();
    private string _usuarioLogado;
    private string _usuarioDonoDoChamado;
    private AgendamentoModel _chamadoAtual;
    public ChamadoDetalhes(string idChamado)
    {
        InitializeComponent();
        _idChamado = idChamado;

        CarregarDadosChamadoPorIdAsync();
    }

    private async void Imagem_Tapped(object sender, EventArgs e)
    {
        if (sender is Image image && image.Source != null)
        {
            await Navigation.PushModalAsync(new FotoTelaCheia(image.Source));
        }
    }

    private async void FotosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ImageSource imagemSelecionada)
        {
            await Navigation.PushModalAsync(new FotoTelaCheia(imagemSelecionada));
        }

        // Limpa a seleção para permitir reabrir se a mesma imagem for clicada de novo
        FotosCollectionView.SelectedItem = null;
    }

    private async void CarregarDadosChamadoPorIdAsync()
    {
        try
        {
            _usuarioLogado = PreferenciasUsuario.ObterUsuario();

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos.json");

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Erro ao acessar Firebase.", "OK");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var todosChamados = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, AgendamentoModel>>>(json);

            foreach (var usuario in todosChamados)
            {
                foreach (var chamado in usuario.Value)
                {
                    if (chamado.Key == _idChamado)
                    {
                        _usuarioDonoDoChamado = usuario.Key;
                        _chamadoAtual = chamado.Value;

                        // Preencher UI
                        EquipamentoLabel.Text = chamado.Value.Equipamento;
                        UsuarioLabel.Text = usuario.Key;
                        DataAgendamentoLabel.Text = chamado.Value.DataSelecionada;
                        HoraAgendamentoLabel.Text = chamado.Value.HoraSelecionada;
                        DataCriacaoLabel.Text = chamado.Value.DataAtual;
                        HoraCriacaoLabel.Text = chamado.Value.HoraAtual;
                        StatusPicker.SelectedItem = chamado.Value.Status;

                        if (chamado.Value.FotosEquipamento != null && chamado.Value.FotosEquipamento.Count > 0)
                        {
                            if (_chamadoAtual.FotosEquipamento != null && _chamadoAtual.FotosEquipamento.Count > 0)
                            {
                                var imagens = new List<ImageSource>();

                                foreach (var fotoBase64 in _chamadoAtual.FotosEquipamento)
                                {
                                    byte[] imagemBytes = Convert.FromBase64String(fotoBase64);
                                    imagens.Add(ImageSource.FromStream(() => new MemoryStream(imagemBytes)));
                                }

                                FotosCollectionView.ItemsSource = imagens;
                            }
                        }

                        // Permitir edição para Rodrigo ou Rafael
                        if (_usuarioLogado == "Rodrigo" || _usuarioLogado == "Rafael")
                        {
                            StatusPicker.IsEnabled = true;
                            SalvarButton.IsVisible = true;
                        }

                        return;
                    }
                }
            }

            await DisplayAlert("Aviso", "Chamado não encontrado.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", "Falha ao carregar dados: " + ex.Message, "OK");
        }
    }

    private async void SalvarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var novoStatus = StatusPicker.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(novoStatus)) return;

            _chamadoAtual.Status = novoStatus;

            var json = JsonSerializer.Serialize(_chamadoAtual);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://agendaluiz-default-rtdb.firebaseio.com/Agendamentos/{_usuarioDonoDoChamado}/{_idChamado}.json";
            var response = await new HttpClient().PutAsync(url, content);

            if (response.IsSuccessStatusCode)
                await DisplayAlert("Sucesso", "Status atualizado com sucesso.", "OK");
            else
                await DisplayAlert("Erro", "Erro ao atualizar no Firebase.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao salvar: {ex.Message}", "OK");
        }
    }

}