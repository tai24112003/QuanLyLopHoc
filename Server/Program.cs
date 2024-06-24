using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {// Khởi tạo Service Collection
            var services = new ServiceCollection();

            // Đăng ký các dịch vụ
            services.AddSingleton<IDataService, ApiService>();
            services.AddTransient<UserDAL>();
            services.AddTransient<SubjectDAL>();
            services.AddTransient<UserBLL>();
            services.AddTransient<SubjectBLL>();

            // Build ServiceProvider từ Service Collection
            var serviceProvider = services.BuildServiceProvider();

            // Sử dụng DI để khởi tạo MainForm
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartClassForm(
                serviceProvider.GetRequiredService<UserBLL>(),
                serviceProvider.GetRequiredService<SubjectBLL>()
            ));
        }
    }
}
