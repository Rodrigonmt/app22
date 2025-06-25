using System;
using Microsoft.Maui.Controls;
using app22.Telas;
using app22.Classes;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.Collections.ObjectModel;

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
        private List<FileResult> _fotosArquivos = new();
        private const int MaxFotos = 5;
        private ObservableCollection<ImageSource> _miniaturas = new();

        public MainPage(string _usuarioLogado)
        {
            InitializeComponent();
            _usuarioLog = _usuarioLogado;

        }

        private async Task TirarFotosSequencialAsync()
        {
            while (_fotosArquivos.Count < MaxFotos)
            {
                var foto = await MediaPicker.Default.CapturePhotoAsync();

                if (foto == null)
                    break; // usuário cancelou

                _fotosArquivos.Add(foto);

                // Mostrar a prévia da última
                using var stream = await foto.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var streamCopia = new MemoryStream(memoryStream.ToArray());
                // Atualiza miniaturas
                streamCopia.Position = 0; // resetar posição
                ImageSource imagemMiniatura = ImageSource.FromStream(() => new MemoryStream(streamCopia.ToArray()));
                _miniaturas.Add(imagemMiniatura);
                MiniaturasCollection.ItemsSource = _miniaturas;
                MiniaturasCollection.IsVisible = true;

                // Pergunta ao usuário se deseja continuar
                bool continuar = await DisplayAlert("Foto capturada",
                                                    $"Você tirou {_fotosArquivos.Count}/{MaxFotos} fotos.\nDeseja tirar outra?",
                                                    "Sim", "Não");
                if (!continuar)
                    break;
            }
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
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await DisplayAlert("Erro", "Captura de imagem não suportada neste dispositivo.", "OK");
                    return;
                }

                await TirarFotosSequencialAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao capturar imagem: {ex.Message}", "OK");
            }
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
            List<string> listaFotosBase64 = new();

            foreach (var foto in _fotosArquivos)
            {
                using var originalStream = await foto.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await originalStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var bitmap = SKBitmap.Decode(memoryStream);

                int targetWidth = (int)(bitmap.Width * 0.2);
                int targetHeight = (int)(bitmap.Height * 0.2);

                using var resizedBitmap = bitmap.Resize(new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.Medium);
                using var image = SKImage.FromBitmap(resizedBitmap);
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, 60);

                using var resizedStream = new MemoryStream();
                data.SaveTo(resizedStream);

                string base64 = Convert.ToBase64String(resizedStream.ToArray());
                listaFotosBase64.Add(base64);
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
                    FotosEquipamento = listaFotosBase64 // ✅ aqui vai a lista
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LimparTela();
        }
        private void LimparTela()
        {
            _fotosArquivos.Clear();
            _miniaturas.Clear();
            MiniaturasCollection.ItemsSource = _miniaturas;
            MiniaturasCollection.IsVisible = false;

            if (botaoSelecionado != null)
            {
                botaoSelecionado.BackgroundColor = Colors.Transparent;
                botaoSelecionado.HorizontalOptions = LayoutOptions.Start;
                botaoSelecionado.VerticalOptions = LayoutOptions.Center;
                botaoSelecionado.Padding = new Thickness(10); // ou o valor original
                botaoSelecionado.Margin = new Thickness(5);   // ou o valor original
            }

            botaoSelecionado = null;

            // Resetar botões de equipamento
            foreach (var view in GetAllChildren(LayoutRoot))
            {
                if (view is Button btn &&
                    btn != BTNAgendar &&
                    btn != BtnTirarFoto)
                {
                    btn.BackgroundColor = Colors.Transparent;
                }
            }

            // Resetar campos de data/hora
            DataAgendamento.Date = DateTime.Today;
            HoraAgendamento.Time = TimeSpan.Zero;
        }


        private IEnumerable<View> GetAllChildren(IView view)
        {
            if (view is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    if (child is View childView)
                    {
                        yield return childView;

                        foreach (var descendant in GetAllChildren(childView))
                        {
                            yield return descendant;
                        }
                    }
                }
            }
            else if (view is ContentView contentView && contentView.Content is View content)
            {
                yield return content;

                foreach (var descendant in GetAllChildren(content))
                {
                    yield return descendant;
                }
            }
            else if (view is Grid grid)
            {
                foreach (var child in grid.Children)
                {
                    if (child is View childView)
                    {
                        yield return childView;

                        foreach (var descendant in GetAllChildren(childView))
                        {
                            yield return descendant;
                        }
                    }
                }
            }
        }



    }

}
