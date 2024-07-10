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

        public svForm Create(int userID, string roomID, int sessionID)
        {
            var roomBLL = _serviceProvider.GetService<RoomBLL>();
            var computerSessionController = _serviceProvider.GetService<ComputerSessionController>();

            var svForm = new svForm();
            svForm.Initialize(userID, roomID, roomBLL, sessionID, computerSessionController);

            return svForm;
        }
    }
}
