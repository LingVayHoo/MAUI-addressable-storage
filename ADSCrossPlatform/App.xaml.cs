using ADSCrossPlatform.Code.Models;

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
            //Регистрация страниц
            services.AddTransient<LoginPage>();
            services.AddTransient<MainPageAndroid>();
            services.AddTransient<Window_310>();
            services.AddTransient<Window_310_Back>();
            services.AddTransient<Window_390>();
            services.AddTransient<Details>();
            services.AddTransient<NewAddressPage>();

            // Регистрация зависимостей
            services.AddSingleton<SecureSettings>();
            services.AddSingleton<DataManager>();
            services.AddSingleton<AddressViewModel>();
            services.AddSingleton<StoredSettings>();
            services.AddSingleton<WebApiModel>();
        }
    }
}
