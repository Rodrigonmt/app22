using app22.Telas;
namespace app22
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Login());
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new NavigationPage(new Login()));
        //}
    }
}