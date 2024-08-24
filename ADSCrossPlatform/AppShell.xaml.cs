namespace ADSCrossPlatform
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Window_310), typeof(Window_310));
            Routing.RegisterRoute(nameof(Details), typeof(Details));
        }
    }
}
