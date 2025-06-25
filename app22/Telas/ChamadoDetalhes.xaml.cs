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
    private List<string> _fotosBase64 = new();
    public ChamadoDetalhes(string idChamado)
    {
        InitializeComponent();
        _idChamado = idChamado;

        CarregarDadosChamadoPorIdAsync();
    }

    private async void FotosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ImageSource imagemSelecionada)
        {
            int index = FotosCollectionView.ItemsSource.Cast<ImageSource>().ToList().IndexOf(imagemSelecionada);

            if (index >= 0)
            {
                await Navigation.PushModalAsync(new FotoTelaCheia(_fotosBase64, index));
            }
        }

        FotosCollectionView.SelectedItem = null;
    }

    private async void Imagem_Tapped(object sender, EventArgs e)
    {
        if (sender is Image image && image.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tap)
        {
            var imagemSource = tap.CommandParameter as ImageSource;

            if (imagemSource != null)
            {
                // Identifica o índice da imagem base64 correspondente
                int index = -1;

                for (int i = 0; i < _fotosBase64.Count; i++)
                {
                    var bytes = Convert.FromBase64String(_fotosBase64[i]);
                    var source = ImageSource.FromStream(() => new MemoryStream(bytes));

                    if (imagemSource.ToString() == source.ToString())
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                {
                    await Navigation.PushModalAsync(new FotoTelaCheia(_fotosBase64, index));
                }
            }
        }
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

                        // Preencher dados na UI
                        EquipamentoLabel.Text = chamado.Value.Equipamento;
                        UsuarioLabel.Text = usuario.Key;
                        DataAgendamentoLabel.Text = chamado.Value.DataSelecionada;
                        HoraAgendamentoLabel.Text = chamado.Value.HoraSelecionada;
                        DataCriacaoLabel.Text = chamado.Value.DataAtual;
                        HoraCriacaoLabel.Text = chamado.Value.HoraAtual;
                        StatusPicker.SelectedItem = chamado.Value.Status;

                        // Fotos
                        _fotosBase64 = chamado.Value.FotosEquipamento ?? new List<string>();

                        if (_fotosBase64.Count > 0)
                        {
                            var imagens = new List<ImageSource>();

                            foreach (var fotoBase64 in _fotosBase64)
                            {
                                byte[] imagemBytes = Convert.FromBase64String(fotoBase64);
                                imagens.Add(ImageSource.FromStream(() => new MemoryStream(imagemBytes)));
                            }

                            FotosCollectionView.ItemsSource = imagens;
                        }

                        // Permitir edição apenas para usuários específicos
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