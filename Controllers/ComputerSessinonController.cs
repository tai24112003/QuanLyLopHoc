using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class ComputerSessionController{
    private readonly SessionComputerBLL _computerSessionBLL;

    public ComputerSessionController(LocalDataHandler localDataHandler, SessionComputerBLL classSessionBLL)
    {
        _computerSessionBLL = classSessionBLL ?? throw new ArgumentNullException(nameof(classSessionBLL));
    }

    public async Task UpdateSessionComputer(int sesssionID ,List<SessionComputer> classSession)
    {
      
        // Start new class session
        try
        {
            // Insert the new class session and get the session ID
            await _computerSessionBLL.InsertSessionComputers(sesssionID,classSession);

            // Save the session ID locally

            Console.WriteLine("New class session started successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error starting new class session: " + ex.Message);
            // Optionally, handle the error by saving the session locally
            _computerSessionBLL.SaveLocalSessionComputers(sesssionID,classSession);
        }
    }
}
