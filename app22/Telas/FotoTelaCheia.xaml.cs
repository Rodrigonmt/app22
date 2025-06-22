namespace app22.Telas;

public partial class FotoTelaCheia : ContentPage
{
	public FotoTelaCheia(ImageSource imagem)
	{
		InitializeComponent();
        ImagemTelaCheia.Source = imagem;
    }
    private async void Fechar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}