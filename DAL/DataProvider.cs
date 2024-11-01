using System;
using System.Data;
using System.Data.OleDb;

public static class DataProvider
{
    private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\qlpm.accdb;Persist Security Info=True;";

    public static OleDbConnection ConnectAccess()
    {
        try
        {
            OleDbConnection con = new OleDbConnection(connectionString);
            con.Open();
            return con;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public static bool RunNonQuery(string query, OleDbParameter[] parameters)
    {
        using (OleDbConnection con = ConnectAccess())
        {
            if (con == null)
                return false;

            using (OleDbCommand cmd = new OleDbCommand(query, con))
            {
                try
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    Console.WriteLine(cmd.CommandText);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing non-query: " + ex.Message);
                    return false;
                }
            }
        }
    }

    public static object RunScalar(string query, OleDbParameter[] parameters)
    {
        using (OleDbConnection con = ConnectAccess())
        {
            if (con == null)
                return null;

            using (OleDbCommand cmd = new OleDbCommand(query, con))
            {
                try
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing scalar query: " + ex.Message);
                    return null;
                }
            }
        }
    }

    public static DataTable GetDataTable(string query, OleDbParameter[] parameters)
    {
        using (OleDbConnection con = ConnectAccess())
        {
            if (con == null)
                return null;

            using (OleDbCommand cmd = new OleDbCommand(query, con))
            {
                try
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing query and returning data table: " + ex.Message);
                    return null;
                }
            }
        }
    }
}
