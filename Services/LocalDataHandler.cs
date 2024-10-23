using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;

public class LocalDataHandler
{
    private readonly ClassSessionBLL _classSessionBLL;
    private readonly SessionComputerBLL _sessionComputerBLL;
    private readonly ClassBLL _classBLL;
    private readonly StudentBLL _studentBLL;
    private readonly ClassStudentBLL _classStudentBLL;

    public LocalDataHandler(ClassSessionBLL classSessionBLL, SessionComputerBLL sessionComputerBLL, ClassBLL classBLL, StudentBLL studentBLL, ClassStudentBLL classStudentBLL)
    {
        _classSessionBLL = classSessionBLL ?? throw new ArgumentNullException(nameof(classSessionBLL));
        _sessionComputerBLL = sessionComputerBLL ?? throw new ArgumentNullException(nameof(sessionComputerBLL));
        _classBLL = classBLL ?? throw new ArgumentNullException(nameof(classBLL));
        _studentBLL = studentBLL ?? throw new ArgumentNullException(nameof(studentBLL));
        _classStudentBLL = classStudentBLL ?? throw new ArgumentNullException(nameof(classStudentBLL));
    }

    public async Task MigrateData()
    {
        try
        {
            // Bước 1: Lấy danh sách lớp có ID âm
            var negativeClasses = _classBLL.LoadNegativeIDClasses();
            if (negativeClasses != null)
                foreach (var classItem in negativeClasses)
                {
                    // Bước 2: Lưu ID lớp âm vào biến 'a'
                    int classID_A = classItem.ClassID;

                    // Bước 3: Lấy danh sách class_student có ClassID là 'a'
                    var classStudents = await _classStudentBLL.GetClassStudentsByClassID(classID_A);

                    // Bước 4: Lấy danh sách sinh viên có ID âm
                    var negativeStudents = _studentBLL.LoadNegativeIDStudentes();

                    // Sau khi đã có toàn bộ thông tin cần thiết (lớp, sinh viên, class_student), ta bắt đầu xử lý xóa và thêm mới

                    // Bước 5: Thêm lớp vào server và lấy ID lớp mới trả về, lưu vào biến 'b'
                    int classID_B = (await _classBLL.InsertClass(classItem)).ClassID;

                    // Bước 6: Xóa class_student có ID lớp là 'a' ở local
                    await _classStudentBLL.DeleteClassStudentsByClassID(classID_A);

                    // Bước 7: Xử lý sinh viên có ID âm
                    foreach (var student in negativeStudents)
                    {
                        // Xóa class_student liên quan đến sinh viên trước khi xóa sinh viên để tránh vi phạm khóa ngoại
                        await _classStudentBLL.DeleteClassStudentsByStudentID(student.StudentID);

                        // Xóa sinh viên có ID âm
                        await _studentBLL.DeleteStudent(student.StudentID);

                        // Sinh ID không âm mới cho sinh viên
                        student.StudentID = GeneratePositiveID(student.StudentID);
                    }

                    // Bước 8: Thêm sinh viên mới vào server với ID không âm
                    await _studentBLL.InsertStudent(negativeStudents);

                    // Bước 9: Thêm sinh viên mới vào local với ID không âm
                    _studentBLL.InsertStudentLocal(negativeStudents);

                    // Bước 10: Cập nhật ID lớp trong danh sách class_student thành ID lớp mới 'b'
                    foreach (var classStudent in classStudents)
                    {
                        classStudent.ClassID = classID_B;
                        classStudent.StudentID = GeneratePositiveID(classStudent.StudentID);
                    }

                    // Bước 11: Thêm danh sách class_student với ID lớp là 'b' vào server
                    await _classStudentBLL.InsertClassStudent(classStudents);

                    // Bước 12: Cập nhật class_session có classID là 'a' thành classID là 'b'
                    await _classSessionBLL.UpdateClassSessionClassID(classID_A, classID_B);

                }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }



    // Hàm tạo ID không âm mới (bạn có thể thay đổi logic này theo ý muốn)
    private string GeneratePositiveID(string negativeID)
    {
        return negativeID.Replace("-", ""); // Ví dụ: chỉ thay thế dấu '-' bằng chuỗi rỗng
    }
}