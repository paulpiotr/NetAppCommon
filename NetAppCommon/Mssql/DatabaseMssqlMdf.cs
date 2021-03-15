#region using

using System;
using System.Data;
using System.IO;
using System.Reflection;
using log4net;
using Microsoft.Data.SqlClient;

#endregion

namespace NetAppCommon.Mssql
{
    public abstract class DatabaseMssqlMdf : IDatabaseMssqlMdf
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     private readonly ILog _log4Net
        ///     private readonly ILog _log4Net
        /// </summary>
        private readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        public virtual string ConnectionString { get; set; }

        public virtual string ConnectionStringName { get; set; }

        public virtual string SettingsJsonFileName { get; set; }

        public virtual string AttachDBFilename { get; set; }

        public virtual string InitialCatalog { get; set; }

        public virtual string Size { get; set; }

        public virtual string MaxSize { get; set; }

        public virtual string FileGrowTh { get; set; }

        public virtual string LogInitialCatalog { get; set; }

        public virtual string LogAttachDBFilename { get; set; }

        public virtual string LogSize { get; set; }

        public virtual string LogMaxSize { get; set; }

        public virtual string LogFileGrowTh { get; set; }

        public virtual string CreateScript { get; set; }

        public virtual string GetCreateScript(string connectionString = null, string connectionStringName = null,
            string settingsJsonFileName = null)
        {
            try
            {
                ConnectionStringName = connectionStringName;
                SettingsJsonFileName = settingsJsonFileName;
                ConnectionString = connectionString ??
                                   DatabaseMssql.GetConnectionString(ConnectionStringName, SettingsJsonFileName);
                if (null != ConnectionString)
                {
                    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
                    AttachDBFilename = sqlConnectionStringBuilder.AttachDBFilename;
                    if (null != AttachDBFilename && !string.IsNullOrWhiteSpace(AttachDBFilename))
                    {
                        InitialCatalog = sqlConnectionStringBuilder.InitialCatalog;
                        LogAttachDBFilename ??= Path.Combine(Path.GetDirectoryName(AttachDBFilename),
                            string.Format("{0}_log.ldf",
                                Path.GetFileName(AttachDBFilename)
                                    .Replace(Path.GetExtension(Path.GetFileName(AttachDBFilename)), string.Empty)));
                        LogInitialCatalog ??= string.Format("{0}_log", InitialCatalog);
                        if (
                            null != sqlConnectionStringBuilder &&
                            null != AttachDBFilename && !string.IsNullOrWhiteSpace(AttachDBFilename) &&
                            !File.Exists(AttachDBFilename) &&
                            null != InitialCatalog && !string.IsNullOrWhiteSpace(InitialCatalog) &&
                            null != LogAttachDBFilename && !string.IsNullOrWhiteSpace(LogAttachDBFilename) &&
                            null != LogInitialCatalog && !string.IsNullOrWhiteSpace(LogInitialCatalog)
                        )
                        {
                            var mdfCreateDatabaseScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                "scripts", "Db", "Mssql", "MdfCreateDatabaseScript.sql");
                            if (File.Exists(mdfCreateDatabaseScriptPath))
                            {
                                CreateScript = File.ReadAllText(mdfCreateDatabaseScriptPath)
                                        .Replace("%InitialCatalog%", InitialCatalog)
                                        .Replace("%AttachDBFilename%", AttachDBFilename)
                                        .Replace("%Size%", Size ?? "8MB")
                                        .Replace("%MaxSize%", MaxSize ?? "131072MB")
                                        .Replace("%FileGrowTh%", FileGrowTh ?? "1 %")
                                        .Replace("%LogInitialCatalog%", LogInitialCatalog)
                                        .Replace("%LogAttachDBFilename%", LogAttachDBFilename)
                                        .Replace("%LogSize%", LogSize ?? "8MB")
                                        .Replace("%LogMaxSize%", LogMaxSize ?? "8192MB")
                                        .Replace("%LogFileGrowTh%", LogFileGrowTh ?? "1 %")
                                    ;
                            }
#if DEBUG
                            _log4Net.Debug($"CreateScript{Environment.NewLine}{CreateScript}{Environment.NewLine}");
#endif
                            return CreateScript;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return null;
        }

        public virtual bool Create(string connectionString = null, string connectionStringName = null,
            string settingsJsonFileName = null)
        {
            try
            {
                CreateScript = GetCreateScript(connectionString, connectionStringName, settingsJsonFileName);
                if (null != CreateScript && !string.IsNullOrWhiteSpace(CreateScript))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(AttachDBFilename)))
                    {
#if DEBUG
                        _log4Net.Debug($"Create directory {Path.GetDirectoryName(AttachDBFilename)}");
#endif
                        Directory.CreateDirectory(Path.GetDirectoryName(AttachDBFilename));
                    }

                    if (Directory.Exists(Path.GetDirectoryName(AttachDBFilename)) &&
                        !File.Exists(AttachDBFilename)
                    )
                    {
                        lock (string.Intern(AttachDBFilename))
                        {
#if DEBUG
                            _log4Net.Debug($"lock (string.Intern({AttachDBFilename}))");
#endif
                            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
                            SqlConnection sqlConnection = null;
                            SqlCommand sqlCommand = null;
                            if (null != sqlConnectionStringBuilder)
                            {
                                sqlConnection =
                                    new SqlConnection(
                                        $"Data Source={sqlConnectionStringBuilder.DataSource}; Integrated Security={sqlConnectionStringBuilder.IntegratedSecurity}");
                                sqlCommand = new SqlCommand(CreateScript, sqlConnection);
                            }

                            try
                            {
                                if (null != sqlConnection && null != sqlCommand)
                                {
                                    sqlConnection.Open();
                                    sqlCommand.ExecuteNonQuery();
#if DEBUG
                                    _log4Net.Debug(
                                        $"Sql command execute non query {Environment.NewLine}{CreateScript}{Environment.NewLine}OK");
#endif
                                    return true;
                                }
                            }
                            catch (Exception e)
                            {
                                _log4Net.Error(
                                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(),
                                        e.Message, e.StackTrace), e);
                            }
                            finally
                            {
                                if (null != sqlConnection && sqlConnection.State == ConnectionState.Open)
                                {
                                    sqlConnection.Close();
                                }
                            }
                        }

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _log4Net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }

            return false;
        }

        public virtual bool Drop(string connectionString = null, string connectionStringName = null,
            string settingsJsonFileName = null)
        {
            throw new NotImplementedException();
        }
    }
}