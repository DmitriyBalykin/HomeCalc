using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbConnectionWrappers
{
    public class StorageConnection : IDisposable
    {
        public SQLiteConnection Connection;
        public StorageConnection(string connectionString)
        {
            Connection = new SQLiteConnection(connectionString);
            Connection.StateChange += Connection_StateChange;
            RecentlyInitiated = false;

            Connection.Open();
        }

        void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            State = e.CurrentState;
        }
        public bool RecentlyInitiated { get; set; }
        public ConnectionState State { get; set; }

        public void Dispose()
        {
            if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
            }
        }
    }
}
