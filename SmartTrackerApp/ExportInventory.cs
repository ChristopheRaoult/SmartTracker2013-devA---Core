using System;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using DataClass;
using smartTracker.Properties;

namespace smartTracker
{
    public class ExportInventory
    {
        public SqlConnection GlobalCon;
        public string GlobalConnectionString;
        public ConnectionState ConState;
        public string TableName;
        public ExportInventory(string globalConnectionString , string tableName)
        {
            GlobalConnectionString = globalConnectionString;
            TableName = tableName;
            ConState = ConnectionState.Closed;         
        }

        public DateTime GetLastScanExported(string serialRfid)
        {
            DateTime lastExportDate = DateTime.MaxValue;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader rd = null;
            try
            {
                if (GlobalCon == null) return lastExportDate;
                if (GlobalCon.State != ConnectionState.Open) OpenDb();

                string sqlQuery = null;
                sqlQuery += "SELECT TOP 1 EventDate FROM " + TableName + " ";
                sqlQuery += "WHERE SerialRFID='" + serialRfid + "' ";
                sqlQuery += "ORDER BY EventDate DESC";

                cmd.Connection = GlobalCon;
                cmd.CommandText = sqlQuery;
                cmd.CommandType = CommandType.Text;
                rd = cmd.ExecuteReader();

                if (rd.HasRows)
                {
                    rd.Read();
                    if (rd[ResStrings.str_EventDate] != DBNull.Value)
                        DateTime.TryParse(rd[ResStrings.str_EventDate].ToString(), out lastExportDate);
                }
                else
                    lastExportDate = DateTime.UtcNow.AddDays(-1.0); // for first time.
            }
            catch(Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            finally
            {
                if (rd != null) rd.Close();
                cmd.Dispose();
            }
            return lastExportDate;
        }

        public bool StoreScan(InventoryData scanToStore)
        {
            bool ret = false;
            SqlCommand cmd = new SqlCommand();
            try
            {
                DateTime gmtDate = scanToStore.eventDate.ToUniversalTime();
                string sqlQuery = null;
                sqlQuery += "INSERT INTO " + TableName + "(SerialRFID,bUserScan,AccessUsed,FirstName,LastName,EventDate,NbTagAdded,NbTagAll,NbTagPresent,NbTagRemoved,listTagAdded,listTagAll,listTagPresent,listTagRemoved) VALUES('";
                sqlQuery += scanToStore.serialNumberDevice + "',";
                sqlQuery += Convert.ToInt32(scanToStore.bUserScan) + ",";
                sqlQuery += (int)(scanToStore.accessType) + ",'";
                sqlQuery += scanToStore.userFirstName + "','";
                sqlQuery += scanToStore.userLastName + "','";
                sqlQuery += gmtDate.ToString("yyyy-MM-dd HH:mm:ss" ,System.Globalization.CultureInfo.InvariantCulture) + "',";
                sqlQuery += scanToStore.listTagAdded.Count + ",";
                sqlQuery += scanToStore.listTagAll.Count + ",";
                sqlQuery += scanToStore.listTagPresent.Count + ",";
                sqlQuery += scanToStore.listTagRemoved.Count + ",'";
                sqlQuery += ArrayListToString(scanToStore.listTagAdded) + "','";
                sqlQuery += ArrayListToString(scanToStore.listTagAll) + "','";
                sqlQuery += ArrayListToString(scanToStore.listTagPresent) + "','";
                sqlQuery += ArrayListToString(scanToStore.listTagRemoved) + "');";
               
                cmd.Connection = GlobalCon;
                cmd.CommandText = sqlQuery;
                cmd.CommandType = CommandType.Text;
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected > 0) ret = true;
            }
            catch(Exception exp)
            {
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            finally
            {               
                cmd.Dispose();
            }
            return ret;
        }


        private string ArrayListToString(ArrayList list)
        {
            string listArray = string.Empty;
                      
            foreach (string uid in list)
            {
                listArray += uid + ";";
            }
            if (list.Count > 0)
            listArray = listArray.Substring(0,listArray.Length - 1);
           
            return listArray;
        }

        public bool IsTableExist()
        {
            bool ret = false;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader rd = null;
            try
            {
                if (GlobalCon == null) return false;
                if (GlobalCon.State != ConnectionState.Open) OpenDb();

                string sqlQuery = null;
                sqlQuery += "SELECT * FROM sysobjects ";
                sqlQuery += "WHERE name='" + TableName + "' AND Xtype='U'";
               
                cmd.Connection = GlobalCon;
                cmd.CommandText = sqlQuery;
                cmd.CommandType = CommandType.Text;

                rd = cmd.ExecuteReader();
                ret = rd.HasRows;

            }
            catch (Exception exp)
            {
                // On affiche l'erreur.
                ErrorMessage.ExceptionMessageBox.Show(exp);
            }
            finally
            {
                if (rd != null) rd.Close();
                cmd.Dispose();
            }
            return ret;
        }

        public void CloseDb()
        {
            if (GlobalCon == null) return;
            if (GlobalCon.State != ConnectionState.Closed)
            {
                GlobalCon.Close();
                ConState = GlobalCon.State;
                GlobalCon = null;
            }
        }
        public bool OpenDb()
        {
            GlobalCon = new SqlConnection();
            GlobalCon.ConnectionString = GlobalConnectionString;
            GlobalCon.Open();
            ConState = GlobalCon.State;
            if (GlobalCon.State == ConnectionState.Open) return true;
            return false;
        }
        public bool IsOpen()
        {
            if (GlobalCon == null) return false;
            if (GlobalCon.State == ConnectionState.Open) return true;
            return false;
        }
    }
}
