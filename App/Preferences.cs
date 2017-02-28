using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

namespace SharpCpp
{
    /// <summary>
    /// Very simplified preferences, just to store key/value pairs of strings.
    /// </summary>
    public static class Preferences
    {
        public const string AppName = "SharpCppApp";

        #region Database Internals

        static readonly DbHelper _dbHelper;

        static readonly PreferencesDbHelper _preferencesDbHelper;

        static Preferences()
        {
            _dbHelper = new DbHelper();
            _preferencesDbHelper = new PreferencesDbHelper(_dbHelper);
        }

        class PreferencesDbHelper
        {
            const string TableName = "Preferences";

            const string IdColumn = "id";

            const string ValueColumn = "value";

            readonly DbHelper DbHelper;

            internal PreferencesDbHelper(DbHelper dbHelper)
            {
                DbHelper = dbHelper;
                CreateTableIfNone();
            }

            void CreateTableIfNone()
            {
                using (var connection = DbHelper.CreateDbConnection()) {
                    connection.Open();

                    var command = "create table if not exists " +
                    $"{ TableName } (" +
                    $"  { IdColumn } text primary key, " +
                    $"  { ValueColumn } text" +
                    ")";

                    using (var c = connection.CreateCommand()) {
                        c.CommandText = command;
                        c.CommandType = CommandType.Text;
                        c.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }

            public string GetValue(string key)
            {
                string value = null;

                using (var connection = _dbHelper.CreateDbConnection()) {
                    connection.Open();

                    using (var command = connection.CreateCommand()) {
                        command.CommandText = $"SELECT {ValueColumn} FROM [{TableName}] WHERE {IdColumn}=@id";
                        command.Parameters.AddWithValue("@id", key);

                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                value = (string)reader[0];
                            }
                        }
                    }

                    connection.Close();
                }

                return value;
            }

            public void PutValue(string key, string value)
            {
                using (var connection = _dbHelper.CreateDbConnection()) {
                    connection.Open();

                    // try to insert

                    using (var command = connection.CreateCommand()) {
                        command.CommandText = $"INSERT OR IGNORE INTO [{TableName}] ({IdColumn}, {ValueColumn}) " +
                            "VALUES (@id, @value)";

                        command.Parameters.AddWithValue("@id", key);
                        command.Parameters.AddWithValue("@value", value);

                        command.ExecuteNonQuery();
                    }

                    // update

                    using (var command = connection.CreateCommand()) {
                        command.CommandText = $"UPDATE {TableName} SET {ValueColumn}=@value WHERE {IdColumn}=@id";

                        command.Parameters.AddWithValue("@id", key);
                        command.Parameters.AddWithValue("@value", value);

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
        }

        class DbHelper
        {
            const string DbName = "SharpCppApp.db";

            readonly string _dbFile;

            internal DbHelper()
            {
                _dbFile = CreateDbIfNone();
            }

            string CreateDbIfNone()
            {
                string dbFile;

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appDirectory = Path.Combine(documents, AppName);

                if (!Directory.Exists(appDirectory)) {
                    Directory.CreateDirectory(appDirectory);
                }

                dbFile = Path.Combine(appDirectory, DbName);
                if (!File.Exists(dbFile)) {
                    SqliteConnection.CreateFile(dbFile);
                }

                return dbFile;
            }

            public SqliteConnection CreateDbConnection()
            {
                return new SqliteConnection("Data Source=" + _dbFile);
            }
        }

        #endregion

        #region LastSelectedFilename

        const string LastSelectedFilenameKey = "last_selected_filename";

        // cached
        static string _selectedFilename;

        static bool _selectedFilenameLazyLoaded;

        public static string LastSelectedFilename {
            get {
                if (!_selectedFilenameLazyLoaded) {
                    _selectedFilename = _preferencesDbHelper.GetValue(LastSelectedFilenameKey);
                    _selectedFilenameLazyLoaded = true;
                }

                return _selectedFilename;
            }
            set {
                _selectedFilename = value;
                _preferencesDbHelper.PutValue(LastSelectedFilenameKey, value);
            }
        }

        #endregion

        #region LastSelectedSaveDirectory

        const string LastPickedSaveDirectoryKey = "last_picked_save_directory";

        public static string LastPickedSaveDirectory {
            get {
                return _preferencesDbHelper.GetValue(LastPickedSaveDirectoryKey);
            }
            set {
                _preferencesDbHelper.PutValue(LastPickedSaveDirectoryKey, value);
            }
        }

        #endregion
    }
}
