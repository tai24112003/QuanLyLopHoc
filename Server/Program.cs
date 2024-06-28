using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace Server
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Gán serviceProvider vào ServiceLocator
            ServiceLocator.ServiceProvider = serviceProvider;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(serviceProvider.GetRequiredService<StartClassForm>());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDataService, ApiService>();
            services.AddTransient<UserDAL>();
            services.AddTransient<RoomDAL>();
            services.AddTransient<SubjectDAL>();
            services.AddTransient<UserBLL>();
            services.AddTransient<RoomBLL>();
            services.AddTransient<SubjectBLL>();
            services.AddTransient<ClassSessionDAL>();
            services.AddTransient<SessionComputerDAL>();
            services.AddTransient<ClassSessionBLL>();
            services.AddTransient<SessionComputerBLL>();
            services.AddTransient<LocalDataHandler>();
            services.AddTransient<ClassSessionController>();

            services.AddTransient<StartClassForm>();
            services.AddTransient<svForm>();
        }
    }
}
