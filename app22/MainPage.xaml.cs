using System;
using Microsoft.Maui.Controls;
using app22.Telas;
using app22.Classes;
using Microsoft.Maui.Platform;

namespace app22
{
    public partial class MainPage : ContentPage
    {

        private Button botaoSelecionado;

        public MainPage()
        {
            InitializeComponent();
            //CorrerLetras();

            string? _usuarioLogado = null;//variavel aceita valor null com o ?
            Task.Run(async () =>
            {
                _usuarioLogado = await SecureStorage.Default.GetAsync("usuario_logado");
                lbl_boasvindas.Text = $"Bem vindo(a) " + _usuarioLogado;
            });
        }
        private async void Desconnectar_Clicked(object sender, EventArgs e)
        {
            bool conf = await DisplayAlert("Tem certeza?", "Sair do app", "Sim", "Não");
            if (conf)
            {
                SecureStorage.Default.Remove("usuario_logado");
                App.Current.MainPage = new Login();
            }
        }
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            // Ação ao clicar (por exemplo, voltar)
            App.Current.MainPage = new Login();
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

                var userId = await SecureStorage.GetAsync("usuario_logado");

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
                    HoraAtual = horaAtual
                };

                var firebaseService = new FirebaseService();
                await firebaseService.SalvarAgendamentoAsync(userId, agendamento);

                await DisplayAlert("Sucesso", "Agendamento salvo com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao salvar: {ex.Message}", "OK");
            }


        }

    }

}
