using System;
using Microsoft.Maui.Controls;
using app22.Telas;
using app22.Classes;

namespace app22
{
    public partial class MainPage : ContentPage
    {
       

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



    }

}
