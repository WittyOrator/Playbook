using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Linq;
using System.Data.OleDb;
using System.IO;

namespace Webb.Playbook.Data
{
    public class VictoryData
    {
        private string userFolder = string.Empty;
        public string UserFolder
        {
            get { return userFolder; }
            set 
            { 
                userFolder = value;

                GetDataTable();
            }
        }

        private DataTable fieldsTable = new DataTable();
        public DataTable FieldsTable
        {
            get { return fieldsTable; }
        }

        public VictoryData()
        {

        }

        public List<String> GetFields()
        {
            var list = from row in FieldsTable.AsEnumerable() select row.Field<string>("FieldName");

            return list.Distinct().ToList<String>();
        }

        public List<String> GetValues(string strField)
        {
            var List = from row in FieldsTable.AsEnumerable()
                       orderby row.Field<string>("Value")
                       where row.Field<string>("FieldName") == strField
                       select row.Field<string>("Value");

            return List.ToList<String>();
        }

        private bool GetDataTable()
        {
            if (Directory.Exists(UserFolder))
            {
                string strMdbFile = UserFolder + @"\Victory.mdb";

                if (File.Exists(strMdbFile))
                {
                    FieldsTable.Clear();
                    string strConn = @"Provider=Microsoft.Jet.OleDb.4.0;Data Source='" + UserFolder + @"\Victory.mdb'";
                    OleDbConnection conn = new OleDbConnection(strConn);
                    conn.Open();

                    OleDbCommand cmd = new OleDbCommand("SELECT FieldID, DefaultFields.FieldName, Value FROM DefaultFields LEFT JOIN FieldsValue ON DefaultFields.FieldName = FieldsValue.FieldName", conn);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    FieldsTable.Load(reader);

                    reader.Close();
                    conn.Close();

                    return true;
                }
            }

            return false;
        }
    }
}
