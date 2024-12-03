// svFormFactory.cs

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Server
{
    public class svFormFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public svFormFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public svForm Create(int userID, string roomID, ClassSession sessionID,int classID)
        {
            var roomBLL = _serviceProvider.GetService<RoomBLL>();
            var sessionComputerBLL = _serviceProvider.GetService<SessionComputerBLL>();
            var classSession = _serviceProvider.GetService<ClassSessionBLL>();
            var classStudent = _serviceProvider.GetService<ClassStudentBLL>();
            var attendance = _serviceProvider.GetService<AttendanceBLL>();
            var computer = _serviceProvider.GetService<ComputerBLL>();
            var student = _serviceProvider.GetService<StudentBLL>();

            var svForm = new svForm();
            svForm.Initialize(userID, roomID,classID, roomBLL, sessionID, sessionComputerBLL,classSession,classStudent,attendance,computer,student, _serviceProvider);

            return svForm;
        }
    }
}
