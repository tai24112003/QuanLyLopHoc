using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;

public class AttendanceBLL
{
    private readonly AttendanceDAL _AttendanceDAL;

    public AttendanceBLL(AttendanceDAL AttendanceDAL)
    {
        _AttendanceDAL = AttendanceDAL ?? throw new ArgumentNullException(nameof(AttendanceDAL));
    }
    public async Task<List<ClassSession>> GetAttendanceByClassID(int classID)
    {
        return await _AttendanceDAL.GetAttendanceByClassID(classID);
    }
    public async Task InsertAttendance(int sessionId, List<Attendance> Attendances)
    {
        try
        {
            await _AttendanceDAL.InsertAttendance(Attendances);
        }
        catch (Exception ex)
        {

            await _AttendanceDAL.InsertAttendanceLocal(sessionId, Attendances);
            Console.WriteLine("Error inserting session computer in BLL: " + ex.Message);
        }
    }


}
