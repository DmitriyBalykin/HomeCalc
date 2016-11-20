using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbConnectionWrappers
{
    public class StorageConnection : IDisposable
    {
        public SQLiteConnection Connection;
        public StorageConnection(string connectionString)
        {
            try
            {
                Connection = new SQLiteConnection(connectionString);
                Connection.StateChange += Connection_StateChange;

                Connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Storage connection failed: {0}", ex.Message);
            }
            
        }

        void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            State = e.CurrentState;
        }
        public ConnectionState State { get; set; }

        public void Dispose()
        {
            try
            {
                if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
                {
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during connection closing {0}", ex.Message);
            }
            
        }
    }
}
