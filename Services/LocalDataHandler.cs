using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Globalization;

public class LocalDataHandler
{
    private readonly ClassSessionBLL _classSessionBLL;
    private readonly SessionComputerBLL _sessionComputerBLL;
    private readonly ClassBLL _classBLL;
    private readonly StudentBLL _studentBLL;
    private readonly ComputerBLL _computerBLL;
    private readonly SettingBLL _settingBLL;
    private readonly ClassStudentBLL _classStudentBLL;
    private readonly AttendanceBLL _attendanceBLL;
    private readonly RoomBLL _roomBLL;

    public LocalDataHandler(RoomBLL roomBLL, ComputerBLL computerBLL, SettingBLL settingBLL, AttendanceBLL attendanceBLL,
                            ClassSessionBLL classSessionBLL, SessionComputerBLL sessionComputerBLL,
                            ClassBLL classBLL, StudentBLL studentBLL, ClassStudentBLL classStudentBLL)
    {
        _classSessionBLL = classSessionBLL ?? throw new ArgumentNullException(nameof(classSessionBLL));
        _sessionComputerBLL = sessionComputerBLL ?? throw new ArgumentNullException(nameof(sessionComputerBLL));
        _classBLL = classBLL ?? throw new ArgumentNullException(nameof(classBLL));
        _studentBLL = studentBLL ?? throw new ArgumentNullException(nameof(studentBLL));
        _classStudentBLL = classStudentBLL ?? throw new ArgumentNullException(nameof(classStudentBLL));
        _attendanceBLL = attendanceBLL ?? throw new ArgumentNullException(nameof(attendanceBLL));
        _settingBLL = settingBLL ?? throw new ArgumentNullException(nameof(settingBLL));
        _computerBLL = computerBLL ?? throw new ArgumentNullException(nameof(computerBLL));
        _roomBLL = roomBLL ?? throw new ArgumentNullException(nameof(roomBLL));
    }

    public async Task MigrateData()
    {
        try
        {
            if (await MigrateComputerAndStudentData())
            {
                await MigrateClassAndStudentData();
                await MigrateDataForClassSessions();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during data migration: {ex}");
        }
    }

    private async Task MigrateClassAndStudentData()
    {
        try
        {
            var negativeClasses = await _classBLL.LoadNegativeIDClasses();
            if (negativeClasses == null) return;

            foreach (var classItem in negativeClasses)
            {
                await MigrateClassData(classItem);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error migrating class and student data: {ex}");
        }
    }

    private async Task MigrateClassData(Class classItem)
    {
        int classID_A = classItem.ClassID;
        var classStudents = await _classStudentBLL.GetClassStudentsByClassID(classID_A);
        var negativeStudents = await _studentBLL.LoadNegativeIDStudentes();

        int classID_B = (await _classBLL.InsertClass(classItem)).ClassID;
        await _classStudentBLL.DeleteClassStudentsByClassID(classID_A);

        if (negativeStudents != null)
        {
            foreach (var student in negativeStudents)
            {
                await DeleteAndMigrateStudent(student);
            }
        }

        await _studentBLL.InsertStudent(negativeStudents);
        await _studentBLL.InsertStudentLocal(negativeStudents);

        foreach (var classStudent in classStudents)
        {
            classStudent.ClassID = classID_B;
            classStudent.StudentID = GeneratePositiveID(classStudent.StudentID);
        }

        await _classStudentBLL.InsertClassStudent(classStudents);
        await _classSessionBLL.UpdateClassSessionClassID(classID_A, classID_B);
        await _classBLL.DeleteClasssByClassID(classID_A);

    }

    private async Task DeleteAndMigrateStudent(Student student)
    {
        await _studentBLL.DeleteStudent(student.StudentID);
        student.StudentID = GeneratePositiveID(student.StudentID);
    }

    private async Task MigrateDataForClassSessions()
    {
        try
        {
            var negativeClassSessions = await _classSessionBLL.LoadNegativeIDClasseSessionAsync();
            if (negativeClassSessions != null)

                foreach (var classSession in negativeClassSessions)
                {
                    await MigrateClassSessionData(classSession);
                }

            var attendanceRecords = await _attendanceBLL.GetAttendanceBySessionIDNegative();
            var sessionComputers = await _sessionComputerBLL.GetSessionComputersBySessionIDNegative();

            // Kiểm tra null hoặc không có phần tử
            if ((attendanceRecords == null || attendanceRecords.Count == 0) &&
                (sessionComputers == null || sessionComputers.Count == 0))
            {
                return;
            }

            // Chỉ thực hiện xóa khi có dữ liệu
            if (attendanceRecords != null && attendanceRecords.Count > 0)
            {
                await _attendanceBLL.DeleteAttendanceBySessionID(attendanceRecords[0].SessionID);
            }
            if (sessionComputers != null && sessionComputers.Count > 0)
            {
                await _sessionComputerBLL.DeleteSessionComputersBySessionID(sessionComputers[0].SessionID);
            }

            // Chỉ tiếp tục xử lý khi có dữ liệu trong attendanceRecords
            if (attendanceRecords != null && attendanceRecords.Count > 0)
            {
                int sessionID_B = int.Parse(GeneratePositiveID(attendanceRecords[0].SessionID.ToString()));

                foreach (var attendance in attendanceRecords)
                {
                    attendance.SessionID = sessionID_B;
                    attendance.StudentID = GeneratePositiveID(attendance.StudentID);
                }
                await _attendanceBLL.InsertAttendance(sessionID_B, attendanceRecords);

                // Chỉ xử lý sessionComputers nếu nó không null hoặc rỗng
                if (sessionComputers != null && sessionComputers.Count > 0)
                {
                    foreach (var sessionComputer in sessionComputers)
                    {
                        sessionComputer.SessionID = sessionID_B;
                    }
                    await _sessionComputerBLL.InsertSessionComputer(sessionID_B, sessionComputers);
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error migrating class sessions data: {ex}");
        }
    }

    private async Task MigrateClassSessionData(ClassSession classSession)
    {
        int sessionID_A = classSession.SessionID;
        var attendanceRecords = await _attendanceBLL.GetAttendanceBySessionID(sessionID_A);
        var sessionComputers = await _sessionComputerBLL.GetSessionComputersBySessionID(sessionID_A);

        var newClassSession = await _classSessionBLL.InsertClassSession(classSession);
        int sessionID_B = newClassSession.SessionID;

        await _attendanceBLL.DeleteAttendanceBySessionID(sessionID_A);
        await _sessionComputerBLL.DeleteSessionComputersBySessionID(sessionID_A);

        foreach (var attendance in attendanceRecords)
        {
            attendance.SessionID = sessionID_B;
            attendance.StudentID = GeneratePositiveID(attendance.StudentID);
        }
        await _attendanceBLL.InsertAttendance(sessionID_B, attendanceRecords);
        if (sessionComputers != null)
        {
            foreach (var sessionComputer in sessionComputers)
            {
                sessionComputer.SessionID = sessionID_B;
            }
            await _sessionComputerBLL.InsertSessionComputer(sessionID_B, sessionComputers);
        }





    }

    private async Task<bool> MigrateComputerAndStudentData()
    {
        try
        {
            var settingLocal = await _settingBLL.GetSettingLocal();
            var settingServer = await _settingBLL.GetSettingServer();
            if (settingServer != null && settingLocal != null)
            {
                string dateFormat = "dd/MM/yyyy HH:mm:ss"; // Định dạng cụ thể của chuỗi ngày tháng
                CultureInfo provider = CultureInfo.InvariantCulture;

                    DateTime lastTimeUpdateStudentLocal = DateTime.ParseExact(settingLocal.lastTimeUpdateStudent, dateFormat, provider);
                    DateTime lastTimeUpdateStudentServer = DateTime.ParseExact(settingServer.lastTimeUpdateStudent, dateFormat, provider);

                    DateTime lastTimeUpdateComputerLocal = DateTime.ParseExact(settingLocal.lastTimeUpdateComputer, dateFormat, provider);
                    DateTime lastTimeUpdateComputerServer = DateTime.ParseExact(settingServer.lastTimeUpdateComputer, dateFormat, provider);

                    DateTime lastTimeUpdateClassLocal = DateTime.ParseExact(settingLocal.lastTimeUpdateComputer, dateFormat, provider);
                    DateTime lastTimeUpdateClassServer = DateTime.ParseExact(settingServer.lastTimeUpdateComputer, dateFormat, provider);
;

                await _roomBLL.GetRoomsByName("F72");
                if (lastTimeUpdateStudentLocal != lastTimeUpdateStudentServer)
                {
                    await SyncStudentData(lastTimeUpdateStudentLocal, lastTimeUpdateStudentServer.AddDays(1), settingServer);
                }
                if (lastTimeUpdateComputerLocal != lastTimeUpdateComputerServer)
                {
                    await SyncComputerData(lastTimeUpdateComputerLocal, lastTimeUpdateComputerServer.AddDays(1), settingServer);
                }
                if (lastTimeUpdateClassLocal != lastTimeUpdateClassServer)
                {
                    await SyncClassData(lastTimeUpdateClassLocal, lastTimeUpdateClassServer.AddDays(1), settingServer);
                }

                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error migrating computer and student data: {ex}");
            return false;
        }
    }


    private async Task SyncStudentData(DateTime lastTimeUpdateStudentLocal, DateTime lastTimeUpdateStudentServer, Setting settingServer)
    {
        var studentsToSync = await _studentBLL.GetStudentsByDateRange(lastTimeUpdateStudentLocal, lastTimeUpdateStudentServer);
        if (studentsToSync != null)
        {
            await _studentBLL.InsertStudentLocal(studentsToSync);
            await _settingBLL.UpdateLastTimeUpdateStudent(settingServer.lastTimeUpdateStudent);
        }
    }

    private async Task SyncComputerData(DateTime lastTimeUpdateComputerLocal, DateTime lastTimeUpdateComputerServer, Setting settingServer)
    {
        var computersToSync = await _computerBLL.GetComputerByDateRange(lastTimeUpdateComputerLocal, lastTimeUpdateComputerServer);
        if (computersToSync != null)
        {
            await _computerBLL.InsertOrUpdateComputers(computersToSync);
            await _settingBLL.UpdateLastTimeUpdateComputer(settingServer.lastTimeUpdateComputer);
        }
    }

    private async Task SyncClassData(DateTime lastTimeUpdateClassLocal, DateTime lastTimeUpdateClassServer, Setting settingServer)
    {
        var ClasssToSync = await _classBLL.GetClassByDateRange(lastTimeUpdateClassLocal, lastTimeUpdateClassServer);
        if (ClasssToSync != null)
        {
            foreach (var classe in ClasssToSync)
            {
                await _classBLL.SaveLocalData(classe);
                await _classStudentBLL.GetClassStudentsByIDAPI(classe.ClassID);
            }
            await _settingBLL.UpdateLastTimeUpdateClass(settingServer.lastTimeUpdateClass);
        }
    }

    private string GeneratePositiveID(string negativeID)
    {
        return negativeID.Replace("-", ""); // Adjust logic as needed
    }
}
