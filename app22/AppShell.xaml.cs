using app22.Telas;

namespace app22
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Login), typeof(Login));
            Routing.RegisterRoute(nameof(NavegarMenus), typeof(NavegarMenus));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(Cadastro), typeof(Cadastro));
            Routing.RegisterRoute(nameof(Chamados), typeof(Chamados));
            Routing.RegisterRoute(nameof(ChamadosAdm), typeof(ChamadosAdm));
            Routing.RegisterRoute(nameof(AlteraCadastro), typeof(AlteraCadastro));
            Routing.RegisterRoute(nameof(ChamadoDetalhes), typeof(ChamadoDetalhes));
        }
    }
}
