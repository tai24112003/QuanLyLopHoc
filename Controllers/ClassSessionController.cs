using System.Threading.Tasks;
using System;

public class ClassSessionController
{
    private readonly LocalDataHandler _localDataHandler;
    private readonly ClassSessionBLL _classSessionBLL;

    public ClassSessionController(LocalDataHandler localDataHandler, ClassSessionBLL classSessionBLL)
    {
        _localDataHandler = localDataHandler ?? throw new ArgumentNullException(nameof(localDataHandler));
        _classSessionBLL = classSessionBLL ?? throw new ArgumentNullException(nameof(classSessionBLL));
    }

    public async Task StartNewClassSession(ClassSession classSession)
    {
        // Check and save local data before starting new session
        bool isLocalDataSaved = await _localDataHandler.SaveLocalDataToDatabase();

        if (isLocalDataSaved)
        {
            Console.WriteLine("Local data saved successfully.");
        }
        else
        {
            Console.WriteLine("Failed to save local data.");
        }

        // Start new class session
        try
        {
            // Insert the new class session and get the session ID
            var insertedClassSession = await _classSessionBLL.InsertClassSession(classSession);

            // Save the session ID locally
            _localDataHandler.SaveLocalSessionId(insertedClassSession.SessionID);

            Console.WriteLine("New class session started successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error starting new class session: " + ex.Message);
            // Optionally, handle the error by saving the session locally
            _localDataHandler.SaveLocalClassSession(classSession);
        }
    }
}
