using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLayer
{
    public class OracleConnect
    {
        OracleConnection _con;
        OracleCommand _cmd;
        public OracleConnect(string cinnectionString)
        {
            _con = new OracleConnection();
            _con.ConnectionString = cinnectionString;
        }
        public void OpenConnect()
        {
            _con.Open();
        }

        public void CloseConnect()
        {
            _con.Close();
            _con.Dispose();
        }

        public void ExecCommand(string query)
        {
            _cmd = new OracleCommand(query, _con);
            _cmd.ExecuteNonQuery();            
            _cmd.Dispose();
        }

        public OracleDataReader GetReader(string query)
        {
            _cmd = new OracleCommand(query, _con);
            OracleDataReader reader = _cmd.ExecuteReader();
            _cmd.Dispose();
            return reader;
        }

    }
}
