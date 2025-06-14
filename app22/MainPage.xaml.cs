using System;
using Microsoft.Maui.Controls;

namespace app22
{
    public partial class MainPage : ContentPage
    {
       

        public MainPage()
        {
            InitializeComponent();
            CorrerLetras();
        }

        private async void CorrerLetras()
        {
            await Task.Delay(500);
            double labelWidth = teste25.Width;
            double containerWidth = teste26.Width;
            while (true)
            {
                teste25.TranslationX = containerWidth;
                await teste25.TranslateTo(-labelWidth, 0, 10000, Easing.Linear);
            }
        }


    }

}
