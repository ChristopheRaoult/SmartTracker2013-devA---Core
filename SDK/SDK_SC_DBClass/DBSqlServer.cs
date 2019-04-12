using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb; //For import xls
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using DataClass;
using System.Text.RegularExpressions;

namespace DBClassSqlServer
{
    public class DBSqlServer : DBClass.IDBInterface
    {
       
        public SqlConnection globalCon;
        public string globalConnectionString;
        public ConnectionState conState;
        SqlTransaction trans;

        public DBSqlServer(string globalConnectionString)
        {
            this.globalConnectionString = globalConnectionString;
            conState = ConnectionState.Closed;
        }
        ~DBSqlServer()
        {
            CloseDB();
        }
        #region dbfn
            public void CloseDB()
            {
                if (globalCon == null) return;
                if (globalCon.State != ConnectionState.Closed)
                {
                    globalCon.Close();
                    conState = globalCon.State;
                    globalCon = null;
                }
            }
            public bool OpenDB()
            {         
                globalCon = new SqlConnection();
                globalCon.ConnectionString = globalConnectionString;
                globalCon.Open();
                conState = globalCon.State;
                if (globalCon.State == ConnectionState.Open) return true;
                else return false;
            }

            private void ExecuteUpdateScript()
            {
                int currentVersion = getVersion();
                if (currentVersion < SqlUpdate.Count)
                {
                    for (int i = (currentVersion + 1); i < SqlUpdate.Count+1; ++i)
                        setUpdate(i);
                }
            }

            public void Connect()
            {
                if (globalCon != null && globalCon.State == ConnectionState.Open) return;
                globalCon = new SqlConnection();
                globalCon.ConnectionString = globalConnectionString;
                globalCon.Open();
                
                // check if updates are available and execute corresponding script
                ExecuteUpdateScript();
            }

            public void Close()
            {
                if (globalCon == null || globalCon.State == ConnectionState.Closed) return;
                globalCon.Close();
            }

            public bool isOpen()
            {
                if (globalCon == null) return false;
                if (globalCon.State == System.Data.ConnectionState.Open) return true;
                else return false;
            }
            public void startTranscation()
            {               
                if (globalCon == null) return;
                if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                trans = globalCon.BeginTransaction();                
            }
            public void endTranscation()
            {
                  trans.Commit();               
            }
        #endregion
        #region update
            public bool isTableExist(string table_name)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM sysobjects ";
                    sqlQuery += "WHERE name='" + table_name + "' AND Xtype='U'";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

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
                    if (rd != null)  rd.Close();
                    if (cmd != null ) cmd.Dispose();
                }
                return ret;
            }
            public int getVersion()
            {
                int ret = -1;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT Value FROM tbo_Db_Info WHERE Criteria='Version'";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        rd.Read();
                        ret = int.Parse(rd["Value"].ToString().Trim());
                    }                  
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool setVersion(int version)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Db_Info WHERE Criteria='Version'";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();

                    sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_Db_Info(Criteria,Value) VALUES ('Version'," + version + ")";
                    cmd.CommandText = sqlQuery;  
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    ret = true;
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;

            }
            public bool setUpdate(int updateNumber)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    string sqlQuery = null;
                    sqlQuery += sqlUpdate[updateNumber];
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                    setVersion(updateNumber);
                    ret = true;
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;

            }
            public bool isTableVersionExist()
            {
                return isTableExist("tbo_Db_Info");
            }
        #endregion
        #region Column
            public ArrayList GetColumnToExport()
            {
                ArrayList ColumnToExport = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Column_Info WHERE ColumnToExport=1";
                  
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        ColumnToExport = new ArrayList();
                        while (rd.Read())
                        {
                            string colName = rd["ColumnName"].ToString().Trim();
                            ColumnToExport.Add(colName);
                        }                        
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ColumnToExport;
            }
            public Hashtable GetColumnInfo()
            {
                Hashtable ColumnInfo = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Column_Info ";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        ColumnInfo = new Hashtable();
                        while (rd.Read())
                        {
                            int Index = int.Parse(rd["IdColumn"].ToString().Trim());
                            string colName = rd["ColumnName"].ToString().Trim();
                            ColumnInfo.Add(Index, colName);
                        }                       
                    } 
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ColumnInfo;
            }
            public dtColumnInfo[] GetdtColumnInfo()
            {
                dtColumnInfo[] colArray = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Column_Info ";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    ArrayList listtmp = new ArrayList();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            dtColumnInfo colInfo = new dtColumnInfo();
                            colInfo.colIndex = int.Parse(rd["IdColumn"].ToString().Trim());
                            colInfo.colName = rd["ColumnName"].ToString().Trim();

                            switch (rd["ColumnType"].ToString().Trim().ToUpper())
                            {
                                case "STRING": colInfo.colType = typeof(string); break;
                                case "INT": colInfo.colType = typeof(int); break;
                                case "UINT64": colInfo.colType = typeof(UInt64); break;
                                case "DOUBLE": colInfo.colType = typeof(double); break;
                                case "DATETIME": colInfo.colType = typeof(DateTime); break;
                                case "IMAGE": colInfo.colType = typeof(byte[]); break;

                                default: colInfo.colType = typeof(string); break;
                            }
                            colInfo.colDoSum = int.Parse(rd["DoSum"].ToString().Trim());
                            listtmp.Add(colInfo);
                        }

                        if (listtmp.Count > 0)
                        {
                            colArray = new dtColumnInfo[listtmp.Count];
                            for (int i = 0; i < listtmp.Count; i++)
                                colArray[i] = (dtColumnInfo)listtmp[i];
                        }                       
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return colArray;
            }
            public Hashtable GetColumnCSVInfo()
            {
                Hashtable ColumnCSVInfo = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Column_Info ";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        ColumnCSVInfo = new Hashtable();
                        while (rd.Read())
                        {
                            int Index = int.Parse(rd["IdColumn"].ToString().Trim());
                            string colName = rd["ColumnCSVName"].ToString().Trim();
                            ColumnCSVInfo.Add(Index, colName);
                        }                      
                    } 
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ColumnCSVInfo;
            }
            public Object GetColumn()
            {
                SqlDataAdapter da = null;               
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    {
                        string sqlQuery = null;
                        sqlQuery += "SELECT * FROM tbo_Column_Info;";
                        da = new SqlDataAdapter(sqlQuery, globalCon);
                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                return da;
            }
            #endregion
        #region Product
            private object locker = new object();
            public DataTable RecoverAllProduct()
            {
                DataTable dtGet = null;               
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    
                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Product_Def;";
                    SqlDataAdapter da = new SqlDataAdapter(sqlQuery, globalCon);
                    dtGet = new DataTable();
                    da.Fill(dtGet);
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                return dtGet;
            }
            public string getProductTagID(string LotID)
            {
                string retString = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                lock (locker)
                {
                    try
                    {
                        if (globalCon == null) return null;
                        if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                        string sqlQuery = null;
                        sqlQuery += "SELECT * FROM tbo_Product_Def ";
                        sqlQuery += "WHERE ProductRef='" + LotID + "';";
                       
                        cmd.Transaction = trans;
                        cmd.Connection = globalCon;
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            rd.Read();
                            retString = rd["TagUID"].ToString().Trim();                          
                        }                       
                    }
                    catch (Exception exp)
                    {
                        // On affiche l'erreur.
                        ErrorMessage.ExceptionMessageBox.Show(exp);
                    }
                    finally
                    {
                        if (rd != null) rd.Close();
                        if (cmd != null) cmd.Dispose();
                    }
                }
                return retString;
            }
            public string[] RecoverProductInfo(string tagUID)
            {
                string[] retString = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                lock (locker)
                {
                    try
                    {
                        if (globalCon == null) return null;
                        if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                        string sqlQuery = null;
                        sqlQuery += "SELECT * FROM tbo_Product_Def ";
                        sqlQuery += "WHERE TagUID=@tagUID;";
                        
                        cmd.Transaction = trans;
                        cmd.Connection = globalCon;
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = tagUID;

                        rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            rd.Read();


                            retString = new string[DataClass.VarGlobal.Nb_Max_Column_Product + 2];
                            retString[0] = rd["TagUID"].ToString().Trim();
                            retString[1] = rd["ProductRef"].ToString().Trim();

                            for (int loop = 1; loop <= DataClass.VarGlobal.Nb_Max_Column_Product; loop++)
                            {
                                string productRow = string.Format("ProductInfo{0}", loop);
                                retString[loop + 1] = rd[productRow].ToString().Trim();
                            }                            
                        }                       
                    }
                    catch (Exception exp)
                    {
                        // On affiche l'erreur.
                        ErrorMessage.ExceptionMessageBox.Show(exp);
                    }
                    finally
                    {
                        if (rd != null) rd.Close();
                        if (cmd != null) cmd.Dispose();
                    }
                }

                return retString;
            }
            public ArrayList RecoverProductHistory(string tagUID)
            {
                ArrayList prodHistory = null;
                string[] retString = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                lock (locker)
                {
                    try
                    {
                        if (globalCon == null) return null;
                        if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                        string sqlQuery = null;
                        sqlQuery += "SELECT * FROM tbo_Product_History ";
                        sqlQuery += "WHERE TagUID= @tagUID ";
                        sqlQuery += "ORDER BY SortedEnterDate DESC;";
                        
                        cmd.Transaction = trans;
                        cmd.Connection = globalCon;
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = tagUID;

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            prodHistory = new ArrayList();
                            while (rd.Read())
                            {
                                retString = new string[DataClass.VarGlobal.Nb_Max_Column_Product + 4];
                                retString[0] = rd["EnterDate"].ToString().Trim();
                                retString[1] = rd["SortedEnterDate"].ToString().Trim();
                                retString[2] = rd["TagUID"].ToString().Trim();
                                retString[3] = rd["ProductRef"].ToString().Trim();

                                for (int loop = 1; loop <= DataClass.VarGlobal.Nb_Max_Column_Product; loop++)
                                {
                                    string productRow = string.Format("ProductInfo{0}", loop);
                                    retString[loop + 3] = rd[productRow].ToString().Trim();
                                }
                                prodHistory.Add(retString);
                            }                          
                        }                     
                    }
                    catch (Exception exp)
                    {
                        // On affiche l'erreur.
                        ErrorMessage.ExceptionMessageBox.Show(exp);
                    }
                    finally
                    {
                        if (rd != null) rd.Close();
                        if (cmd != null) cmd.Dispose();
                    }
                }

                return prodHistory;
            }
            public bool IsProductExist(string tagUID)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Product_Def ";
                    sqlQuery += "WHERE TagUID=@tagUID;";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = tagUID;

                    rd = cmd.ExecuteReader();
                    ret = rd.HasRows;
                    rd.Close();
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool StoreProduct(ProductClassTemplate ProductToStore)
            {

                bool ret = false;

                /*System.Text.RegularExpressions.Regex myRegex = new Regex(@"[0-7]{10}");
                if (!myRegex.IsMatch(ProductToStore.tagUID)) return false;*/


                if (IsProductExist(ProductToStore.tagUID))
                {
                    ret = UpdateProduct(ProductToStore);
                }
                else
                {
                    ret = AddProduct(ProductToStore);
                }
                AddProductHistory(ProductToStore);
                return ret;
            }
            private bool AddProductHistory(ProductClassTemplate ProductToStore)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    DateTime nowDate = DateTime.Now;
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;

                    sqlQuery += "INSERT INTO tbo_Product_History(EnterDate,SortedEnterDate,TagUID,ProductRef";
                    for (int loop = 1; loop <= VarGlobal.Nb_Max_Column_Product; loop++)
                    {
                        sqlQuery += ",ProductInfo" + loop.ToString();
                    }
                    sqlQuery += ") VALUES('";
                    sqlQuery += nowDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                    sqlQuery += nowDate.ToString("s") + "',";
                    sqlQuery += "@tagUID,'";
                    sqlQuery += ProductToStore.reference;
                    for (int loop = 0; loop < VarGlobal.Nb_Max_Column_Product; loop++)
                    {

                        sqlQuery += "','" + ProductToStore.productInfo[loop];
                    }
                    sqlQuery += "');";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = ProductToStore.tagUID; 

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            private bool AddProduct(ProductClassTemplate ProductToStore)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;

                    sqlQuery += "INSERT INTO tbo_Product_Def(TagUID,ProductRef";
                    for (int loop = 1; loop <= VarGlobal.Nb_Max_Column_Product; loop++)
                    {
                        sqlQuery += ",ProductInfo" + loop.ToString();
                    }
                    sqlQuery += ") VALUES(";
                    sqlQuery += "@tagUID,'";
                    sqlQuery += ProductToStore.reference;
                    for (int loop = 0; loop < VarGlobal.Nb_Max_Column_Product; loop++)
                    {

                        sqlQuery += "','" + ProductToStore.productInfo[loop];
                    }
                    sqlQuery += "');";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = ProductToStore.tagUID;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            private bool UpdateProduct(ProductClassTemplate ProductToStore)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    Hashtable ColumnInfo = GetColumnInfo();
                    string sqlQuery = null;
                    sqlQuery += "UPDATE tbo_Product_Def ";
                    sqlQuery += "SET ProductRef='" + ProductToStore.reference + "'";
                    for (int i = 1; i < ColumnInfo.Count - 1; i++)
                    {
                        sqlQuery += ",ProductInfo" + i.ToString() + "='" + ProductToStore.productInfo[i - 1] + "'";
                    }
                    //sqlQuery += "ProductInfo1='" + ProductToStore.description + "' ";
                    sqlQuery += " WHERE TagUID=@tagUID;";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = ProductToStore.tagUID;
                   
                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool DeleteProduct(string tagUID)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    if (IsProductExist(tagUID))
                    {

                        string sqlQuery = null;
                        sqlQuery += "DELETE FROM tbo_Product_Def ";
                        sqlQuery += "WHERE TagUID=@tagUID;";
                        
                        cmd.Transaction = trans;
                        cmd.Connection = globalCon;
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = tagUID;                  

                        int rowAffected = cmd.ExecuteNonQuery();

                        if (rowAffected > 0) ret = true;
                        cmd.Dispose();
                    }
                    else
                        ret = false;
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }

            public bool AddUidHistory(string _initialUid, string _writtenUid)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tb_UidHistory (InitialUid,WrittenUid,WrittenDate) VALUES (@InitialUid,@WrittenUid,@WrittenDate)";
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@InitialUidD", SqlDbType.VarChar, 20).Value = _initialUid;
                    cmd.Parameters.Add("@_writtenUid", SqlDbType.VarChar, 20).Value = _writtenUid;                
                    cmd.Parameters.Add("@WrittenDate",SqlDbType.VarChar, 20).Value =  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    int rowAffected = cmd.ExecuteNonQuery();
                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
               finally
                {             
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public string GetInitialUID(string tagUID)
            {
                string _initialUID = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;

                    sqlQuery += "SELECT * FROM  tb_UidHistory ";
                    sqlQuery += "WHERE WrittenUid=@WrittenUid ";
                    sqlQuery += "ORDER BY IdWrite DESC;";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;                 
                    cmd.Parameters.Add("@WrittenUid", SqlDbType.VarChar, 20).Value = tagUID;
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        rd.Read();
                        _initialUID = rd[1].ToString();
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return _initialUID;
            }
            public List<UidWriteHistory> GetUidHistory(string _initialUid)
            {
                List<UidWriteHistory> listWrittenUid = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM  tb_UidHistory ";
                    sqlQuery += "WHERE InitialUid=@InitialUid ";
                    sqlQuery += "ORDER BY IdWrite DESC;";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@InitialUid", SqlDbType.VarChar, 20).Value = _initialUid;
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        listWrittenUid = new List<UidWriteHistory>();
                        while (rd.Read())
                        {
                            UidWriteHistory tmpUid = new UidWriteHistory();
                            tmpUid._idWrite = Convert.ToInt32(rd["IdWrite"].ToString());
                            tmpUid._initialUid = rd["InitialUid"].ToString();
                            tmpUid._writtenUid = rd["WrittenUid"].ToString();
                            tmpUid._writtenDate = DateTime.ParseExact(rd["WrittenDate"].ToString(), "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal);
                            listWrittenUid.Add(tmpUid);
                        }
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return listWrittenUid;

            }
        #endregion
        #region Inventory
            public bool IsInventoryExist(InventoryData IDtoStore)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    DateTime gmtDate = IDtoStore.eventDate.ToUniversalTime();
                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Scan_Event ";
                    sqlQuery += "WHERE SerialRFID='" + IDtoStore.serialNumberDevice;
                    sqlQuery += "' AND SortedEventDate='" + gmtDate.ToString("s") + "';";
                  
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();
                    ret = rd.HasRows;
                    rd.Close();
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool StoreInventory(InventoryData IDtoStore , bool bStoreforDevice = false)
            //public bool StoreInventory(InventoryData IDtoStore)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    Hashtable ColumnInfo = GetColumnInfo();

                    DateTime gmtDate = IDtoStore.eventDate.ToUniversalTime();
                    IDtoStore.eventDate = gmtDate;

                    int nbTostore = IDtoStore.nbTagAdded + IDtoStore.nbTagPresent + IDtoStore.nbTagRemoved;
                    int checkNbToStore = IDtoStore.dtTagAdded.Rows.Count + IDtoStore.dtTagPresent.Rows.Count + IDtoStore.dtTagRemove.Rows.Count;
                    if (nbTostore != checkNbToStore)
                    {
                        string info = string.Format("Added : {0}/{1} , Present : {2}/{3} , Removed : {4}/{5}", IDtoStore.nbTagAdded, IDtoStore.dtTagAdded.Rows.Count, IDtoStore.nbTagPresent, IDtoStore.dtTagPresent.Rows.Count, IDtoStore.nbTagRemoved, IDtoStore.dtTagRemove.Rows.Count);
                        ErrorMessage.ExceptionMessageBox.Show("Error Before Convert for Store", info, "Info in  store inventory DBClassSQlite");
                        return false;
                    }

                    StoredInventoryData siv = ConvertInventory.ConvertForStore(IDtoStore, ColumnInfo);

                    if (siv == null)
                    {
                        string info = string.Format("Added : {0}/{1} , Present : {2}/{3} , Removed : {4}/{5}", IDtoStore.nbTagAdded, IDtoStore.dtTagAdded.Rows.Count, IDtoStore.nbTagPresent, IDtoStore.dtTagPresent.Rows.Count, IDtoStore.nbTagRemoved, IDtoStore.dtTagRemove.Rows.Count);
                        ErrorMessage.ExceptionMessageBox.Show("Error After Convert for Store", info, "Info in  store inventory DBClassSQlite");
                       
                        return false;
                    }
                    if (siv.serialNumberDevice.Equals("xxxxxxxx"))
                        return false;


                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream();
                    //bf.Serialize(mem, IDtoStore);
                    bf.Serialize(mem, siv);
                    string idStream = Convert.ToBase64String(mem.ToArray());


                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;


                    sqlQuery += "INSERT INTO tbo_Scan_Event(SerialRFID,bUserScan,FirstName,LastName,EventDate,SortedEventDate,NbTagAll,nbTagInReader,NbTagAdded,NbTagRemoved,InventoryStream) VALUES('";
                    sqlQuery += IDtoStore.serialNumberDevice + "',";
                    sqlQuery += Convert.ToByte(IDtoStore.bUserScan).ToString() + ",'";
                    sqlQuery += IDtoStore.userFirstName + "','";
                    sqlQuery += IDtoStore.userLastName + "','";
                    sqlQuery += gmtDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                    sqlQuery += gmtDate.ToString("s") + "',";
                    sqlQuery += IDtoStore.nbTagAll.ToString() + ",";
                    sqlQuery += IDtoStore.nbTagPresent.ToString() + ",";
                    sqlQuery += IDtoStore.nbTagAdded.ToString() + ",";
                    sqlQuery += IDtoStore.nbTagRemoved.ToString() + ",'";
                    sqlQuery += idStream + "');";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);

                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public int getRowInsertIndex()
            {
                int rowInserted = -1;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return -1;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT @@IDENTITY";                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rowInserted = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return rowInserted;
            }
            public bool storeTagEvent(InventoryData IDtoStore, int IdScan)
            {
                bool ret = false;               
                try
                {
                    DeviceInfo di = RecoverDevice(IDtoStore.serialNumberDevice);
                    if (di == null) return false;

                    if ((di.deviceType == DeviceType.DT_SBR) || (di.deviceType == DeviceType.DT_STR))
                    {
                        trans = globalCon.BeginTransaction();
                        foreach (string TagAdded in IDtoStore.listTagAll)
                            storeTag(IdScan, TagAdded, TagEventType.TET_Present);
                        trans.Commit();
                    }
                    else
                    {
                        trans = globalCon.BeginTransaction();
                        foreach (string TagAdded in IDtoStore.listTagAdded)
                            storeTag(IdScan, TagAdded, TagEventType.TET_Added);
                        trans.Commit();
                        trans = globalCon.BeginTransaction();
                        foreach (string TagRemove in IDtoStore.listTagRemoved)
                            storeTag(IdScan, TagRemove, TagEventType.TET_Removed);
                        trans.Commit();
                    }


                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                return ret;
            }
            private bool storeTag(int idScan, string UID, TagEventType TagEventType)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_Tag_Event (idScanEvent,TagUID,IdTagEventType) VALUES(";
                    sqlQuery += idScan + ",";
                    sqlQuery += "@UID,";
                    sqlQuery += (int)TagEventType + ");";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text; 
                    cmd.Parameters.Add("@UID", SqlDbType.VarChar, 20).Value = UID;
                    cmd.Transaction = trans;
                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }   
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public InventoryData[] GetInventory(DeviceInfo di, UserClassTemplate uct, int nbData = 100)
            {
                InventoryData[] inventoryArray = null;
                ArrayList listInventory = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    Hashtable ColumnInfo = GetColumnInfo();

                    string sqlQuery = null;
                    sqlQuery += "SELECT TOP " +  nbData + " InventoryStream,SortedEventDate FROM tbo_Scan_Event WHERE 1=1 ";
                    if (uct != null)
                    {
                        sqlQuery += "AND FirstName='" + uct.firstName + "' ";
                        sqlQuery += "AND LastName='" + uct.lastName + "' ";
                    }
                    if (di != null)
                        sqlQuery += "AND SerialRFID='" + di.SerialRFID + "' ";
                    sqlQuery += "ORDER BY SortedEventDate DESC;";
                                        
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            if (rd[0] == DBNull.Value) continue;
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString().Trim()));
                            //InventoryData dt = new InventoryData();
                            //dt = (InventoryData)bf.Deserialize(mem);

                            StoredInventoryData siv = new StoredInventoryData();
                            siv = (StoredInventoryData)bf.Deserialize(mem);
                            InventoryData dt = ConvertInventory.ConvertForUse(siv, ColumnInfo);

                            DateTime utcDate = dt.eventDate;
                            dt.eventDate = utcDate.ToLocalTime();

                            listInventory.Add(dt);
                        }                        
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (listInventory.Count > 0)
                {
                    inventoryArray = new InventoryData[listInventory.Count];
                    listInventory.CopyTo(inventoryArray);
                }
                return inventoryArray;
            }
            public string[] GetInventoryBefore(string SerialRFID, string DateTimetoParse)
            {
                ArrayList listInventory = new ArrayList();
                string[] strData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    //DateTime dt = DateTime.Parse(DateTimetoParse);
                    DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tbo_Scan_Event WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "AND SortedEventDate<'" + dt.ToUniversalTime().ToString("s") + "' ";
                    sqlQuery += "ORDER BY SortedEventDate DESC";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            if (rd[0] == DBNull.Value) continue;
                            listInventory.Add(rd[0].ToString().Trim());
                        }                       
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                if (listInventory.Count > 0)
                {
                    strData = new string[listInventory.Count];
                    int nIndex = 0;
                    foreach (string dt in listInventory)
                        strData[nIndex++] = dt;
                }
                return strData;
            }
            public string[] GetInventoryAfter(string SerialRFID, string DateTimetoParse)
            {
                ArrayList listInventory = new ArrayList();
                string[] strData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    //DateTime dt = DateTime.Parse(DateTimetoParse);
                    DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tbo_Scan_Event WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "AND SortedEventDate>'" + dt.ToUniversalTime().ToString("s") + "' ";
                    sqlQuery += "ORDER BY SortedEventDate ASC";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            if (rd[0] == DBNull.Value) continue;
                            listInventory.Add(rd[0].ToString().Trim());
                        }                        
                    }
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (listInventory.Count > 0)
                {
                    strData = new string[listInventory.Count];
                    int nIndex = 0;
                    foreach (string dt in listInventory)
                        strData[nIndex++] = dt;
                }
                return strData;
            }
            public string[] GetInventory(string SerialRFID, string DateTimetoParse)
            {

                ArrayList listInventory = new ArrayList();
                string[] strData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    //DateTime dt = DateTime.Parse(DateTimetoParse);
                    DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tbo_Scan_Event WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "AND SortedEventDate>'" + dt.ToUniversalTime().ToString("s") + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            if (rd[0] == DBNull.Value) continue;
                            listInventory.Add(rd[0].ToString().Trim());
                        }                       
                    }
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (listInventory.Count > 0)
                {
                    strData = new string[listInventory.Count];
                    int nIndex = 0;
                    foreach (string dt in listInventory)
                        strData[nIndex++] = dt;
                }
                return strData;
            }
            public string[] GetInventoryFromData(string SerialRFID, string spareData1, string spareData2)
            {
                ArrayList listInventory = new ArrayList();
                string[] strData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {                  

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT InventoryStream,SortedEventDate FROM tbo_Scan_Event WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "AND SpareData1='" + spareData1 + "' ";
                    sqlQuery += "AND SpareData2='" + spareData2 + "';";                   

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            if (rd[0] == DBNull.Value) continue;
                            listInventory.Add(rd[0].ToString().Trim());
                        }
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (listInventory.Count > 0)
                {
                    strData = new string[listInventory.Count];
                    int nIndex = 0;
                    foreach (string dt in listInventory)
                        strData[nIndex++] = dt;
                }
                return strData;
            }

            public InventoryData GetLastScan(string serialRFID)
            {
                InventoryData retData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    Hashtable ColumnInfo = GetColumnInfo();


                    string sqlQuery = null;
                    sqlQuery += "SELECT TOP 1 * FROM tbo_Scan_Event ";
                    sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                    sqlQuery += "ORDER BY SortedEventDate DESC";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        if (rd["InventoryStream"] == DBNull.Value) return null;
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String((rd["InventoryStream"].ToString().Trim())));
                        //retData = new InventoryData();
                        //retData = (InventoryData)bf.Deserialize(mem);
                        StoredInventoryData siv = new StoredInventoryData();
                        siv = (StoredInventoryData)bf.Deserialize(mem);
                        retData = ConvertInventory.ConvertForUse(siv, ColumnInfo);

                        DateTime utcDate = retData.eventDate;
                        retData.eventDate = utcDate.ToLocalTime();                      
                    }                    

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return retData;
            }
            public InventoryData GetLastScanFromID(string serialRFID, int IdEvent)
            {
                InventoryData retData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    Hashtable ColumnInfo = GetColumnInfo();


                    string sqlQuery = null;

                    sqlQuery += "SELECT * FROM tb_Scan_Event ";
                    sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                    sqlQuery += "AND IdScanEvent=" + IdEvent + " ;";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        if (rd["InventoryStream"] == DBNull.Value) return null;
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String((rd["InventoryStream"].ToString().Trim())));
                        //retData = new InventoryData();
                        //retData = (InventoryData)bf.Deserialize(mem);
                        StoredInventoryData siv = new StoredInventoryData();
                        siv = (StoredInventoryData)bf.Deserialize(mem);
                        retData = ConvertInventory.ConvertForUse(siv, ColumnInfo);

                        DateTime utcDate = retData.eventDate;
                        retData.eventDate = utcDate.ToLocalTime();
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return retData;
            }
            public TagEventClass[] GetTagEvent(string tagUID)
            {
                TagEventClass[] tagEventArray = null;
                ArrayList listTagEvent = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "SELECT TTE.*,TSE.SerialRFID,TDD.DeviceName,TSE.SortedEventDate,TSE.bUserScan,TSE.FirstName,TSE.LastName ";
                    sqlQuery += "FROM (( tbo_Tag_Event AS TTE ";
                    sqlQuery += "INNER JOIN tbo_Scan_Event AS TSE ON TTE.IdScanEvent=TSE.IdScanEvent) ";
                    sqlQuery += "INNER JOIN tbo_Device_Def AS TDD ON TSE.SerialRFID=TDD.SerialRFID) ";
                    sqlQuery += "WHERE TTE.TagUID=@tagUID ";
                    sqlQuery += "ORDER BY TSE.SortedEventDate DESC";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = tagUID;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            TagEventClass tec = new TagEventClass();
                            DateTime TmpUtc = DateTime.ParseExact(rd["SortedEventDate"].ToString().Trim(), "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                            DateTime TmpLocal = TmpUtc.ToLocalTime();
                            tec.eventSortedDate = TmpLocal.ToString("s");
                            tec.eventDate = (string)TmpLocal.ToString("G");
                            tec.serialRFID = rd["SerialRFID"].ToString().Trim();

                            int eventType = int.Parse(rd["IdTagEventType"].ToString().Trim());
                            switch ((TagEventType)eventType)
                            {
                                case TagEventType.TET_Removed: tec.tagEventType = "Removed"; break;
                                case TagEventType.TET_Added: tec.tagEventType = "Added"; break;
                                case TagEventType.TET_Present: tec.tagEventType = "Present"; break;
                            }

                            tec.tagUID = rd["TagUID"].ToString().Trim();
                            tec.DeviceName = rd["DeviceName"].ToString().Trim();
                            tec.FirstName = rd["FirstName"].ToString().Trim();
                            tec.LastName = rd["LastName"].ToString().Trim();
                            listTagEvent.Add(tec);
                        }                      
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (listTagEvent.Count > 0)
                {
                    tagEventArray = new TagEventClass[listTagEvent.Count];
                    listTagEvent.CopyTo(tagEventArray);
                }
                return tagEventArray;
            }
            public TagEventClass GetLastTagEvent(string tagUID)
            {
                TagEventClass tec = null;
                ArrayList listTagEvent = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "SELECT TOP 1 TTE.*,TSE.SerialRFID,TDD.DeviceName,TSE.SortedEventDate,TSE.bUserScan,TSE.FirstName,TSE.LastName ";
                    sqlQuery += "FROM (( tbo_Tag_Event AS TTE ";
                    sqlQuery += "INNER JOIN tbo_Scan_Event AS TSE ON TTE.IdScanEvent=TSE.IdScanEvent) ";
                    sqlQuery += "INNER JOIN tbo_Device_Def AS TDD ON TSE.SerialRFID=TDD.SerialRFID) ";
                    sqlQuery += "WHERE TTE.TagUID=@tagUID ";
                    sqlQuery += "ORDER BY TSE.SortedEventDate DESC";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = tagUID;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();

                        tec = new TagEventClass();
                        DateTime TmpUtc = DateTime.ParseExact(rd["SortedEventDate"].ToString().Trim(), "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                        DateTime TmpLocal = TmpUtc.ToLocalTime();
                        tec.eventSortedDate = TmpLocal.ToString("s");
                        tec.eventDate = (string)TmpLocal.ToString("G");
                        tec.serialRFID = rd["SerialRFID"].ToString().Trim();

                        int eventType = int.Parse(rd["IdTagEventType"].ToString().Trim());
                        switch ((TagEventType)eventType)
                        {
                            case TagEventType.TET_Removed: tec.tagEventType = "Removed"; break;
                            case TagEventType.TET_Added: tec.tagEventType = "Added"; break;
                            case TagEventType.TET_Present: tec.tagEventType = "Present"; break;
                        }

                        tec.tagUID = rd["TagUID"].ToString().Trim();
                        tec.DeviceName = rd["DeviceName"].ToString().Trim();
                        tec.FirstName = rd["FirstName"].ToString().Trim();
                        tec.LastName = rd["LastName"].ToString().Trim();
                        listTagEvent.Add(tec);                       
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return tec;
            }
            public bool DeleteOldInventory(string serialRFID, int NbToKeep)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;

                    sqlQuery += "UPDATE tbo_Scan_Event SET InventoryStream=' ' ";
                    sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                    sqlQuery += "AND IdScanEvent NOT IN ";
                    sqlQuery += "(SELECT TOP " + NbToKeep.ToString() +" IdScanEvent FROM tbo_Scan_Event ";
                    sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                    sqlQuery += "ORDER BY SortedEventDate DESC)";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();

                    ret = true;

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
        #endregion
        #region Group
            public DataTable RecoverAllGroup()
            {
                DataTable dtGet = null;               
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    {
                        string sqlQuery = null;
                        sqlQuery += "SELECT * FROM tbo_Group_Def;";
                        SqlDataAdapter da = new SqlDataAdapter(sqlQuery, globalCon);
                        dtGet = new DataTable();
                        da.Fill(dtGet);
                        
                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                return dtGet;
            }
            public bool DeleteTagLinkFromBox(string tagBox)
            {
                bool ret = false;
                SqlCommand cmd2 = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Group_Link ";
                    sqlQuery += "WHERE GroupTagRef=@tagBox;";
                    
                    cmd2.Transaction = trans;
                    cmd2.Connection = globalCon;
                    cmd2.CommandText = sqlQuery;
                    cmd2.CommandType = System.Data.CommandType.Text;
                    cmd2.Parameters.Add("@tagBox", SqlDbType.VarChar, 20).Value = tagBox;
                    cmd2.ExecuteNonQuery();

                    int rowAffected = cmd2.ExecuteNonQuery();
                    if (rowAffected > 0) ret = true;
                    

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd2 != null) cmd2.Dispose();
                }
                return ret;
            }
            public bool DeleteGroup(string tagBox)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlCommand cmd2 = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Group_Link ";
                    sqlQuery += "WHERE GroupTagRef=@tagBox;";

                    
                    cmd2.Transaction = trans;
                    cmd2.Connection = globalCon;
                    cmd2.CommandText = sqlQuery;
                    cmd2.CommandType = System.Data.CommandType.Text;
                    cmd2.Parameters.Add("@tagBox", SqlDbType.VarChar, 20).Value = tagBox;
                    cmd2.ExecuteNonQuery();


                    sqlQuery += "DELETE FROM tbo_Group_Def ";
                    sqlQuery += "WHERE TagGroup='" + tagBox + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();


                    if (rowAffected > 0) ret = true;                   

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                    if (cmd2 != null) cmd2.Dispose();
                }

                return ret;
            }
            public bool AddGroup(string TagBox, string groupRef, string groupInfo, string criteria = null)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    if (string.IsNullOrEmpty(criteria))
                    {
                        sqlQuery += "INSERT INTO tbo_Group_Def(TagGroup,GroupRef,GroupInfo) VALUES(";
                        sqlQuery += "@tagBox,'";
                        sqlQuery += groupRef + "','";
                        sqlQuery += groupInfo + "');";
                    }
                    else
                    {
                        sqlQuery += "INSERT INTO tbo_Group_Def(TagGroup,GroupRef,GroupInfo,Criteria) VALUES(";
                        sqlQuery += "@tagBox','";
                        sqlQuery += groupRef + "','";
                        sqlQuery += groupInfo + "','";
                        sqlQuery += criteria + "');";
                    }
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagBox", SqlDbType.VarChar, 20).Value = TagBox;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public int GetGroupLinkCount(string GroupTagRef)
            {
                int ret = 0;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT COUNT(ProductTagRef) FROM tbo_Group_Link ";
                    sqlQuery += "WHERE GroupTagRef=@GroupTagRef;";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@GroupTagRef", SqlDbType.VarChar, 20).Value = GroupTagRef;
                    ret = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;

            }
            public string GetGroupName(string TagUID)
            {
                string groupName = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT TGD.GroupRef ";
                    sqlQuery += "FROM (tbo_Group_Def AS TGD ";
                    sqlQuery += "INNER JOIN tbo_Group_Link AS TGL ";
                    sqlQuery += "ON TGL.GroupTagRef=TGD.TagGroup) ";
                    sqlQuery += "WHERE TGL.ProductTagRef=@tagUID;";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@tagUID", SqlDbType.VarChar, 20).Value = TagUID;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            groupName = rd["GroupRef"].ToString().Trim();
                        }

                    }   
                }

                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return groupName;
            }
            public ArrayList GetGroupLink(string GroupTagRef)
            {
                ArrayList link = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT ProductTagRef FROM tbo_Group_Link ";
                    sqlQuery += "WHERE GroupTagRef=@GroupTagRef;";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@GroupTagRef", SqlDbType.VarChar, 20).Value = GroupTagRef;

                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        link = new ArrayList();
                        while (rd.Read())
                        {
                            link.Add(rd["ProductTagRef"].ToString().Trim());
                        }
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return link;

            }
            public bool DeleteGroupLink(string ProductTagRef)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();


                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Group_Link ";
                    sqlQuery += "WHERE ProductTagRef=@ProductTagRef;";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@ProductTagRef", SqlDbType.VarChar, 20).Value = ProductTagRef;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                 

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return ret;
            }
            public bool AddGroupLink(string GroupTagRef, string productTagRef)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_Group_Link(GroupTagRef,ProductTagRef) VALUES(";
                    sqlQuery += "@GroupTagRef,";
                    sqlQuery += "@productTagRef);";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@GroupTagRef", SqlDbType.VarChar, 20).Value = GroupTagRef;
                    cmd.Parameters.Add("@productTagRef", SqlDbType.VarChar, 20).Value = productTagRef;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
        #endregion
        #region Image
            public ArrayList GetImageListName()
            {
                ArrayList img = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT ImageName FROM tbo_Image;";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                   rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        img = new ArrayList();
                        while (rd.Read())
                        {
                            img.Add(rd["ImageName"].ToString().Trim());
                        }
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return img;

            }
            public bool IsImageExist(string imageName)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Image ";
                    sqlQuery += "WHERE ImageName='" + imageName + "';";

                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

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
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool AddImage(string imageName, byte[] imageArray)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();


                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_Image(ImageName,ImageByteArray) VALUES('";
                    sqlQuery += imageName + "',@byteArray);";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@byteArray", SqlDbType.Binary).Value = imageArray;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool DeleteImage(string imageName)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Image ";
                    sqlQuery += "WHERE ImageName='" + imageName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                  

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return ret;
            }
            public byte[] getImage(string imageName)
            {

                byte[] image = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT ImageByteArray FROM tbo_Image ";
                    sqlQuery += "WHERE ImageName='" + imageName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            image = GetBytes(reader);
                        }
                        reader.Close();
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return image;
            }
            static byte[] GetBytes(SqlDataReader reader)
            {
                const int CHUNK_SIZE = 2 * 1024;
                byte[] buffer = new byte[CHUNK_SIZE];
                long bytesRead;
                long fieldOffset = 0;
                using (MemoryStream stream = new MemoryStream())
                {
                    while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                    {
                        byte[] actualRead = new byte[bytesRead];
                        Buffer.BlockCopy(buffer, 0, actualRead, 0, (int)bytesRead);
                        stream.Write(actualRead, 0, actualRead.Length);
                        fieldOffset += bytesRead;
                    }
                    return stream.ToArray();
                }
            }
            public int getNumberofLink(string ImageName)
            {
                int ret = 0;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT COUNT(LotIDorTagID) FROM tbo_Image_Link ";
                    sqlQuery += "WHERE ImageName='" + ImageName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    ret = Convert.ToInt32(cmd.ExecuteScalar());                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool IsImageLinkExist(string LotIDorTagID)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Image_Link ";
                    sqlQuery += "WHERE LotIDorTagID='" + LotIDorTagID + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();
                    ret = rd.HasRows;
                    rd.Close();
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public string getImageNameLink(string LotIDorTagID)
            {
                string imageName = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Image_Link ";
                    sqlQuery += "WHERE LotIDorTagID='" + LotIDorTagID + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            imageName = rd["ImageName"].ToString().Trim();
                        }

                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return imageName;
            }
            public bool AddImageLink(string imageName, string LotIDorTagID)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();


                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_Image_Link (LotIDorTagID,ImageName) VALUES('";
                    sqlQuery += LotIDorTagID + "','" + imageName + "');";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool DeleteallImageLink(string ImageName)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Image_Link ";
                    sqlQuery += "WHERE ImageName='" + ImageName + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();
                   
                    ret = true;

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return ret;
            }
            public bool DeleteImageLink(string LotIDorTagID)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Image_Link ";
                    sqlQuery += "WHERE LotIDorTagID='" + LotIDorTagID + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                    

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return ret;
            }
        #endregion
        #region User
            public UserClassTemplate[] RecoverUser()
            {
                UserClassTemplate[] userArray = null;
                ArrayList listUser = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT FPTemplate FROM tbo_User_Def";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString().Trim()));
                            UserClassTemplate TheUser = new UserClassTemplate();
                            TheUser = (UserClassTemplate)bf.Deserialize(mem);
                            listUser.Add(TheUser);

                        }                       
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                if (listUser.Count > 0)
                {
                    userArray = new UserClassTemplate[listUser.Count];
                    listUser.CopyTo(userArray);
                }
                return userArray;
            }
            /*public UserClassTemplate[] RecoverAllowedUser(string serialRFID)
            {
                UserClassTemplate[] userArray = null;
                ArrayList listUser = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT TUD.FPTemplate ";
                    sqlQuery += "FROM (tbo_User_Def AS TUD ";
                    sqlQuery += "INNER JOIN tbo_User_Grant AS TUG ";
                    sqlQuery += "ON TUG.FirstName=TUD.FirstName ";
                    sqlQuery += "AND TUG.LastName=TUD.LastName) ";
                    sqlQuery += "WHERE TUG.SerialRFID='" + serialRFID + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString().Trim()));
                            UserClassTemplate TheUser = new UserClassTemplate();
                            TheUser = (UserClassTemplate)bf.Deserialize(mem);
                            listUser.Add(TheUser);
                        }                       
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (listUser.Count > 0)
                {
                    userArray = new UserClassTemplate[listUser.Count];
                    listUser.CopyTo(userArray);
                }
                return userArray;
            }*/
            public DeviceGrant[] RecoverAllowedUser(string serialRFID)
            {
                DeviceGrant[] userArray = null;
                ArrayList listUser = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT TUD.FPTemplate,TUG.DoorGranted ";
                    sqlQuery += "FROM (tbo_User_Def AS TUD ";
                    sqlQuery += "INNER JOIN tbo_User_Grant AS TUG ";
                    sqlQuery += "ON TUG.FirstName=TUD.FirstName ";
                    sqlQuery += "AND TUG.LastName=TUD.LastName) ";
                    sqlQuery += "WHERE TUG.SerialRFID='" + serialRFID + "';";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString()));
                            UserClassTemplate TheUser = new UserClassTemplate();
                            TheUser = (UserClassTemplate)bf.Deserialize(mem);

                            DeviceGrant newuser = new DeviceGrant();
                            newuser.user = TheUser;
                            int reg = int.Parse(rd["DoorGranted"].ToString());
                            newuser.userGrant = (UserGrant)reg;
                            listUser.Add(newuser);
                        }
                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (listUser.Count > 0)
                {
                    userArray = new DeviceGrant[listUser.Count];
                    listUser.CopyTo(userArray);
                }
                return userArray;
            }

            public UserClassTemplate RecoverUser(string firstName, string lastName)
            {

                UserClassTemplate user = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT FPTemplate FROM tbo_User_Def ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream(Convert.FromBase64String(rd[0].ToString().Trim()));
                            user = new UserClassTemplate();
                            user = (UserClassTemplate)bf.Deserialize(mem);

                        }                       
                    }                  
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return user;
            }
            public bool IsUserExist(string firstName, string lastName)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_User_Def ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();
                    ret = rd.HasRows;
                    rd.Close();
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool StoreUser(UserClassTemplate UserToStore)
            {

                bool ret = false;

                if (IsUserExist(UserToStore.firstName, UserToStore.lastName))
                {
                    ret = UpdateUser(UserToStore);
                }
                else
                {
                    ret = AddUser(UserToStore);
                }
                return ret;
            }
            private bool AddUser(UserClassTemplate UserToStore)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream();
                    bf.Serialize(mem, UserToStore);
                    string str = Convert.ToBase64String(mem.ToArray());

                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_User_Def(FirstName,LastName,FPTemplate) VALUES('";
                    sqlQuery += UserToStore.firstName + "','";
                    sqlQuery += UserToStore.lastName + "','";
                    sqlQuery += str + "');";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            private bool UpdateUser(UserClassTemplate UserToStore)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream();
                    bf.Serialize(mem, UserToStore);
                    string str = Convert.ToBase64String(mem.ToArray());

                    string sqlQuery = null;
                    sqlQuery += "UPDATE tbo_User_Def ";
                    sqlQuery += "SET FPTemplate='" + str + "' ";
                    sqlQuery += "WHERE FirstName='" + UserToStore.firstName + "' ";
                    sqlQuery += "AND LastName='" + UserToStore.lastName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                  
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public int getUserFingerAlert(string firstName, string lastName)
            {
                int ret = -1;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;

                    sqlQuery += "SELECT FingerAlert FROM tbo_User_Def ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    ret = Convert.ToInt32(cmd.ExecuteScalar());
                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return ret;
            }
            public bool UpdateUserFingerAlert(string firstName, string lastName, int fingerAlert)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "UPDATE tbo_User_Def ";
                    sqlQuery += "SET FingerAlert=" + fingerAlert + " ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                   

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool UpdateUserMaxRemovedItem(string firstName, string lastName, int maxItem)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "UPDATE tbo_User_Def ";
                    sqlQuery += "SET MaxRemoveItem=" + maxItem + " ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                    

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public int getUserMaxRemovedItem(string firstName, string lastName)
            {
                int ret = -1;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;

                    sqlQuery += "SELECT MaxRemoveItem FROM tbo_User_Def ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    ret = Convert.ToInt32(cmd.ExecuteScalar());
                  
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool UpdateUserMaxRemoveValue(string firstName, string lastName, float maxRemoveValue)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "UPDATE tbo_User_Def ";
                    sqlQuery += "SET MaxRemoveValue=" + maxRemoveValue + " ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                  

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public double getUserMaxRemoveValue(string firstName, string lastName)
            {
                double ret = -1.0;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;

                    sqlQuery += "SELECT MaxRemoveValue FROM tbo_User_Def ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    ret = Convert.ToDouble(cmd.ExecuteScalar());
                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool DeleteUser(string firstName, string lastName)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_User_Def ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "';";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                   

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool DeleteAllUser()
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_User_Def;";


                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                    

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
        #endregion
        #region user Grant
            public string[] recoverUserGrant(UserClassTemplate User)
            {
                string[] userGrantArray = null;
                ArrayList listUserGrant = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT SerialRFID FROM tbo_User_Grant ";
                    sqlQuery += "WHERE FirstName='" + User.firstName + "' ";
                    sqlQuery += "AND LastName='" + User.lastName + "';";
                 
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            listUserGrant.Add(rd["SerialRFID"].ToString().Trim());
                        }                       
                    }                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                if (listUserGrant.Count > 0)
                {
                    userGrantArray = new string[listUserGrant.Count];
                    listUserGrant.CopyTo(userGrantArray);
                }
                return userGrantArray;
            }
            public DeviceGrant[] recoverUserGrantFull(UserClassTemplate User)
            {
                DeviceGrant[] userGrantArray = null;
                ArrayList listUserGrant = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;

                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT SerialRFID,DoorGranted FROM tbo_User_Grant ";
                    sqlQuery += "WHERE FirstName='" + User.firstName + "' ";
                    sqlQuery += "AND LastName='" + User.lastName + "';";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            DeviceGrant dg = new DeviceGrant();
                            dg.user = User;
                            dg.serialRFID = (string)rd["SerialRFID"];

                            if (rd["DoorGranted"] == DBNull.Value)
                                dg.userGrant = UserGrant.UG_NONE;
                            else
                                dg.userGrant = (UserGrant)(int)rd["DoorGranted"];
                            listUserGrant.Add(dg);
                        }                       
                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                if (listUserGrant.Count > 0)
                {
                    userGrantArray = new DeviceGrant[listUserGrant.Count];
                    listUserGrant.CopyTo(userGrantArray);
                }
                return userGrantArray;
            }

            public bool isGrantExist(string firstName, string lastName, string SerialRFID)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_User_Grant ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "' ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
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
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool StoreGrant(UserClassTemplate UserToStore, string SerialRFID, string BelongTo,UserGrant userGrant)
            {
                bool ret = false;

                if (!isGrantExist(UserToStore.firstName, UserToStore.lastName, SerialRFID))
                {
                    ret = AddUserGrant(UserToStore, SerialRFID, BelongTo, userGrant);
                }
                else
                {
                    DeleteUserGrant(UserToStore.firstName, UserToStore.lastName, SerialRFID);
                    ret = AddUserGrant(UserToStore, SerialRFID, BelongTo, userGrant);
                }
                return ret;
            }
            private bool AddUserGrant(UserClassTemplate UserToStore, string SerialRFID, string BelongTo, UserGrant userGrant)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    if (BelongTo == null)
                    {
                        sqlQuery += "INSERT INTO tbo_User_Grant(FirstName,LastName,SerialRFID,DoorGranted) VALUES('";
                        sqlQuery += UserToStore.firstName + "','";
                        sqlQuery += UserToStore.lastName + "','";
                        sqlQuery += SerialRFID + "',";
                        sqlQuery += (int)userGrant + ")";
                    }
                    else
                    {
                        sqlQuery += "INSERT INTO tbo_User_Grant(FirstName,LastName,SerialRFID,BelongTo,DoorGranted) VALUES('";
                        sqlQuery += UserToStore.firstName + "','";
                        sqlQuery += UserToStore.lastName + "','";
                        sqlQuery += SerialRFID + "','";
                        sqlQuery += BelongTo + "',";
                        sqlQuery += (int)userGrant + ")";
                    }
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }                
                return ret;
            }
            public bool DeleteUserGrant(UserClassTemplate UserToStore, string SerialRFID)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                if (isGrantExist(UserToStore.firstName, UserToStore.lastName, SerialRFID))
                {
                    try
                    {
                        if (globalCon == null) return ret;
                        if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                        string sqlQuery = null;
                        sqlQuery += "DELETE FROM tbo_User_Grant ";
                        sqlQuery += "WHERE FirstName='" + UserToStore.firstName + "' ";
                        sqlQuery += "AND LastName='" + UserToStore.lastName + "' ";
                        sqlQuery += "AND SerialRFID='" + SerialRFID + "';";
                       
                        cmd.Transaction = trans;
                        cmd.Connection = globalCon;
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;

                        int rowAffected = cmd.ExecuteNonQuery();

                        if (rowAffected > 0) ret = true;                        
                    }
                    catch (Exception exp)
                    {
                        // On affiche l'erreur.
                        ErrorMessage.ExceptionMessageBox.Show(exp);
                    }
                    finally
                    {
                        if (rd != null) rd.Close();
                        if (cmd != null) cmd.Dispose();
                    }                   
                }
                return ret;
            }
            public bool DeleteUserGrant(string firstName, string lastName, string SerialRFID)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;

                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_User_Grant ";
                    sqlQuery += "WHERE FirstName='" + firstName + "' ";
                    sqlQuery += "AND LastName='" + lastName + "'";

                    if (string.IsNullOrEmpty(SerialRFID))
                        sqlQuery += ";";
                    else
                        sqlQuery += " AND SerialRFID='" + SerialRFID + "';";


                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }                

                return ret;
            }
            public bool DeleteUserGrant(string BelongTo)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_User_Grant ";
                    sqlQuery += "WHERE BelongTo='" + BelongTo + "';";


                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }                
                return ret;
            }
        #endregion
        #region Device
            public DeviceInfo[] RecoverAllDevice(string machineName = null)
            {
                DeviceInfo[] deviceArray = null;
                ArrayList listDevice = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Device_Def ";
                    if (!string.IsNullOrEmpty(machineName))
                    {
                        sqlQuery += "WHERE BelongTo='" + machineName + "';";
                    }
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            DeviceInfo dev = new DeviceInfo();
                            dev.DeviceName = rd["DeviceName"].ToString().Trim();
                            dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString().Trim());
                            dev.SerialRFID = rd["SerialRFID"].ToString().Trim();

                            dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : rd["SerialFPMaster"].ToString().Trim();
                            dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : rd["SerialFPSlave"].ToString().Trim();
                            dev.bLocal = byte.Parse(rd["bLocal"].ToString().Trim());
                            dev.enabled = int.Parse(rd["Enabled"].ToString().Trim());
                            //dev.comLCD = rd.IsDBNull(rd.GetOrdinal("comLCD")) ? string.Empty : (string)rd["comLCD"];
                            dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : rd["comMasterReader"].ToString().Trim();
                            dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : rd["comSlaveReader"].ToString().Trim();
                            dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : rd["comTempReader"].ToString().Trim();
                            dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString().Trim());
                            dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());
                            listDevice.Add(dev);
                        }                        
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }                
                if (listDevice.Count > 0)
                {
                    deviceArray = new DeviceInfo[listDevice.Count];
                    listDevice.CopyTo(deviceArray);
                }
                return deviceArray;
            }

            public List<DeviceInfo> GetAllDevices()
            {
                List<DeviceInfo> devices = new List<DeviceInfo>();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return devices;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Device_Def;";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (!rd.HasRows) return devices;

                    while (rd.Read())
                    {
                        DeviceInfo dev = new DeviceInfo();
                        dev.DeviceName = rd["DeviceName"].ToString().Trim();
                        dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString().Trim());
                        dev.SerialRFID = rd["SerialRFID"].ToString().Trim();

                        dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : rd["SerialFPMaster"].ToString().Trim();
                        dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : rd["SerialFPSlave"].ToString().Trim();
                        dev.bLocal = byte.Parse(rd["bLocal"].ToString().Trim());
                        dev.enabled = int.Parse(rd["Enabled"].ToString().Trim());

                        dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : rd["comMasterReader"].ToString().Trim();
                        dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : rd["comSlaveReader"].ToString().Trim();
                        dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : rd["comTempReader"].ToString().Trim();
                        dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString().Trim());
                        dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());

                        devices.Add(dev);
                    }
                }
                catch (Exception)
                {
                }

                return devices;
            }

            public DeviceInfo[] RecoverDevice(bool bLocal,string machineName = null)
            {
                DeviceInfo[] deviceArray = null;
                ArrayList listDevice = new ArrayList();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Device_Def ";
                    if (bLocal)
                        sqlQuery += "WHERE bLocal=1 ";
                    else
                        sqlQuery += "WHERE bLocal=0 ";

                    if (!string.IsNullOrEmpty(machineName))
                    {
                        sqlQuery += "AND BelongTo='" + machineName + "';";
                    }
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            DeviceInfo dev = new DeviceInfo();
                            dev.DeviceName = rd["DeviceName"].ToString().Trim();
                            dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString().Trim());
                            dev.SerialRFID = rd["SerialRFID"].ToString().Trim();

                            dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : rd["SerialFPMaster"].ToString().Trim();
                            dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : rd["SerialFPSlave"].ToString().Trim();
                            dev.IP_Server = rd.IsDBNull(rd.GetOrdinal("IP_Server")) ? string.Empty : rd["IP_Server"].ToString().Trim();
                            dev.Port_Server = rd.IsDBNull(rd.GetOrdinal("Port_Server")) ? 0 : int.Parse(rd["Port_Server"].ToString().Trim());
                            dev.bLocal = byte.Parse(rd["bLocal"].ToString().Trim());
                            dev.enabled = int.Parse(rd["Enabled"].ToString().Trim());
                            //dev.comLCD = rd.IsDBNull(rd.GetOrdinal("comLCD")) ? string.Empty : (string)rd["comLCD"];
                            dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : rd["comMasterReader"].ToString().Trim();
                            dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : rd["comSlaveReader"].ToString().Trim();
                            dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : rd["comTempReader"].ToString().Trim();
                            dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString().Trim());
                            dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());
                            listDevice.Add(dev);
                        }                        
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }                
                if (listDevice.Count > 0)
                {
                    deviceArray = new DeviceInfo[listDevice.Count];
                    listDevice.CopyTo(deviceArray);
                }
                return deviceArray;
            }
            public DeviceInfo RecoverDevice(string SerialRFID)
            {

                DeviceInfo dev = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Device_Def ";
                    sqlQuery += "WHERE SerialRFID='" + SerialRFID + "';";

  
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            dev = new DeviceInfo();
                            dev.DeviceName = rd["DeviceName"].ToString().Trim();
                            dev.deviceType = (DeviceType)int.Parse(rd["IdDeviceType"].ToString().Trim()); ;
                            dev.SerialRFID = rd["SerialRFID"].ToString().Trim();

                            dev.SerialFPMaster = rd.IsDBNull(rd.GetOrdinal("SerialFPMaster")) ? string.Empty : rd["SerialFPMaster"].ToString().Trim();
                            dev.SerialFPSlave = rd.IsDBNull(rd.GetOrdinal("SerialFPSlave")) ? string.Empty : rd["SerialFPSlave"].ToString().Trim();
                            dev.bLocal = byte.Parse(rd["bLocal"].ToString().Trim().ToString());
                            dev.IP_Server = rd.IsDBNull(rd.GetOrdinal("IP_server")) ? string.Empty : rd["IP_server"].ToString().Trim();
                            dev.Port_Server = rd.IsDBNull(rd.GetOrdinal("Port_Server")) ? 0 : int.Parse(rd["Port_Server"].ToString().Trim());
                            dev.enabled = int.Parse(rd["Enabled"].ToString().Trim().ToString());
                            //dev.comLCD = rd.IsDBNull(rd.GetOrdinal("comLCD")) ? string.Empty : (string)rd["comLCD"];
                            dev.comMasterReader = rd.IsDBNull(rd.GetOrdinal("comMasterReader")) ? string.Empty : rd["comMasterReader"].ToString().Trim();
                            dev.comSlaveReader = rd.IsDBNull(rd.GetOrdinal("comSlaveReader")) ? string.Empty : rd["comSlaveReader"].ToString().Trim();
                            dev.comTempReader = rd.IsDBNull(rd.GetOrdinal("comTempReader")) ? string.Empty : rd["comTempReader"].ToString().Trim();
                            dev.accessReaderType = (AccessBagerReaderType)int.Parse(rd["accessReaderType"].ToString().Trim());
                            dev.fridgeType = (FridgeType)int.Parse(rd["fridgeType"].ToString());
                        }                        
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return dev;
            }
            public ArrayList RecoverDistinctSerialRFID()
            {
                ArrayList listSerial = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;
                    sqlQuery += "SELECT DISTINCT [SerialRFID] ";
                    sqlQuery += "FROM [DB_ST2013].[dbo].[tbo_Device_Def]";
                        

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        listSerial = new ArrayList();
                        while (rd.Read())
                        {  
                            string serial = rd["SerialRFID"].ToString().Trim();
                            if (!listSerial.Contains(serial))
                                listSerial.Add(serial);
                        }                          
                    }                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return listSerial;

            }
            public bool IsDeviceExist(string SerialRFID, string machineName = null)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Device_Def "; ;
                    sqlQuery += "WHERE SerialRFID='" + SerialRFID + "' ";
                    if (!string.IsNullOrEmpty(machineName))
                    {
                        sqlQuery += "AND BelongTo='" + machineName + "';";
                    }
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text; ;

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
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool StoreDevice(DeviceInfo DeviceToStore, bool bNetwork, string machineName = null)
            {

                bool ret = false;

                if (IsDeviceExist(DeviceToStore.SerialRFID,machineName))
                {
                    ret = UpdateDevice(DeviceToStore, bNetwork,machineName);
                }
                else
                {
                    ret = AddDevice(DeviceToStore, bNetwork);
                }
                return ret;
            }
            private bool AddDevice(DeviceInfo DeviceToStore, bool bNetwork)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();                   
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;                   

                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_Device_Def(DeviceName,IdDeviceType,SerialRFID,bLocal,Enabled,BelongTo) VALUES('";
                    sqlQuery += DeviceToStore.DeviceName + "',";
                    sqlQuery += (int)DeviceToStore.deviceType + ",'";
                    sqlQuery += DeviceToStore.SerialRFID + "',";
                    sqlQuery += "1,";
                    sqlQuery += (int)DeviceToStore.enabled + ",'";
                    sqlQuery += Environment.MachineName.ToString() + "');";

                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();

                    if (DeviceToStore.fridgeType != FridgeType.FT_UNKNOWN)
                    {
                        sqlQuery = null;
                        sqlQuery += "UPDATE tb_Device_Def SET";
                        sqlQuery += " fridgeType=" + (int)DeviceToStore.fridgeType;
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    if (!string.IsNullOrEmpty(DeviceToStore.SerialFPMaster))
                    {
                        sqlQuery = null;
                        sqlQuery += "UPDATE tbo_Device_Def SET";
                        sqlQuery += " SerialFPMaster='" + DeviceToStore.SerialFPMaster + "'"; ;
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    if (!string.IsNullOrEmpty(DeviceToStore.SerialFPSlave))
                    {
                        sqlQuery = null;
                        sqlQuery += "UPDATE tbo_Device_Def SET";
                        sqlQuery += " SerialFPSlave='" + DeviceToStore.SerialFPSlave + "'";
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    if (!string.IsNullOrEmpty(DeviceToStore.comMasterReader))
                    {
                        sqlQuery = null;
                        sqlQuery += "UPDATE tbo_Device_Def SET";
                        sqlQuery += " comMasterReader='" + DeviceToStore.comMasterReader + "'";
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();


                        // update reader type
                        sqlQuery = null;
                        sqlQuery += "UPDATE tbo_Device_Def SET";
                        sqlQuery += " accessReaderType=" + (int)DeviceToStore.accessReaderType;
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();

                    }
                    if (!string.IsNullOrEmpty(DeviceToStore.comSlaveReader))
                    {
                        sqlQuery = null;
                        sqlQuery += "UPDATE tbo_Device_Def SET";
                        sqlQuery += " comSlaveReader='" + DeviceToStore.comSlaveReader + "'";
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();

                        // no need to update reader as master choose th type

                    }
                    if (!string.IsNullOrEmpty(DeviceToStore.comTempReader))
                    {
                        sqlQuery = null;
                        sqlQuery += "UPDATE tbo_Device_Def SET";
                        sqlQuery += " comTempReader='" + DeviceToStore.comTempReader + "'";
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }


                    if (bNetwork)
                    {
                        sqlQuery = null;
                        sqlQuery += "UPDATE tbo_Device_Def SET";
                        sqlQuery += " IP_Server ='" + DeviceToStore.IP_Server + "',";
                        sqlQuery += " Port_Server=" + DeviceToStore.Port_Server + ",";
                        sqlQuery += " bLocal=0 ";
                        sqlQuery += " WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }

                    ret = true;
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return ret;
            }
            private bool UpdateDevice(DeviceInfo DeviceToStore, bool bNetwork, string machineName = null)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "UPDATE tbo_Device_Def ";
                    sqlQuery += "SET DeviceName='" + DeviceToStore.DeviceName + "',";
                    sqlQuery += " IdDeviceType=" + (int)DeviceToStore.deviceType + ",";
                    if (!string.IsNullOrEmpty(DeviceToStore.SerialFPMaster))
                        sqlQuery += " SerialFPMaster='" + DeviceToStore.SerialFPMaster + "',";
                    if (!string.IsNullOrEmpty(DeviceToStore.SerialFPSlave))
                        sqlQuery += " SerialFPSlave='" + DeviceToStore.SerialFPSlave + "',";
                    if (!string.IsNullOrEmpty(DeviceToStore.comMasterReader))
                    {
                        sqlQuery += " comMasterReader='" + DeviceToStore.comMasterReader + "',";
                        sqlQuery += " accessReaderType=" + (int)DeviceToStore.accessReaderType + ",";
                    }
                    if (!string.IsNullOrEmpty(DeviceToStore.comSlaveReader))
                        sqlQuery += " comSlaveReader='" + DeviceToStore.comSlaveReader + "',";
                    if (!string.IsNullOrEmpty(DeviceToStore.comTempReader))
                    {
                        sqlQuery += " comTempReader='" + DeviceToStore.comTempReader + "',";
                    }
                    if (DeviceToStore.fridgeType != FridgeType.FT_UNKNOWN)
                    {
                        sqlQuery += " fridgeType=" + (int)DeviceToStore.fridgeType + ",";
                    }
                    if (bNetwork)
                    {
                        sqlQuery += " IP_Server ='" + DeviceToStore.IP_Server + "',";
                        sqlQuery += " Port_Server=" + DeviceToStore.Port_Server + ",";
                        sqlQuery += " bLocal=0,";
                    }
                    else
                        sqlQuery += " bLocal=" + DeviceToStore.bLocal.ToString() + ",";
                    sqlQuery += " Enabled=" + (int)DeviceToStore.enabled + " ";
                    sqlQuery += "WHERE SerialRFID='" + DeviceToStore.SerialRFID + "' ";
                    if (!string.IsNullOrEmpty(machineName))
                    {
                        sqlQuery += "AND BelongTo='" + machineName + "';";
                    }
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool UpdateNetworkDevice(string serialRFID, string IP_Remote, int port_Remote)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "UPDATE tbo_Device_Def ";
                    if (string.IsNullOrEmpty(IP_Remote))
                        sqlQuery += "SET IP_Server ='Null',";
                    else
                        sqlQuery += "SET IP_Server ='" + IP_Remote + "',";
                    sqlQuery += " Port_Server=" + port_Remote + " ";
                    sqlQuery += "WHERE SerialRFID='" + serialRFID + "';";

                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool DeleteDevice(DeviceInfo DeviceToStore)
            {

                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Device_Def ";
                    sqlQuery += "WHERE SerialRFID='" + DeviceToStore.SerialRFID + "';";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;                    

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
        #endregion
        #region alert
            public smtpInfo getSmtpInfo(bool onlyWhenActive)
            {
                smtpInfo smtp = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Mail_Smtp";
                    if (onlyWhenActive)
                        sqlQuery += " WHERE bActive=1";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            smtp = new smtpInfo();
                            smtp.smtp = rd["Smtp"].ToString().Trim().ToString();
                            smtp.port = Convert.ToInt32(rd["Port"].ToString().Trim().ToString());
                            smtp.sender = rd["Sender"].ToString().Trim().ToString();
                            smtp.login = rd["Login"].ToString().Trim().ToString();
                            smtp.pwd = rd["Pwd"].ToString().Trim().ToString();
                            smtp.bUseSSL = Convert.ToBoolean(Convert.ToInt32(rd["bUseSSL"].ToString().Trim().ToString()));
                            smtp.bActive = Convert.ToBoolean(Convert.ToInt32(rd["bActive"].ToString().Trim().ToString()));
                        }                        
                    }
                  
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return smtp;
            }
            public bool DeleteSMTP()
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Mail_Smtp ";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool AddSmtp(smtpInfo smtp)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO [tbo_Mail_Smtp] (Smtp,Port,Sender,Login,Pwd,bUseSSL,bActive) VALUES('";
                    sqlQuery += smtp.smtp + "',";
                    sqlQuery += smtp.port + ",'";
                    sqlQuery += smtp.sender + "','";
                    sqlQuery += smtp.login + "','";
                    sqlQuery += smtp.pwd + "',";
                    sqlQuery += Convert.ToInt32(smtp.bUseSSL) + ",";
                    sqlQuery += Convert.ToInt32(smtp.bActive) + ");";

                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool DeleteAlert(alertInfo alert)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Alert_List ";
                    sqlQuery += "WHERE AlertName='" + alert.AlertName + "'"; 
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                  
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool AddAlertInfo(alertInfo alert)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    if (alert.alertData == null)
                    {
                        sqlQuery += "INSERT INTO [tbo_Alert_List] (AlertName,RecipientList,CCRecipientList,BCCRecipientList,MailSubject,AlertMessage,bActive) VALUES ('";
                        sqlQuery += alert.AlertName + "','";
                        sqlQuery += alert.RecipientList + "','";
                        sqlQuery += alert.CCRecipientList + "','";
                        sqlQuery += alert.BCCRecipientList + "','";
                        sqlQuery += alert.MailSubject + "','";
                        sqlQuery += alert.AlertMessage + "',";
                        sqlQuery += Convert.ToInt32(alert.bActive) + ");";
                    }
                    else
                    {
                        sqlQuery += "INSERT INTO [tbo_Alert_List] (AlertName,RecipientList,CCRecipientList,BCCRecipientList,MailSubject,AlertMessage,bActive,AlertData) VALUES ('";
                        sqlQuery += alert.AlertName + "','";
                        sqlQuery += alert.RecipientList + "','";
                        sqlQuery += alert.CCRecipientList + "','";
                        sqlQuery += alert.BCCRecipientList + "','";
                        sqlQuery += alert.MailSubject + "','";
                        sqlQuery += alert.AlertMessage + "',";
                        sqlQuery += Convert.ToInt32(alert.bActive) + ",'";
                        sqlQuery += alert.alertData + "');";
                    }
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public int GetDoorOpenToLongTime()
            {
                int retData = 30; // default Value
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return retData;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT AlertData FROM tbo_Alert_List ";
                    sqlQuery += "WHERE AlertName='AT_Door_Open_Too_Long'";
                 
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            int.TryParse(rd[0].ToString().Trim().ToString(), out retData);
                        }                        
                    }
                    rd.Close();
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return retData;
            }
            public double GetMaxTempFridgeValue()
            {
                double retData = 12.0; // default Value
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return retData;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT AlertData FROM tbo_Alert_List ";
                    sqlQuery += "WHERE AlertName='AT_Max_Fridge_Temp'";

                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            string val = rd[0].ToString().Trim().ToString();
                            System.Globalization.CultureInfo culture;

                            if (val.Contains("."))
                            {
                                // Utilisation de InvariantCulture si présence du . comme séparateur décimal. 
                                culture = System.Globalization.CultureInfo.InvariantCulture;
                            }
                            else
                            {
                                // Utilisation de CurrentCulture sinon (utilisation de , comme séparateur décimal).
                                culture = System.Globalization.CultureInfo.CurrentCulture;
                            }

                            // Conversion de la chaîne en double, en utilisant la culture correspondante.            
                            double.TryParse(val, System.Globalization.NumberStyles.Number, culture, out retData);
                        }                        
                    }
                    rd.Close();
                    cmd.Dispose();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return retData;
            }
            public alertInfo getAlertInfo(AlertType alertType, bool bActive)
            {
                alertInfo alert = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Alert_List ";
                    sqlQuery += "WHERE AlertName='" + alertType.ToString();

                    if (bActive) sqlQuery += "' AND bActive=1";
                    else sqlQuery += "'";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            alert = new alertInfo();
                            alert.type = alertType;
                            alert.AlertName = rd["AlertName"].ToString().Trim().ToString();
                            alert.RecipientList = rd.IsDBNull(rd.GetOrdinal("RecipientList")) ? string.Empty : rd["RecipientList"].ToString().Trim();
                            alert.CCRecipientList = rd.IsDBNull(rd.GetOrdinal("CCRecipientList")) ? string.Empty : rd["CCRecipientList"].ToString().Trim();
                            alert.BCCRecipientList = rd.IsDBNull(rd.GetOrdinal("BCCRecipientList")) ? string.Empty : rd["BCCRecipientList"].ToString().Trim();
                            alert.MailSubject = rd.IsDBNull(rd.GetOrdinal("MailSubject")) ? string.Empty : rd["MailSubject"].ToString().Trim();
                            alert.AlertMessage = rd.IsDBNull(rd.GetOrdinal("AlertMessage")) ? string.Empty : rd["AlertMessage"].ToString().Trim();
                            alert.bActive = Convert.ToBoolean(Convert.ToInt32(rd["bActive"].ToString().Trim().ToString()));
                            alert.alertData = rd.IsDBNull(rd.GetOrdinal("AlertData")) ? string.Empty : rd["AlertData"].ToString().Trim();
                        }                       
                    }
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return alert;
            }
            public string GetColumnNameForAlert(AlertType alertType)
            {
                string columnName = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT ColumnName FROM tbo_Column_Info ";
                    sqlQuery += "WHERE AlertLink='" + alertType.ToString() + "'";
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            columnName = rd.IsDBNull(rd.GetOrdinal("ColumnName")) ? string.Empty : (string)rd["ColumnName"].ToString().Trim();
                        }                        
                    }                 
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return columnName;
            }
            public string GetColumnNameForWeight()
            {
                string columnName = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT ColumnName FROM tbo_Column_Info ";
                    sqlQuery += "WHERE AlertLink='WEIGHT'";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        rd.Read();
                        {
                            columnName = rd.IsDBNull(rd.GetOrdinal("ColumnName")) ? string.Empty : (string)rd["ColumnName"].ToString().Trim();
                        }
                    }
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return columnName;
            }
            public bool storeAlert(AlertType alertType, DeviceInfo di, UserClassTemplate utc, string alertData)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    DateTime gmtDate = DateTime.Now.ToUniversalTime();
                    string sqlQuery = null;
                    sqlQuery += "INSERT INTO tbo_Alert_History(AlertDate,SortedAlertDate,AlertName,SerialRFID,FirstName,LastName,AlertData) VALUES('";
                    sqlQuery += gmtDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                    sqlQuery += gmtDate.ToString("s") + "','";
                    sqlQuery += alertType.ToString() + "','";
                    sqlQuery += di.SerialRFID;
                    if (utc != null)
                    {
                        sqlQuery += "','";
                        sqlQuery += utc.firstName + "','";
                        sqlQuery += utc.lastName;
                    }
                    else
                    {
                        sqlQuery += "',' ',' ";
                    }
                    if (!string.IsNullOrEmpty(alertData))
                        sqlQuery += "','" + alertData;
                    else
                        sqlQuery += "',' ";
                    sqlQuery += "');";
                                      
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    
                }

                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
        #endregion
        #region Fridge
            public bool StoreTempFridge(string SerialRFID, tempInfo tempInfoToStore)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream();
                    bf.Serialize(mem, tempInfoToStore);
                    string idStream = Convert.ToBase64String(mem.ToArray());
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;

                    sqlQuery += "INSERT INTO tbo_Fridge_Temp(SerialRFID,Event_Date,Sorted_Event_Date,Mean_Value,Max_Value,Min_Value,Nb_Point,TempStream) VALUES('";
                    sqlQuery += SerialRFID + "','";
                    sqlQuery += tempInfoToStore.CreationDate.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "','";
                    sqlQuery += tempInfoToStore.CreationDate.ToString("s") + "','";
                    sqlQuery += tempInfoToStore.mean.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "','";
                    sqlQuery += tempInfoToStore.max.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "','";
                    sqlQuery += tempInfoToStore.min.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "','";
                    sqlQuery += tempInfoToStore.nbValueTemp + "','";
                    sqlQuery += idStream + "');";
                                        
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                   
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;

            }
            public tempInfo GetLastFridgeTemp(string serialRFID)
            {
                tempInfo retData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT TOP 1 * FROM tbo_Fridge_Temp ";
                    sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                    sqlQuery += "ORDER BY IdTemp DESC";

                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream mem = new MemoryStream(Convert.FromBase64String((string)rd["TempStream"].ToString().Trim()));
                        retData = (tempInfo)bf.Deserialize(mem);                        
                    }
                    rd.Close();                   

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return retData;
            }
            public bool GetLastFridgeTempfromxHours(string serialRFID, int nbHour, out ArrayList tempChamber, out ArrayList tempBottle)
            {
                bool ret = false;
                tempInfo retData = null;
                tempChamber = null;
                tempBottle = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT TOP " + nbHour + " * FROM tbo_Fridge_Temp ";
                    sqlQuery += "WHERE SerialRFID='" + serialRFID + "' ";
                    sqlQuery += "ORDER BY IdTemp DESC";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        tempChamber = new ArrayList();
                        tempBottle = new ArrayList();
                        ArrayList listClasstemp = new ArrayList();
                        while (rd.Read())
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            MemoryStream mem = new MemoryStream(Convert.FromBase64String((string)rd["TempStream"].ToString().Trim()));
                            retData = (tempInfo)bf.Deserialize(mem);
                            listClasstemp.Insert(0, retData);
                        }

                        foreach (tempInfo ti in listClasstemp)
                        {
                            if (ti.tempChamber != null)
                                foreach (KeyValuePair<int, double> pair in ti.tempChamber)
                                {
                                    tempChamber.Insert(0, pair.Value);
                                }
                            if (ti.tempBottle != null)
                                foreach (KeyValuePair<int, double> pair in ti.tempBottle)
                                {
                                    tempBottle.Insert(0, pair.Value);
                                }
                        }
                    }
                    ret = true;

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public string[] GetFridgeTempAfter(string serialRFID, string DateTimetoParse, int nbItem = int.MaxValue)
            {
                ArrayList retData = new ArrayList(); ;
                string[] strData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    //DateTime dt = DateTime.Parse(DateTimetoParse);
                    DateTime dt = DateTime.ParseExact(DateTimetoParse, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal);
                    
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT TOP " + nbItem + " TempStream FROM tbo_Fridge_Temp WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + serialRFID + "' ";
                    sqlQuery += "AND Sorted_Event_Date>'" + dt.ToString("s") + "' ";
                    sqlQuery += "ORDER BY Sorted_Event_Date ASC";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            retData.Add(rd[0].ToString().Trim().ToString());
                        }
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (retData.Count > 0)
                {
                    strData = new string[retData.Count];
                    int nIndex = 0;
                    foreach (string dt in retData)
                        strData[nIndex++] = dt;
                }
                return strData;
            }

            //  temp v2
            public PtTemp GetLastTempPoint(string SerialRFID, bool bFridgeOK)
            {
                PtTemp lastTemp = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    if (!bFridgeOK)
                    {
                        sqlQuery += "SELECT TOP 1 * FROM tbo_TempFridge ";
                        sqlQuery += "WHERE 1=1 ";
                        sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                        sqlQuery += "ORDER BY TemperatureDate DESC";
                    }
                    else
                    {
                        sqlQuery += "SELECT TOP 1 * FROM tbo_TempFridge ";
                        sqlQuery += "WHERE bFridgeOK=1 ";
                        sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                        sqlQuery += "ORDER BY TemperatureDate DESC";
                    }
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        lastTemp = new PtTemp();
                        lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                        lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                        lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                        lastTemp.bFridgeOK = (int)rd["bFridgeOK"];
                    }
                    rd.Close();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }


                return lastTemp;
            }
            public PtTemp[] GetPointFromDate(string SerialRFID, string date)
            {
                ArrayList retData = new ArrayList(); ;
                PtTemp[] PtData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_TempFridge WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "AND TemperatureDate>'" + date + "' ORDER BY TemperatureDate ASC ;";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            PtTemp lastTemp = new PtTemp();
                            lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                            lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                            lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                            lastTemp.bFridgeOK = (int)rd["bFridgeOK"];
                            retData.Add(lastTemp);
                        }
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (retData.Count > 0)
                {
                    PtData = new PtTemp[retData.Count];
                    int nIndex = 0;
                    foreach (PtTemp data in retData)
                        PtData[nIndex++] = data;
                }
                return PtData;
            }
            public List<PtTemp> GetPointBeforeDate(string SerialRFID, string date, int NbPt)
            {
                List<PtTemp> result = new List<PtTemp>();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;

                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM (SELECT TOP " + NbPt + "* FROM tbo_TempFridge WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "AND TemperatureDate<'" + date + "' ORDER BY TemperatureDate DESC) x ORDER BY TemperatureDate ASC";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            PtTemp lastTemp = new PtTemp();
                            lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                            lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                            lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                            lastTemp.bFridgeOK = (int)rd["bFridgeOK"];
                            result.Add(lastTemp);
                        }
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return result;
            }

            public PtTemp[] GetPointBetweenDate(string SerialRFID, string dateFrom, string dateTo)
            {
                ArrayList retData = new ArrayList(); ;
                PtTemp[] PtData = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {

                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_TempFridge WHERE 1=1 ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "' ";
                    sqlQuery += "AND TemperatureDate>'" + dateFrom + "' ";
                    sqlQuery += "AND TemperatureDate<'" + dateTo + "' ORDER BY TemperatureDate ASC ;";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            PtTemp lastTemp = new PtTemp();
                            lastTemp.TempAcqDate = rd["TemperatureDate"].ToString();
                            lastTemp.TempBottle = rd.IsDBNull(rd.GetOrdinal("TemperatureBottle")) ? null : (double?)rd["TemperatureBottle"];
                            lastTemp.TempChamber = rd.IsDBNull(rd.GetOrdinal("TemperatureChamber")) ? null : (double?)rd["TemperatureChamber"];
                            lastTemp.bFridgeOK = (int)rd["bFridgeOK"];
                            retData.Add(lastTemp);
                        }
                    }

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                if (retData.Count > 0)
                {
                    PtData = new PtTemp[retData.Count];
                    int nIndex = 0;
                    foreach (PtTemp data in retData)
                        PtData[nIndex++] = data;
                }
                return PtData;
            }
            public bool AddNewPtTemp(string SerialRFID, string date)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;

                    sqlQuery += "INSERT INTO tbo_TempFridge(TemperatureDate,SerialRFID) VALUES('";
                    sqlQuery += date + "','";
                    sqlQuery += SerialRFID + "')";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool AddNewPtTemp(string SerialRFID, string date, double tempBottle, double tempChamber)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;

                    sqlQuery += "INSERT INTO tbo_TempFridge(TemperatureDate,SerialRFID,TemperatureBottle,TemperatureChamber,bFridgeOK) VALUES('";
                    sqlQuery += date + "','";
                    sqlQuery += SerialRFID + "',";
                    sqlQuery += tempBottle.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
                    sqlQuery += tempChamber.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",1)";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool UpdatePtTemp(string SerialRFID, string date, double tempBottle, double tempChamber)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;

                    sqlQuery += "UPDATE tbo_TempFridge ";
                    sqlQuery += "SET TemperatureBottle=" + tempBottle.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
                    sqlQuery += "TemperatureChamber=" + tempChamber.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
                    sqlQuery += "bFridgeOK=1 ";
                    sqlQuery += "WHERE TemperatureDate='" + date + "' ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "';";


                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
            public bool IsPointExist(string SerialRFID, string date, out bool bValueNull)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                bValueNull = false;
                try
                {

                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_TempFridge WHERE 1=1 ";
                    sqlQuery += "AND TemperatureDate='" + date + "' ";
                    sqlQuery += "AND SerialRFID='" + SerialRFID + "';";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        bValueNull = !Convert.ToBoolean((int)rd["bFridgeOK"]);
                        ret = true;
                    }
                    rd.Close();
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.

                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }

            public string GetLastReportDate()
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                string lastReportDate = String.Empty;

                try
                {
                    if (globalCon == null) return lastReportDate; // str empty
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = String.Format("SELECT ReportDate FROM tbo_Daily_TempReport_History;");

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (!rd.HasRows) return lastReportDate; // str empty
                    rd.Read();
                    lastReportDate = rd["ReportDate"].ToString();
                }
                catch (Exception exp)
                {
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }

                return lastReportDate;
            }
            public void UpdateLastReportDate(string dateAsString)
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;

                try
                {
                    if (globalCon == null) return;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = String.Format("UPDATE tbo_Daily_TempReport_History set ReportDate = '{0}';", dateAsString);

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    if (cmd.ExecuteNonQuery() == 0) // no date have been inserted yet : let's create a new one
                    {
                        sqlQuery = String.Format("INSERT INTO tbo_Daily_TempReport_History VALUES('{0}');", dateAsString);

                        cmd.Transaction = trans;
                        cmd.Connection = globalCon;
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception exp)
                {
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
            }

           
        #endregion
        #region Formule
            public FormuleData getFormuleInfo()
            {
                FormuleData retFormule = null;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return null;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "SELECT * FROM tbo_Formula;";

                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        rd.Read();
                        retFormule = new FormuleData();

                        retFormule.Title = (string)rd["Title"].ToString().Trim();
                        retFormule.Formule = (string)rd["Formulae"].ToString().Trim();
                        retFormule.Enable = Convert.ToInt32(rd["Enable"].ToString().Trim().ToString());                      
                    }                 
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return retFormule;
            }
            public bool setFormuleInfo(FormuleData fd)
            {
                bool ret = false;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    string sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Formula;";
                    
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    int rowAffected = cmd.ExecuteNonQuery();

                    sqlQuery = string.Empty;

                    sqlQuery += "INSERT INTO tbo_Formula(Title,Formulae,Enable) VALUES('";
                    sqlQuery += fd.Title + "','";
                    sqlQuery += fd.Formule + "',";
                    sqlQuery += fd.Enable.ToString() + ");";


                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0) ret = true;
                    
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
        #endregion
        #region import

            public DataSet ImportCSVToDS(string filename)
            {
                DataTable dt = null;
                DataSet ds = null;

                try
                {
                    Hashtable ColumnInfo = GetColumnInfo();
                    Hashtable ColumnCSVInfo = GetColumnCSVInfo();

                    string CSVFilePathName = filename;
                    string[] Lines = File.ReadAllLines(CSVFilePathName);
                    string[] Fields;
                    Fields = Lines[0].Split(new char[] { ',' });
                    int Cols = Fields.GetLength(0);
                    dt = new DataTable();
                    //1st row must be column names; force lower case to ensure matching later on.
                    for (int i = 0; i < Cols; i++)
                    {
                        for (int loop = 0; loop < ColumnInfo.Count; loop++)
                        {
                            if (Fields[i].Equals(ColumnCSVInfo[loop])) Fields[i] = ColumnInfo[loop].ToString();
                        }

                        dt.Columns.Add(Fields[i], typeof(string));
                    }
                    DataRow Row;
                    for (int i = 1; i < Lines.GetLength(0); i++)
                    {
                        Fields = Lines[i].Split(new char[] { ',' });
                        Row = dt.NewRow();
                        for (int f = 0; f < Cols; f++)
                        {
                            if (Fields[f].Contains("\""))
                            {
                                int start = Fields[f].IndexOf("\"") + 1;
                                int end = Fields[f].LastIndexOf("\"");
                                int lenght = end - start;
                                Fields[f] = Fields[f].Substring(start, lenght);
                            }
                            Row[f] = Fields[f];
                        }
                        dt.Rows.Add(Row);
                    }

                    ds = new DataSet();
                    ds.Tables.Add(dt);
                }
                catch (IOException exp)
                {
                    MessageBox.Show("The File is already opened by another application \r\n Please close this application before try to import!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }

                return ds;

            }
            public DataSet ImportExcelToDS(string filename)
            {
                OleDbConnection objConn;
                DataTable dt = null;
                DataSet ds = null;
                string sheetName = "Import";
                String[] excelSheets = null;
                try
                {
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                     "Data Source=" + filename + ";" +
                                     "Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";


                    objConn = new OleDbConnection(strConn);
                    objConn.Open();
                    dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });

                    if (dt != null)
                    {


                        excelSheets = new String[dt.Rows.Count];
                        int i = 0;

                        // Add the sheet name to the string array.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[i] = row["TABLE_NAME"].ToString();
                            i++;
                        }
                    }

                    sheetName = excelSheets[0];
                    objConn.Close();
                    // }


                    OleDbDataAdapter da;
                    ds = new DataSet();
                    if (sheetName.Contains("$"))
                        da = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "]", strConn);
                    else
                        da = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "$]", strConn);
                    da.Fill(ds);
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                return ds;
            }
            public DataSet ImportExcelToDS(string filename, string sheetName)
            {
                DataSet ds = null;
                try
                {
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                     "Data Source=" + filename + ";" +
                                     "Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";


                    OleDbDataAdapter da;
                    ds = new DataSet();
                    if (sheetName.Contains("$"))
                        da = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "]", strConn);
                    else
                        da = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "$]", strConn);
                    da.Fill(ds);
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                return ds;
            }
            #endregion
        #region export
            public static void Export(DataTable dt, string filepath, string tablename)
            {
                string connString;
                if (!DataClass.ExcelInfo.isExcel2010_64bits())
                {
                    //  OLEDB 4.0 pas inscrit en 64 bits
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                          filepath + ";Extended Properties=Excel 8.0;";
                }
                else
                {
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            filepath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
                }
                try
                {
                    using (OleDbConnection con = new OleDbConnection(connString))
                    {
                        con.Open();
                        StringBuilder strSQL = new StringBuilder();
                        strSQL.Append("CREATE TABLE ").Append("[" + tablename + "]");
                        strSQL.Append("(");
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            strSQL.Append("[" + dt.Columns[i].ColumnName.Replace(".", "#") + "] text,");
                        }
                        strSQL = strSQL.Remove(strSQL.Length - 1, 1);
                        strSQL.Append(")");

                        OleDbCommand cmd = new OleDbCommand(strSQL.ToString(), con);
                        cmd.ExecuteNonQuery();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            strSQL.Length = 0;
                            StringBuilder strfield = new StringBuilder();
                            StringBuilder strvalue = new StringBuilder();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                strfield.Append("[" + dt.Columns[j].ColumnName.Replace(".", "#") + "]");
                                strvalue.Append("'" + dt.Rows[i][j].ToString() + "'");
                                if (j != dt.Columns.Count - 1)
                                {
                                    strfield.Append(",");
                                    strvalue.Append(",");
                                }
                                else
                                {
                                }
                            }
                            cmd.CommandText = strSQL.Append(" insert into [" + tablename + "]( ")
                                .Append(strfield.ToString())
                                .Append(") values (").Append(strvalue).Append(")").ToString();
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }

                }
                catch (Exception ex)
                {
                    ErrorMessage.ExceptionMessageBox.Show(ex);
                }
            }
            public static void ExportToTemplate(DataTable dt, string filepath, string tablename)
            {
                //excel 2003
                //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                //     filepath + ";Extended Properties=Excel 8.0;";
                //Excel 2007



                string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                       filepath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
                try
                {
                    using (OleDbConnection con = new OleDbConnection(connString))
                    {
                        con.Open();
                        StringBuilder strSQL = new StringBuilder();
                        OleDbCommand cmd = new OleDbCommand(strSQL.ToString(), con);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            strSQL.Length = 0;
                            StringBuilder strfield = new StringBuilder();
                            StringBuilder strvalue = new StringBuilder();
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                strfield.Append("[" + dt.Columns[j].ColumnName + "]");
                                strvalue.Append("'" + dt.Rows[i][j].ToString() + "'");
                                if (j != dt.Columns.Count - 1)
                                {
                                    strfield.Append(",");
                                    strvalue.Append(",");
                                }
                                else
                                {
                                }
                            }
                            cmd.CommandText = strSQL.Append(" insert into [" + tablename + "]( ")
                                .Append(strfield.ToString())
                                .Append(") values (").Append(strvalue).Append(")").ToString();
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }

                }
                catch (Exception ex)
                {
                    ErrorMessage.ExceptionMessageBox.Show(ex);
                }
            }
            #endregion
        #region stat
            public int GetInventoryCount(string serialRFID = null, string firstName = null, string lastName = null, string dateFrom = null, string dateTo = null)
            {
                int ret = -1;
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return ret;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();
                    string sqlQuery = null;

                    sqlQuery += "SELECT COUNT(*) FROM tbo_Scan_Event WHERE 1=1 ";

                    if (!string.IsNullOrEmpty(serialRFID))
                        sqlQuery += "AND [SerialRFID]='" + serialRFID + "' ";

                    if ((!string.IsNullOrEmpty(firstName)) && (!string.IsNullOrEmpty(lastName)))
                    {
                        sqlQuery += "AND [FirstName]='" + firstName + "' ";
                        sqlQuery += "AND [LastName]='" + lastName + "' ";
                    }

                    if (dateFrom != null)
                    {
                        DateTime dtFrom = DateTime.ParseExact(dateFrom, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                        sqlQuery += "AND [SortedEventDate]>'" + dtFrom.ToUniversalTime().ToString("s") + "' ";
                    }

                    if (dateTo != null)
                    {
                        DateTime dtTo = DateTime.ParseExact(dateTo, "yyyy-MM-dd HH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
                        sqlQuery += "AND [SortedEventDate]<'" + dtTo.ToUniversalTime().ToString("s") + "' ";
                    }
                   
                    cmd.Transaction = trans;
                    cmd.Connection = globalCon;
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    ret = Convert.ToInt32(cmd.ExecuteScalar());                  

                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }
        #endregion
        #region utilSQl
           
            public bool cleanDB(bool bDeleteColum = false)
            {
                bool ret = false;
                string sqlQuery;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = globalCon;
                cmd.Transaction = trans;
                
                SqlDataReader rd = null;
                try
                {
                    if (globalCon == null) return false;
                    if (globalCon.State != System.Data.ConnectionState.Open) OpenDB();

                    if (bDeleteColum)
                    {
                        sqlQuery = null;
                        sqlQuery += "DELETE FROM tbo_Column_Info ";

                        cmd.Connection = globalCon;
                        cmd.CommandText = sqlQuery;
                        cmd.CommandType = System.Data.CommandType.Text;

                        cmd.ExecuteNonQuery();
                    }

                    sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Product_Def ";

                   
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();

                    sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Tag_Event ";
                    
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;

                    cmd.ExecuteNonQuery();

                    sqlQuery = null;
                    sqlQuery += "DELETE FROM tbo_Scan_Event ";
                    
                    cmd.CommandText = sqlQuery;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                    ret = true;
                }
                catch (Exception exp)
                {
                    // On affiche l'erreur.
                    ErrorMessage.ExceptionMessageBox.Show(exp);
                }
                finally
                {
                    if (rd != null) rd.Close();
                    if (cmd != null) cmd.Dispose();
                }
                return ret;
            }

          
            public Hashtable SqlUpdate { get { return sqlUpdate; } }
            public readonly Hashtable sqlUpdate = new Hashtable
       {
            #region table version
                       { 1 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Db_Info' AND Xtype='U')
                               CREATE TABLE tbo_Db_Info
                               (Criteria   VARCHAR(255)      NOT NULL PRIMARY KEY,
                                Value     INTEGER,	   
                                Modified  DATETIME)"},
            #endregion
            #region table tbo_Product_Def
                       { 2 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Product_Def' AND Xtype='U')
                                CREATE TABLE [tbo_Product_Def] (
                                [TagUID] VARCHAR(20)  NOT NULL PRIMARY KEY,
                                [ProductRef] VARCHAR(50),
                                [ProductInfo1] VARCHAR(75),
                                [ProductInfo2] VARCHAR(75),
                                [ProductInfo3] VARCHAR(75),
                                [ProductInfo4] VARCHAR(75),
                                [ProductInfo5] VARCHAR(75),
                                [ProductInfo6] VARCHAR(75),
                                [ProductInfo7] VARCHAR(75),
                                [ProductInfo8] VARCHAR(75),
                                [ProductInfo9] VARCHAR(75),
                                [ProductInfo10] VARCHAR(75),
                                [ProductInfo11] VARCHAR(75),
                                [ProductInfo12] VARCHAR(75),
                                [ProductInfo13] VARCHAR(75),
                                [ProductInfo14] VARCHAR(75),
                                [ProductInfo15] VARCHAR(75),
                                [ProductInfo16] VARCHAR(75),
                                [ProductInfo17] VARCHAR(75),
                                [ProductInfo18] VARCHAR(75),
                                [ProductInfo19] VARCHAR(75),
                                [ProductInfo20] VARCHAR(75),
                                [ProductInfo21] VARCHAR(75),
                                [ProductInfo22] VARCHAR(75),
                                [ProductInfo23] VARCHAR(75),
                                [ProductInfo24] VARCHAR(75),
                                [ProductInfo25] VARCHAR(75),
                                [ProductInfo26] VARCHAR(75),
                                [ProductInfo27] VARCHAR(75),
                                [ProductInfo28] VARCHAR(75),
                                [ProductInfo29] VARCHAR(75),
                                [ProductInfo30] VARCHAR(75),
                                [ProductInfo31] VARCHAR(75),
                                [ProductInfo32] VARCHAR(75),
                                [ProductInfo33] VARCHAR(75),
                                [ProductInfo34] VARCHAR(75), 
                                [ProductInfo35] VARCHAR(75), 
                                [ProductInfo36] VARCHAR(75),
                                [ProductInfo37] VARCHAR(75),
                                [ProductInfo38] VARCHAR(75),
                                [ProductInfo39] VARCHAR(75),
                                [ProductInfo40] VARCHAR(75),
                                [ProductInfo41] VARCHAR(75),
                                [ProductInfo42] VARCHAR(75),
                                [ProductInfo43] VARCHAR(75),
                                [ProductInfo44] VARCHAR(75),
                                [ProductInfo45] VARCHAR(75),
                                [ProductInfo46] VARCHAR(75),
                                [ProductInfo47] VARCHAR(75),
                                [ProductInfo48] VARCHAR(75),
                                [ProductInfo49] VARCHAR(75),
                                [ProductInfo50] VARCHAR(75),
                                [ProductInfo51] VARCHAR(75),
                                [ProductInfo52] VARCHAR(75),
                                [ProductInfo53] VARCHAR(75), 
                                [ProductInfo54] VARCHAR(75),
                                [ProductInfo55] VARCHAR(75), 
                                [ProductInfo56] VARCHAR(75), 
                                [ProductInfo57] VARCHAR(75), 
                                [ProductInfo58] VARCHAR(75), 
                                [ProductInfo59] VARCHAR(75),
                                [ProductInfo60] VARCHAR(75),
                                [ProductInfo61] VARCHAR(75),
                                [ProductInfo62] VARCHAR(75),
                                [ProductInfo63] VARCHAR(75),
                                [ProductInfo64] VARCHAR(75),
                                [ProductInfo65] VARCHAR(75),
                                [ProductInfo66] VARCHAR(75), 
                                [ProductInfo67] VARCHAR(75), 
                                [ProductInfo68] VARCHAR(75),
                                [ProductInfo69] VARCHAR(75), 
                                [ProductInfo70] VARCHAR(75), 
                                [ProductInfo71] VARCHAR(75),
                                [ProductInfo72] VARCHAR(75), 
                                [ProductInfo73] VARCHAR(75),
                                [ProductInfo74] VARCHAR(75),
                                [ProductInfo75] VARCHAR(75),
                                [ProductInfo76] VARCHAR(75),
                                [ProductInfo77] VARCHAR(75), 
                                [ProductInfo78] VARCHAR(75), 
                                [ProductInfo79] VARCHAR(75),
                                [ProductInfo80] VARCHAR(75),
                                [ProductInfo81] VARCHAR(75),
                                [ProductInfo82] VARCHAR(75),
                                [ProductInfo83] VARCHAR(75), 
                                [ProductInfo84] VARCHAR(75),
                                [ProductInfo85] VARCHAR(75),
                                [ProductInfo86] VARCHAR(75),
                                [ProductInfo87] VARCHAR(75), 
                                [ProductInfo88] VARCHAR(75),
                                [ProductInfo89] VARCHAR(75), 
                                [ProductInfo90] VARCHAR(75),
                                [ProductInfo91] VARCHAR(75),
                                [ProductInfo92] VARCHAR(75),
                                [ProductInfo93] VARCHAR(75),
                                [ProductInfo94] VARCHAR(75), 
                                [ProductInfo95] VARCHAR(75), 
                                [ProductInfo96] VARCHAR(75), 
                                [ProductInfo97] VARCHAR(75), 
                                [ProductInfo98] VARCHAR(75), 
                                [ProductInfo99] VARCHAR(75), 
                                [ProductInfo100] VARCHAR(75))"},
      

            #endregion
            #region table tbo_Product_History
            { 3 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Product_History' AND Xtype='U')
                    CREATE TABLE [tbo_Product_History] (
                    [EnterDate] DATETIME,
                    [SortedEnterDate] VARCHAR(50),
                    [TagUID] VARCHAR(20)  NOT NULL,
                    [ProductRef] VARCHAR(50),
                    [ProductInfo1] VARCHAR(75),
                    [ProductInfo2] VARCHAR(75),
                    [ProductInfo3] VARCHAR(75),
                    [ProductInfo4] VARCHAR(75),
                    [ProductInfo5] VARCHAR(75),
                    [ProductInfo6] VARCHAR(75),
                    [ProductInfo7] VARCHAR(75),
                    [ProductInfo8] VARCHAR(75),
                    [ProductInfo9] VARCHAR(75),
                    [ProductInfo10] VARCHAR(75),
                    [ProductInfo11] VARCHAR(75),
                    [ProductInfo12] VARCHAR(75),
                    [ProductInfo13] VARCHAR(75),
                    [ProductInfo14] VARCHAR(75),
                    [ProductInfo15] VARCHAR(75),
                    [ProductInfo16] VARCHAR(75),
                    [ProductInfo17] VARCHAR(75),
                    [ProductInfo18] VARCHAR(75),
                    [ProductInfo19] VARCHAR(75),
                    [ProductInfo20] VARCHAR(75),
                    [ProductInfo21] VARCHAR(75),
                    [ProductInfo22] VARCHAR(75),
                    [ProductInfo23] VARCHAR(75),
                    [ProductInfo24] VARCHAR(75),
                    [ProductInfo25] VARCHAR(75),
                    [ProductInfo26] VARCHAR(75),
                    [ProductInfo27] VARCHAR(75),
                    [ProductInfo28] VARCHAR(75),
                    [ProductInfo29] VARCHAR(75),
                    [ProductInfo30] VARCHAR(75),
                    [ProductInfo31] VARCHAR(75),
                    [ProductInfo32] VARCHAR(75),
                    [ProductInfo33] VARCHAR(75),
                    [ProductInfo34] VARCHAR(75), 
                    [ProductInfo35] VARCHAR(75), 
                    [ProductInfo36] VARCHAR(75),
                    [ProductInfo37] VARCHAR(75),
                    [ProductInfo38] VARCHAR(75),
                    [ProductInfo39] VARCHAR(75),
                    [ProductInfo40] VARCHAR(75),
                    [ProductInfo41] VARCHAR(75),
                    [ProductInfo42] VARCHAR(75),
                    [ProductInfo43] VARCHAR(75),
                    [ProductInfo44] VARCHAR(75),
                    [ProductInfo45] VARCHAR(75),
                    [ProductInfo46] VARCHAR(75),
                    [ProductInfo47] VARCHAR(75),
                    [ProductInfo48] VARCHAR(75),
                    [ProductInfo49] VARCHAR(75),
                    [ProductInfo50] VARCHAR(75),
                    [ProductInfo51] VARCHAR(75),
                    [ProductInfo52] VARCHAR(75),
                    [ProductInfo53] VARCHAR(75), 
                    [ProductInfo54] VARCHAR(75),
                    [ProductInfo55] VARCHAR(75), 
                    [ProductInfo56] VARCHAR(75), 
                    [ProductInfo57] VARCHAR(75), 
                    [ProductInfo58] VARCHAR(75), 
                    [ProductInfo59] VARCHAR(75),
                    [ProductInfo60] VARCHAR(75),
                    [ProductInfo61] VARCHAR(75),
                    [ProductInfo62] VARCHAR(75),
                    [ProductInfo63] VARCHAR(75),
                    [ProductInfo64] VARCHAR(75),
                    [ProductInfo65] VARCHAR(75),
                    [ProductInfo66] VARCHAR(75), 
                    [ProductInfo67] VARCHAR(75), 
                    [ProductInfo68] VARCHAR(75),
                    [ProductInfo69] VARCHAR(75), 
                    [ProductInfo70] VARCHAR(75), 
                    [ProductInfo71] VARCHAR(75),
                    [ProductInfo72] VARCHAR(75), 
                    [ProductInfo73] VARCHAR(75),
                    [ProductInfo74] VARCHAR(75),
                    [ProductInfo75] VARCHAR(75),
                    [ProductInfo76] VARCHAR(75),
                    [ProductInfo77] VARCHAR(75), 
                    [ProductInfo78] VARCHAR(75), 
                    [ProductInfo79] VARCHAR(75),
                    [ProductInfo80] VARCHAR(75),
                    [ProductInfo81] VARCHAR(75),
                    [ProductInfo82] VARCHAR(75),
                    [ProductInfo83] VARCHAR(75), 
                    [ProductInfo84] VARCHAR(75),
                    [ProductInfo85] VARCHAR(75),
                    [ProductInfo86] VARCHAR(75),
                    [ProductInfo87] VARCHAR(75), 
                    [ProductInfo88] VARCHAR(75),
                    [ProductInfo89] VARCHAR(75), 
                    [ProductInfo90] VARCHAR(75),
                    [ProductInfo91] VARCHAR(75),
                    [ProductInfo92] VARCHAR(75),
                    [ProductInfo93] VARCHAR(75),
                    [ProductInfo94] VARCHAR(75), 
                    [ProductInfo95] VARCHAR(75), 
                    [ProductInfo96] VARCHAR(75), 
                    [ProductInfo97] VARCHAR(75), 
                    [ProductInfo98] VARCHAR(75), 
                    [ProductInfo99] VARCHAR(75), 
                    [ProductInfo100] VARCHAR(75))"},
      

            #endregion
            #region column
        
           { 4 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Column_Info' AND Xtype='U')
                    CREATE TABLE [tbo_Column_Info](
                    [IdColumn] INTEGER PRIMARY KEY NOT NULL,
                    [ColumnName] VARCHAR(50) NULL,
                    [ColumnCSVName] VARCHAR(50)  NULL,
                    [ColumnType] VARCHAR(50)  NULL, 
                    [AlertLink] VARCHAR(50) DEFAULT 'NONE', 
                    [ColumnToExport] INTEGER DEFAULT 1, 
                    [DoSum] INTEGER DEFAULT 0)"},
            { 5 , @"INSERT INTO [tbo_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) VALUES ('0','TagUID','TagUID','UINT64')"},
            { 6 , @"INSERT INTO [tbo_Column_Info] ([IdColumn],[ColumnName],[ColumnCSVName],[ColumnType]) VALUES ('1','LotID','LotID','STRING')"},                                               
            #endregion
            #region alert
            { 7 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Alert_History' AND Xtype='U')
                    CREATE TABLE [tbo_Alert_History] (
                    [AlertListID] INTEGER  PRIMARY KEY IDENTITY(1,1),
                    [AlertDate] DATETIME NULL,
                    [SortedAlertDate] VARCHAR(50) NULL, 
                    [AlertName] VARCHAR(50) NULL, 
                    [SerialRFID] VARCHAR(50) NULL,
                    [FirstName] VARCHAR(50) NULL ,
                    [LastName] VARCHAR(50) NULL,
                    [AlertData] VARCHAR(50) NULL )"},
            { 8 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Alert_List' AND Xtype='U')
                    CREATE TABLE [tbo_Alert_List](
                    [AlertListID] INTEGER  PRIMARY KEY IDENTITY(1,1), 
                    [AlertName] VARCHAR(50) NULL, 
                    [RecipientList] VARCHAR(1024) NULL,
                    [AlertMessage] VARCHAR(2048) NULL ,
                    [MailSubject] VARCHAR(512) NULL , 
                    [CCRecipientList] VARCHAR(512) NULL,
                    [BCCRecipientList] VARCHAR(512) NULL, 
                    [bActive] INTEGER DEFAULT 0, 
                    [AlertData] VARCHAR(255)  NULL)"},
            { 9 ,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Power_Cut','A power cut alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','POWER CUT ALERT ON [READERNAME]',0)" },
            { 10,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Usb_Unplug','A usb cable unplug alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','USB CABLE UNPLUG ALERT ON [READERNAME]',0)" },
            { 11,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive],[AlertData]) VALUES ('AT_Door_Open_Too_Long','A door opened too long alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','DOOR OPEN TOO LONG ALERT ON [READERNAME]',0,30)" },
            { 12,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Finger_Alert','A finger alert alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','FINGER ALERT ALERT ON [READERNAME]',0)" },
            { 13,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Remove_Too_Many_Items','A remove too many items in number alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','REMOVE TOO MANY ITEMS ALERT ON [READERNAME]',0)" },
            { 14,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Limit_Value_Exceed','A remove too many items in value alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE] from user [USERNAME]','REMOVE TOO MANY VALUE ALERT ON [READERNAME]',0)" },
            { 15,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Move_Sensor','A move sensor alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','MOVE SENSOR ALERT ON [READERNAME]',0)" }, 
            { 16,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive],[AlertData]) VALUES ('AT_Max_Fridge_Temp','A max temperature alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','MAX TEMPERATURE ALERT ON [READERNAME]',0,'10')" }, 
            { 17,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],[bActive]) VALUES ('AT_Remove_Tag_Max_Time','A remove tag max allowed time alert was detected on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','MAX TEMPERATURE ALERT ON [READERNAME]',0)" }, 
            { 18,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Stock_Limit','A limit stock alert has occurs on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','STOCK LIMIT ALERT ON [READERNAME]',0)" }, 
            { 19,  "INSERT INTO [tbo_Alert_List] ([AlertName],[AlertMessage],[MailSubject],bActive) VALUES ('AT_Bad_Blood_Patient','Bad blood bags removed for patient alert has occurs on [READERNAME] (S/N:[READERSERIAL])\nthe [DATE]','BAD BLOOD BAG REMOVED FOR PATIENT ALERT ON [READERNAME]',0)" }, 
            { 20,  @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Mail_Smtp' AND Xtype='U')
                    CREATE TABLE [tbo_Mail_Smtp](
                    [MailID] INTEGER  PRIMARY KEY  IDENTITY(1,1), 
                    [Smtp] VARCHAR(50)  NULL,
                    [Port] INTEGER  NULL,
                    [Sender] VARCHAR(255)  NULL,
                    [Login] VARCHAR(100)  NULL,
                    [Pwd] VARCHAR(100)  NULL, 
                    [bUseSSL] INTEGER DEFAULT 0, 
                    [bActive] INTEGER DEFAULT 0)"},
            { 21 , "INSERT INTO [tbo_Mail_Smtp] (Smtp,Port,Sender,Login,Pwd,bUseSSL,bActive) VALUES('smtp.gmail.com',587,'spacecode.rfid@gmail.com','spacecode.rfid@gmail.com','rfid123456',1,0)"},
            #endregion
            #region device
            { 22 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Device_Def' AND Xtype='U')
                    CREATE TABLE [tbo_Device_Def] (
                    [IdReaderDef] INTEGER PRIMARY KEY IDENTITY(1,1), 
                    [DeviceName] VARCHAR(25) NULL, 
                    [IdDeviceType] INTEGER DEFAULT 0, 
                    [SerialRFID] VARCHAR(25) NULL, 
                    [SerialFPMaster] VARCHAR(50) NULL, 
                    [SerialFPSlave] VARCHAR(50) NULL, 
                    [bLocal] INTEGER, 
                    [IP_Server] VARCHAR(20) NULL, 
                    [Port_Server] INTEGER DEFAULT 0, 
                    [Enabled] INTEGER DEFAULT 0, 
                    [comTempReader] VARCHAR(10)  NULL, 
                    [comMasterReader] VARCHAR(10)  NULL, 
                    [comSlaveReader] VARCHAR(10)  NULL, 
                    [accessReaderType] INTEGER DEFAULT 1, 
                    [fridgeType] INTEGER DEFAULT 0,
                    [BelongTo] VARCHAR(50) NULL)"},
            { 23 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Fridge_Temp' AND Xtype='U')
                    CREATE TABLE [tbo_Fridge_Temp](
                    [IdTemp] INTEGER PRIMARY KEY IDENTITY(1,1), 
                    [SerialRFID] VARCHAR(20) NOT NULL , 
                    [Event_Date] DATETIME, 
                    [Sorted_Event_Date] VARCHAR(50), 
                    [Mean_Value] FLOAT, 
                    [Max_Value] FLOAT, 
                    [Min_Value] FLOAT, 
                    [Nb_Point] INTEGER, 
                    [TempStream] NVARCHAR(max))"},
            #endregion
            #region formule
            { 24 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Formula' AND Xtype='U')
                    CREATE TABLE [tbo_Formula](
                    [IdFormule] INTEGER PRIMARY KEY  IDENTITY(1,1), 
                    [Title] VARCHAR(255), 
                    [Formulae] VARCHAR(255), 
                    [Enable] INTEGER DEFAULT 0)"},
            #endregion
            #region group
            { 25 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Group_Def' AND Xtype='U')
                    CREATE TABLE [tbo_Group_Def](
                    [TagGroup] VARCHAR(20) PRIMARY KEY ,
                    [GroupRef] VARCHAR(50),
                    [GroupInfo] VARCHAR(255),
                    [Criteria] VARCHAR(255))"},
            { 26 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Group_Link' AND Xtype='U')
                    CREATE TABLE [tbo_Group_Link](
                    [IdLink] INTEGER  PRIMARY KEY ,
                    [GroupTagRef] VARCHAR(50)  NULL,
                    [ProductTagRef] VARCHAR(50)  NULL)"},
            #endregion
            #region image
            { 27 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Image' AND Xtype='U')
                    CREATE TABLE [tbo_Image](
                    [ImageID] INTEGER PRIMARY KEY IDENTITY(1,1),
                    [ImageName] VARCHAR(50) NULL,
                    [ImageByteArray]  VARBINARY (max)  NULL)"},            
            { 28 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Image_Link' AND Xtype='U')
                    CREATE TABLE [tbo_Image_Link](
                    [LotIDorTagID] VARCHAR(50) PRIMARY KEY ,
                    [ImageName] VARCHAR(50) NOT NULL)"},
            #endregion
            #region inventory
            { 29, @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Scan_Event' AND Xtype='U')
                    CREATE TABLE [tbo_Scan_Event] (
                    [IdScanEvent] INTEGER PRIMARY KEY IDENTITY(1,1), 
                    [SerialRFID] VARCHAR(20), 
                    [bUSerScan] INTEGER, 
                    [FirstName] VARCHAR(50), 
                    [LastName] VARCHAR(50), 
                    [EventDate] DATETIME, 
                    [SortedEventDate] VARCHAR(25), 
                    [NbTagAll] INTEGER, 
                    [NbTagInReader] INTEGER, 
                    [NbTagAdded] INTEGER, 
                    [NbTagRemoved] INTEGER, 
                    [InventoryStream] NVARCHAR(max))"},
            { 30 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Tag_Event' AND Xtype='U')
                    CREATE TABLE [tbo_Tag_Event] (
                    [IdTagEvent] INTEGER PRIMARY KEY IDENTITY(1,1), 
                    [IdScanEvent] INTEGER, 
                    [TagUID] VARCHAR(20), 
                    [IdTagEventType] INTEGER)"},
            #endregion
            #region user
            { 31 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_User_Def' AND Xtype='U')
                    CREATE TABLE [tbo_User_Def] (
                    [IdUserDef] INTEGER PRIMARY KEY IDENTITY(1,1), 
                    [FirstName] VARCHAR(50), 
                    [LastName] VARCHAR(50), 
                    [FPTemplate] NVARCHAR(max), 
                    [FingerAlert] INTEGER DEFAULT -1, 
                    [MaxRemoveItem] INTEGER DEFAULT 0, 
                    [MaxRemoveValue] FLOAT DEFAULT 0.0)"},
            { 32 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_User_Grant' AND Xtype='U')
                    CREATE TABLE [tbo_User_Grant] (
                    [N°] INTEGER PRIMARY KEY IDENTITY(1,1), 
                    [FirstName] VARCHAR(50), 
                    [LastName] VARCHAR(50), 
                    [SerialRFID] VARCHAR(50), 
                    [BelongTo] VARCHAR(50))"},
            {33 , @"ALTER TABLE [tbo_User_Grant] ADD [DoorGranted] INTEGER NOT NULL DEFAULT (3)"}, 
          
          

            #endregion
            {34 , @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_TempFridge' AND Xtype='U')
                    CREATE TABLE [tbo_TempFridge] (
                    [IdTemp] INTEGER PRIMARY KEY IDENTITY(1,1),
                    [TemperatureDate] VARCHAR(20)  NOT NULL,
                    [SerialRFID] VARCHAR(25) NULL,
                    [TemperatureBottle] FLOAT, 
                    [TemperatureChamber] FLOAT, 
                    [bFridgeOK] INTEGER DEFAULT 0)"},
            { 35,  @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbo_Daily_TempReport_History' AND Xtype='U')
                        CREATE TABLE [tbo_Daily_TempReport_History] (
                        [ReportDate] VARCHAR(20) NOT NULL)"},         
                                                        
       };
            #endregion
    }
}
