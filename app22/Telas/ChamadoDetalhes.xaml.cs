
namespace app22.Telas;

public partial class ChamadoDetalhes : ContentPage
{
    private string _idChamado; 
    public ChamadoDetalhes(string idChamado)
    {
        InitializeComponent();
        _idChamado = idChamado;

        // Aqui voc� pode carregar os dados do Firebase com o ID

    }

    

}