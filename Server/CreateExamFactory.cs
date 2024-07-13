// svFormFactory.cs

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Server
{
    public class CreateExamFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CreateExamFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public CreateExam Create(int user_id)
        {
            var exam = _serviceProvider.GetService<ExamBLL>();

            var createExam = new CreateExam();
            createExam.Initialize(user_id, exam);

            return createExam;
        }
    }
}
