﻿using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<ClassDAL>();
            services.AddTransient<ClassStudentDAL>();
            //services.AddTransient<ClassSubjectDAL>();
            services.AddTransient<UserDAL>();
            services.AddTransient<RoomDAL>();
            services.AddTransient<SubjectDAL>();
            services.AddTransient<StudentDAL>();
            services.AddTransient<StudentBLL>();
            services.AddTransient<AttendanceBLL>();
            services.AddTransient<AttendanceDAL>();
            services.AddTransient<UserBLL>();
            services.AddTransient<ClassBLL>();
            services.AddTransient<RoomBLL>();
            services.AddTransient<SubjectBLL>();
            services.AddTransient<ClassSessionDAL>();
            services.AddTransient<SessionComputerDAL>();
            services.AddTransient<ComputerDAL>();
            services.AddTransient<ClassSessionBLL>();
            //services.AddTransient<ClassSubjectBLL>();
            services.AddTransient<ClassStudentBLL>();
            services.AddTransient<ExamBLL>();
            services.AddTransient<SessionComputerBLL>();
            services.AddTransient<ComputerBLL>();
            services.AddTransient<SubmisstionBLL>();
            services.AddTransient<SubmisstionDAL>();
            services.AddTransient<LocalDataHandler>();
            services.AddTransient<ExcelController>();

            services.AddTransient<svFormFactory>();
            services.AddTransient<StartClassForm>();
            services.AddTransient<svForm>();
            services.AddTransient<CreateExam>();
            services.AddTransient<CreateExamFactory>();
        }
    }
}
