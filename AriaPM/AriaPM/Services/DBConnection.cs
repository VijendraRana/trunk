using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace AriaPM.Services
{
    public class DBConnection
    {
        public string ConnectionString { get; set; }

        public DBConnection()
        {
            ConnectionString = "server=aria-pm-db.c8gxntt3fgqr.ap-southeast-2.rds.amazonaws.com;port=3306;database=AriaPM;user=admin;password=!Password01;pooling=true";
        }

        public int ExecuteNonQuery(string query, ObservableCollection<Params> paramList)
        {
            int result = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        if (paramList != null)
                            foreach (var param in paramList)
                            {
                                cmd.Parameters.AddWithValue(param.Name, param.Value);
                            }

                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public object ExecuteScalar(string query, ObservableCollection<Params> paramList)
        {
            object result = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        if (paramList != null)
                            foreach (var param in paramList)
                            {
                                cmd.Parameters.AddWithValue(param.Name, param.Value);
                            }

                        result = cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }

        public object GetQueryResult(string query, ObservableCollection<Params> paramList)
        {
            object result = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        MySqlCommand cmd = new MySqlCommand(query, conn);

                        if (paramList != null)
                            foreach (var param in paramList)
                            {
                                cmd.Parameters.AddWithValue(param.Name, param.Value);
                            }

                        MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        sda.Fill(ds);
                        result = ds;
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
    }

    public class Params
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
