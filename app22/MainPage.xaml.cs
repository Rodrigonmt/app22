using System;
using Microsoft.Maui.Controls;
using app22.Telas;
using app22.Classes;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using Microsoft.Maui.Platform;

#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.Core.App;
#endif

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
            #if ANDROID
                var status = ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.Camera);
                if (status != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(Platform.CurrentActivity!, new string[] { Manifest.Permission.Camera }, 0);
                    await DisplayAlert("Permissão", "Permissão para usar a câmera é necessária.", "OK");
                    return;
                }
            #endif

            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    _fotoArquivo = await MediaPicker.Default.CapturePhotoAsync();

                    if (_fotoArquivo != null)
                    {
                        var stream = await _fotoArquivo.OpenReadAsync();

                        // ✅ Aqui aplica a imagem no controle com layout ajustado
                        ImagemEquipamentoPreview.Source = ImageSource.FromStream(() => stream);
                        ImagemEquipamentoPreview.IsVisible = true;
                        FrameImagemPreview.IsVisible = true;
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
                using var originalStream = await _fotoArquivo.OpenReadAsync();

                // Redimensionar/comprimir a imagem com SkiaSharp
                using var memoryStream = new MemoryStream();
                using var bitmap = SKBitmap.Decode(originalStream);

                // Reduz o tamanho para largura máxima de 800px mantendo proporção
                int targetWidth = 400;
                int targetHeight = (int)((double)bitmap.Height / bitmap.Width * targetWidth);

                using var resizedBitmap = bitmap.Resize(new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.Medium);
                using var image = SKImage.FromBitmap(resizedBitmap);
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, 25); // 50 = qualidade JPEG (0-100)

                data.SaveTo(memoryStream);

                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }

            if (botaoSelecionado == null)
            {
                await DisplayAlert("Erro", "Favor selecionar o equipamento com defeito", "Ok");
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
                await Navigation.PushAsync(new Chamados(_usuarioLog));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao salvar: {ex.Message}", "OK");
            }

        }

    }

}
