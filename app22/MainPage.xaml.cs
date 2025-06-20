using System;
using Microsoft.Maui.Controls;
using app22.Telas;
using app22.Classes;
using Microsoft.Maui.Platform;

/*Status serviços
Pendente
Em Andamento
Coleta efetuada
Concluído
Cancelado
Reagendado
 
*/

namespace app22
{
    public partial class MainPage : ContentPage
    {
        private FileResult _fotoArquivo;
        private Button botaoSelecionado;
        public string? _usuarioLog = null;//variavel aceita valor null com o ?

        public MainPage(string _usuarioLogado)
        {
            InitializeComponent();
            _usuarioLog = _usuarioLogado;
            atualizarBoasVindas();
            
        }

        private async void BtnTirarFoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    _fotoArquivo = await MediaPicker.Default.CapturePhotoAsync();

                    if (_fotoArquivo != null)
                    {
                        var stream = await _fotoArquivo.OpenReadAsync();
                        ImagemEquipamentoPreview.Source = ImageSource.FromStream(() => stream);
                        ImagemEquipamentoPreview.IsVisible = true;
                    }
                }
                else
                {
                    await DisplayAlert("Erro", "Captura de imagem não suportada neste dispositivo.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao capturar imagem: {ex.Message}", "OK");
            }
        }

        private void atualizarBoasVindas()
        {
            lbl_boasvindas.Text = $"Bem vindo(a) " + _usuarioLog;
        }
        
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            // Ação ao clicar (por exemplo, voltar)
            App.Current.MainPage = new NavegarMenus();
        }

        private void MarcarBotao_Clicked(object sender, EventArgs e)
        {
            var botaoClicado = (Button)sender;

            // Desmarca o botão anterior
            if (botaoSelecionado != null)
            {
                botaoSelecionado.BackgroundColor = Colors.Transparent;
            }

            // Marca o botão atual
            botaoClicado.BackgroundColor = Colors.Red;
            botaoClicado.BackgroundColor = Color.FromRgba(211, 211, 211, 0.9);

            // Garante que o botão preencha o espaço (opcional, mas recomendado)
            botaoClicado.HorizontalOptions = LayoutOptions.Fill;
            botaoClicado.VerticalOptions = LayoutOptions.Fill;
            botaoClicado.Padding = new Thickness(0);
            botaoClicado.Margin = new Thickness(0);

            // Atualiza o selecionado
            botaoSelecionado = botaoClicado;
        }

        private async void BTNAgendar_Clicked(object sender, EventArgs e)
        {
            string base64Image = null;

            if (_fotoArquivo != null)
            {
                using var stream = await _fotoArquivo.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }

            if (botaoSelecionado==null)
            {
                DisplayAlert("Erro", "Favor selecionar o equipamento com defeito", "Ok");
                return;
            }

            try
            {
                var botaoSelecionadoTexto = botaoSelecionado?.Text ?? "Nenhum";

                var dataSelecionada = DataAgendamento.Date.ToString("yyyy-MM-dd");
                var horaSelecionada = HoraAgendamento.Time.ToString(@"hh\:mm");

                var dataAtual = DateTime.Now.ToString("yyyy-MM-dd");
                var horaAtual = DateTime.Now.ToString("HH:mm");

                var userId = _usuarioLog;

                if (string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Erro", "Usuário não identificado no dispositivo.", "OK");
                    return;
                }

                var agendamento = new AgendamentoModel
                {
                    Equipamento = botaoSelecionadoTexto,
                    DataSelecionada = dataSelecionada,
                    HoraSelecionada = horaSelecionada,
                    DataAtual = dataAtual,
                    HoraAtual = horaAtual,
                    Status = "Pendente",
                    FotoEquipamento = base64Image
                };

                var firebaseService = new FirebaseService();
                await firebaseService.SalvarAgendamentoAsync(userId, agendamento);

                await DisplayAlert("Sucesso", "Agendamento salvo com sucesso!", "OK");
                App.Current.MainPage = new Chamados();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao salvar: {ex.Message}", "OK");
            }


        }

    }

}
