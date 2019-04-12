
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;
using System.Configuration;


#if UseSQLITE
using System.Data.SQLite;

namespace SDK_SC_Fingerprint
{
   
    public class DBClassUser
    {
        private string ConnectionString;
        private readonly ArrayList UserList;

        public DBClassUser(ArrayList UserList)
        {
            this.UserList = UserList;

            string pathDB = Application.StartupPath;           
           
            // On crée un nouvel objet SQLiteConnectionStringBuilder.
            SQLiteConnectionStringBuilder SQLCSB = new SQLiteConnectionStringBuilder();
            // On définit nos paramètres.
            SQLCSB.DataSource = pathDB + @"\UserDB.db3";
            SQLCSB.Password = "123456"; // Permet de définir le mot de passe à utiliser pour l'accès à la BDD
            // Obtenons cette ConnectionString !
            ConnectionString = SQLCSB.ToString();
        }
        public void RecoverUser()
        {
            try
            {
                UserList.Clear();
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();
                SQLCmd.CommandText = "SELECT EnrolledData FROM UserTable";

                // On crée un objet SQLiteDataReader
                SQLiteDataReader SQLDReader = SQLCmd.ExecuteReader();
                
                // La méthode Read() lit l'entrée actuelle puis renvoie true tant qu'il y a des entrées à lire.
                while (SQLDReader.Read())
                {
                   
                   BinaryFormatter bf = new BinaryFormatter();
                   MemoryStream mem = new MemoryStream(Convert.FromBase64String(SQLDReader[0].ToString()));
                   UserClass TheUser = new UserClass();
                   TheUser = (UserClass)bf.Deserialize(mem);                    
                   UserList.Add(TheUser);
                }
                SQLC.Close();
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
            }
        }
        public bool IsUserExist(string firstName, string lastName)
        {
            try
            {
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();

                SQLCmd.CommandText = "SELECT * FROM UserTable ";
                SQLCmd.CommandText += "WHERE FirstName='" + firstName + "' ";
                SQLCmd.CommandText += "AND LastName='" + lastName + "';";
                SQLiteDataReader SQLDReader = SQLCmd.ExecuteReader();               
               
                bool bRet = SQLDReader.HasRows;
                SQLC.Close();
                return bRet;
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
                return false;
            }
        }
        public void StoreUser()
        {
            try
            {
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                foreach (UserClass TheUser in UserList)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream();
                    bf.Serialize(mem, TheUser);
                    string str = Convert.ToBase64String(mem.ToArray());
                    // On demande à notre connexion de nous créer un objet SQLiteCommand.
                    SQLiteCommand SQLCmd = SQLC.CreateCommand();

                    SQLCmd.CommandText = "UPDATE UserTable ";
                    SQLCmd.CommandText += "SET EnrolledData='" + str + "' ";
                    SQLCmd.CommandText += "WHERE FirstName='" + TheUser.firstName + "' ";
                    SQLCmd.CommandText += "AND LastName='" + TheUser.lastName + "';";
                    SQLCmd.ExecuteNonQuery();
                }
                SQLC.Close();
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
            }
        }
        public void RecoverUserData(ref UserClass UserData)
        {
            try
            {
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();

                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();
                SQLCmd.CommandText = "SELECT EnrolledData FROM UserTable ";
                SQLCmd.CommandText += "WHERE FirstName='" + UserData.firstName + "' ";
                SQLCmd.CommandText += "AND LastName='" + UserData.lastName + "';";

                // On crée un objet SQLiteDataReader
                SQLiteDataReader SQLDReader = SQLCmd.ExecuteReader();

                // La méthode Read() lit l'entrée actuelle puis renvoie true tant qu'il y a des entrées à lire.
                while (SQLDReader.Read())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream mem = new MemoryStream(Convert.FromBase64String(SQLDReader[0].ToString()));
                    UserData = (UserClass)bf.Deserialize(mem);
                }
                SQLC.Close();
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
            }
        }
        public void AddUser(UserClass TheUser)
        {
            try
            {
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream mem = new MemoryStream();
                bf.Serialize(mem, TheUser);
                string str = Convert.ToBase64String(mem.ToArray());

                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();
                SQLCmd.CommandText = "INSERT INTO UserTable VALUES('";
                SQLCmd.CommandText += TheUser.firstName + "','";
                SQLCmd.CommandText += TheUser.lastName + "','";
                SQLCmd.CommandText += str + "');";
                SQLCmd.ExecuteNonQuery();
                SQLC.Close();
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
            }
        }
        public void DeleteUser(string firstName,string LastName)
        {
            try
            {
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();

                SQLCmd.CommandText = "DELETE FROM UserTable ";
                SQLCmd.CommandText += "WHERE FirstName='" + firstName + "' ";
                SQLCmd.CommandText += "AND LastName='" + LastName + "';";
                SQLCmd.ExecuteNonQuery();
                SQLC.Close();
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
            }
        }
        public void DeleteUser(UserClass TheUser)
        {
            try
            {
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();

                SQLCmd.CommandText = "DELETE FROM UserTable ";
                SQLCmd.CommandText += "WHERE FirstName='" + TheUser.firstName + "' ";
                SQLCmd.CommandText += "AND LastName='" + TheUser.lastName + "';";
                SQLCmd.ExecuteNonQuery();
                SQLC.Close();
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
            }
        }
    }
   

    public class DBClassFinger
    {
        private string ConnectionString;

        public DBClassFinger()     
        {           

            string pathDB = Application.StartupPath;           
           
            // On crée un nouvel objet SQLiteConnectionStringBuilder.
            SQLiteConnectionStringBuilder SQLCSB = new SQLiteConnectionStringBuilder();
            // On définit nos paramètres.
            SQLCSB.DataSource = pathDB + @"\FingerDB.db3";
            SQLCSB.Password = "123456"; // Permet de définir le mot de passe à utiliser pour l'accès à la BDD
            // Obtenons cette ConnectionString !
            ConnectionString = SQLCSB.ToString();
        }

        public string GetSerialNumberFinger(string SerialNumberReader)
        {
            string serial = null;
            try
            {
               
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();
                SQLCmd.CommandText = "SELECT SerialFinger FROM SerialFingerTable ";
                SQLCmd.CommandText += "WHERE SerialReader='" + SerialNumberReader + "';"; ; 

                // On crée un objet SQLiteDataReader
                SQLiteDataReader SQLDReader = SQLCmd.ExecuteReader();
                bool bRet = SQLDReader.HasRows;

                if (bRet)
                {
                    // La méthode Read() lit l'entrée actuelle puis renvoie true tant qu'il y a des entrées à lire.
                    SQLDReader.Read();
                    serial = SQLDReader[0].ToString();
                }                   
                SQLC.Close();
                return serial;
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
                return null;
            }
        }

        public bool UpdateSerialFinger(string SerialReader, string SerialFinger)
        {

            try
            {
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();  
              
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();

                SQLCmd.CommandText = "UPDATE SerialFingerTable ";
                SQLCmd.CommandText += "SET SerialFinger='" + SerialFinger + "' ";
                SQLCmd.CommandText += "WHERE SerialReader='" + SerialReader + "';";                
                SQLCmd.ExecuteNonQuery();
                return true;
               
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
                return false;
            }
        }

        public bool AddSerialFinger(string SerialReader, string SerialFinger)
        {
            try
            {
                // On crée une connexion, le constructeur prend en paramètre la ConnectionString.
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();

                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();

                SQLCmd.CommandText = "INSERT INTO SerialFingerTable VALUES('";
                SQLCmd.CommandText += SerialReader + "','";
                SQLCmd.CommandText += SerialFinger + "');";
                SQLCmd.ExecuteNonQuery();
                SQLC.Close();
                return true;
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
                return false;
            }
        }

        public bool DeleteSerialFinger(string SerialReader)
        {
            try
            {
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();
                SQLCmd.CommandText = "DELETE FROM SerialFingerTable ";
                SQLCmd.CommandText += "WHERE SerialReader='" + SerialReader + "';";
                SQLCmd.ExecuteNonQuery();
                SQLC.Close();
                return true;
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
                return false;
            }
        }

        public bool IsSerialReaderHasData(string SerialReader)
        {
            try
            {
                SQLiteConnection SQLC = new SQLiteConnection(ConnectionString);
                // On ouvre la connexion.
                SQLC.Open();
                // On demande à notre connexion de nous créer un objet SQLiteCommand.
                SQLiteCommand SQLCmd = SQLC.CreateCommand();

                SQLCmd.CommandText = "SELECT * FROM SerialFingerTable ";
                SQLCmd.CommandText += "WHERE SerialReader='" + SerialReader + "';";
                SQLiteDataReader SQLDReader = SQLCmd.ExecuteReader();
                bool bRet = SQLDReader.HasRows;
                SQLC.Close();
                return bRet;
            }
            catch (Exception Ex)
            {
                // On affiche l'erreur.
                MessageBox.Show(string.Format("*** Une exception a été lancée : {0} ***", Ex.Message));
                return false;
            }
        }


    }
}
#endif