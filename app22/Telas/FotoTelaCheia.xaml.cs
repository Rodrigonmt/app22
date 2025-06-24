using System;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.IO;

namespace app22.Telas;

public partial class FotoTelaCheia : ContentPage
{
    private List<string> _fotosBase64;
    private int _indiceAtual;
    private double _currentScale = 1;
    private double _startScale = 1;

    public FotoTelaCheia(List<string> fotosBase64, int indiceInicial)
    {
        InitializeComponent();
        _fotosBase64 = fotosBase64;
        _indiceAtual = indiceInicial;
        MostrarImagem();
    }

    private void MostrarImagem()
    {
        _currentScale = 1;
        ImagemTelaCheia.Scale = 1;

        if (_indiceAtual >= 0 && _indiceAtual < _fotosBase64.Count)
        {
            var bytes = Convert.FromBase64String(_fotosBase64[_indiceAtual]);
            ImagemTelaCheia.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        }
    }

    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            _startScale = ImagemTelaCheia.Scale;
        }
        else if (e.Status == GestureStatus.Running)
        {
            ImagemTelaCheia.Scale = Math.Max(1, _startScale * e.Scale);
        }
    }

    private void Proxima_Clicked(object sender, EventArgs e)
    {
        _indiceAtual = (_indiceAtual + 1) % _fotosBase64.Count;
        MostrarImagem();
    }

    private void Anterior_Clicked(object sender, EventArgs e)
    {
        _indiceAtual = (_indiceAtual - 1 + _fotosBase64.Count) % _fotosBase64.Count;
        MostrarImagem();
    }

    private async void Fechar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
