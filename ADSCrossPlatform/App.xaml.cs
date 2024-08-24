namespace ADSCrossPlatform
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            // Настройка DI
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Запуск начальной страницы
            MainPage = ServiceProvider.GetRequiredService<LoginPage>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<LoginPage>();
            services.AddTransient<MainPageAndroid>();
            // Другие сервисы
        }

        //private Page GetMainPage()
        //{
        //    // Определяем платформу и возвращаем соответствующую страницу обернутую в NavigationPage
        //    if (DeviceInfo.Platform == DevicePlatform.WinUI)
        //    {
        //        return new NavigationPage(new MainPageAndroid());
        //    }
        //    else if (DeviceInfo.Platform == DevicePlatform.Android)
        //    {
        //        return new NavigationPage(new MainPageWindows());
        //    }
        //    else
        //    {
        //        // По умолчанию возвращаем MainPageWindows обернутую в NavigationPage
        //        return new NavigationPage(new MainPageWindows());
        //    }
        //}
    }
}
