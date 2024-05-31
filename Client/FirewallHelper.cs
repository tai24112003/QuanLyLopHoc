﻿using System;
using System.Diagnostics;
using System.Security.Principal;
namespace testUdpTcp
{

    public class FirewallHelper
    {
        public static void AddFirewallRule()
        {
            if (IsUserAdministrator())
            {
                // Đường dẫn tới tập lệnh .bat
                string scriptPath = @"D:\allow_firewall.bat";

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = $"/c {scriptPath}";
                psi.Verb = "runas"; // Yêu cầu quyền admin
                psi.UseShellExecute = true;

                try
                {
                    Process.Start(psi).WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add firewall rule: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("This operation requires administrator privileges.");
            }
        }

        private static bool IsUserAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
