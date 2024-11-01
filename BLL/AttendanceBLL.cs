using System;
using System.Collections.Generic;
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
            Console.WriteLine("Error inserting attendance in BLL: " + ex.Message);
        }
    }

    public async Task DeleteAttendanceBySessionID(int sessionID)
    {
        try
        {
            string response = await _AttendanceDAL.DeleteAttendanceBySessionID(sessionID);
            Console.WriteLine("Delete response: " + response);
        }
        catch (Exception ex)
        {
            bool success = await _AttendanceDAL.DeleteAttendanceLocal(sessionID);
            if (success)
            {
                Console.WriteLine("Deleted attendance locally for sessionID: " + sessionID);
            }
            else
            {
                Console.WriteLine("Error deleting attendance in BLL: " + ex.Message);
            }
        }
    }

    public async Task UpdateAttendanceLocal(Attendance attendance)
    {
        bool success = await _AttendanceDAL.UpdateAttendanceLocal(attendance);
        if (success)
        {
            Console.WriteLine("Attendance updated locally for AttendanceID: " + attendance.AttendanceID);
        }
        else
        {
            Console.WriteLine("Error updating attendance locally in BLL.");
        }
    }

    public async Task<List<Attendance>> LoadAttendancesLocal(int classID)
    {
        return await _AttendanceDAL.LoadAttendancesLocal(classID);
    }

    public async Task<List<Attendance>> GetAttendanceBySessionID(int sessionID)
    {
        return await _AttendanceDAL.GetAttendanceBySessionID(sessionID);
    }

}
