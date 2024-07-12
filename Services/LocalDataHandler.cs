using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading.Tasks;

public class LocalDataHandler
{
    private readonly ClassSessionBLL _classSessionBLL;
    private readonly SessionComputerBLL _sessionComputerBLL;
    private readonly string localClassSessionsFilePath = "localClassSessions.json";
    private readonly string localSessionComputersFilePath = "localSessionComputers.json";

    public LocalDataHandler(ClassSessionBLL classSessionBLL, SessionComputerBLL sessionComputerBLL)
    {
        _classSessionBLL = classSessionBLL ?? throw new ArgumentNullException(nameof(classSessionBLL));
        _sessionComputerBLL = sessionComputerBLL ?? throw new ArgumentNullException(nameof(sessionComputerBLL));
    }

    public async Task<bool> SaveLocalDataToDatabase()
    {
        try
        {
            // Save local class sessions
            var classSessions = LoadLocalClassSessions();
            var sessionComputers = LoadLocalSessionComputers();
            foreach (var entry in classSessions)
            {
                string keyString = entry.Key.ToString();

                // Kiểm tra ký tự đầu của key có phải dấu '-' hay không
                if (keyString.StartsWith("-"))
                {
                    // Thực hiện thao tác chèn (insert)
                    var classSession = await _classSessionBLL.InsertClassSession(entry.Value);

                    // Tìm kiếm trong sessionComputers có key tương tự
                    foreach (var sessionEntry in sessionComputers)
                    {
                        string sessionKeyString = sessionEntry.Key.ToString();

                        if (sessionKeyString.StartsWith("-") && sessionKeyString == keyString)
                        {
                            // Thực hiện thao tác cần thiết khi tìm thấy key tương tự
                            //await _sessionComputerBLL.InsertSessionComputers(sessionEntry.Key, sessionEntry.Value);
                            break;
                        }
                    }
                }
            }

            foreach (var kvp in sessionComputers)
            {
                string keyString = kvp.Key.ToString();
                //if (!keyString.StartsWith("-"))

                    //await _sessionComputerBLL.InsertSessionComputers(kvp.Key, kvp.Value);
            }
            // Delete local files after successful save
            DeleteLocalFiles();

            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving local data to database: " + ex.Message);
            return false;
        }
    }

    public void SaveLocalSessionId(int sessionId)
    {
        File.WriteAllText("localSessionId.txt", sessionId.ToString());
    }

    public void SaveLocalClassSession(int sessionID, ClassSession classSession)
    {
        // Đọc dữ liệu từ tệp và chuyển đổi thành từ điển
        Dictionary<int, ClassSession> classSessions;
        if (File.Exists(localClassSessionsFilePath))
        {
            var jsonData = File.ReadAllText(localClassSessionsFilePath);
            classSessions = JsonConvert.DeserializeObject<Dictionary<int, ClassSession>>(jsonData) ?? new Dictionary<int, ClassSession>();
        }
        else
        {
            classSessions = new Dictionary<int, ClassSession>();
        }

        // Thêm hoặc cập nhật phần tử trong từ điển
        classSessions[sessionID] = classSession;

        // Ghi lại từ điển vào tệp
        File.WriteAllText(localClassSessionsFilePath, JsonConvert.SerializeObject(classSessions));
    }

    private Dictionary<int, ClassSession> LoadLocalClassSessions()
    {
        if (File.Exists(localClassSessionsFilePath))
        {
            var localData = File.ReadAllText(localClassSessionsFilePath);
            return JsonConvert.DeserializeObject<Dictionary<int, ClassSession>>(localData) ?? new Dictionary<int, ClassSession>();
        }

        return new Dictionary<int, ClassSession>();
    }


    private Dictionary<int, List<SessionComputer>> LoadLocalSessionComputers()
    {
        if (File.Exists(localSessionComputersFilePath))
        {
            var localData = File.ReadAllText(localSessionComputersFilePath);
            return JsonConvert.DeserializeObject<Dictionary<int, List<SessionComputer>>>(localData) ?? new Dictionary<int, List<SessionComputer>>();
        }

        return new Dictionary<int, List<SessionComputer>>();
    }

    private void DeleteLocalFiles()
    {
        if (File.Exists(localClassSessionsFilePath))
        {
            File.Delete(localClassSessionsFilePath);
        }

        if (File.Exists(localSessionComputersFilePath))
        {
            File.Delete(localSessionComputersFilePath);
        }

        if (File.Exists("localSessionId.txt"))
        {
            File.Delete("localSessionId.txt");
        }
    }
}
