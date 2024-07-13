    using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

    public class RoomDAL
    {
        private readonly IDataService _dataService;

        public RoomDAL(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<string> GetRoomByRoomID(string roomID)
        {
            try
            {
                string roomJson = await _dataService.GetAsync($"room/{roomID}");
                return roomJson;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

    }
