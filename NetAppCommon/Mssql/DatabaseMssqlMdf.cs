﻿using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace NetAppCommon.Mssql
{
    public abstract class DatabaseMssqlMdf : IDatabaseMssqlMdf
    {

        #region private static readonly log4net.ILog _log4net
        /// <summary>
        /// Log4 Net Logger
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
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

        public virtual string GetCreateScript(string connectionString = null, string connectionStringName = null, string settingsJsonFileName = null)
        {
            try
            {
                ConnectionStringName = connectionStringName;
                SettingsJsonFileName = settingsJsonFileName;
                ConnectionString = connectionString ?? DatabaseMssql.GetConnectionString(ConnectionStringName, SettingsJsonFileName);
                if (null != ConnectionString)
                {
                    SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
                    AttachDBFilename = sqlConnectionStringBuilder.AttachDBFilename;
                    InitialCatalog = sqlConnectionStringBuilder.InitialCatalog;
                    LogAttachDBFilename = LogAttachDBFilename ?? Path.Combine(Path.GetDirectoryName(AttachDBFilename), string.Format("{0}_log.ldf", Path.GetFileName(AttachDBFilename).Replace(Path.GetExtension(Path.GetFileName(AttachDBFilename)), string.Empty)));
                    LogInitialCatalog = LogInitialCatalog ?? string.Format("{0}_log", InitialCatalog);
                    _log4net.Debug($"AttachDBFilename { AttachDBFilename }, InitialCatalog { InitialCatalog }, LogAttachDBFilename { LogAttachDBFilename }, LogInitialCatalog { LogInitialCatalog }");
                    if (
                        null != sqlConnectionStringBuilder &&
                        null != AttachDBFilename && !string.IsNullOrWhiteSpace(AttachDBFilename) &&
                        !File.Exists(AttachDBFilename) &&
                        null != InitialCatalog && !string.IsNullOrWhiteSpace(InitialCatalog) &&
                        null != LogAttachDBFilename && !string.IsNullOrWhiteSpace(LogAttachDBFilename) &&
                        null != LogInitialCatalog && !string.IsNullOrWhiteSpace(LogInitialCatalog)
                        )
                    {
                        CreateScript = $"" +
                            $"BEGIN TRY EXEC sp_detach_db { InitialCatalog }; END TRY BEGIN CATCH SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; END CATCH;" +
                            $"BEGIN TRY CREATE DATABASE { InitialCatalog } ON PRIMARY " +
                            $"(" +
                                $"NAME = '{ InitialCatalog }', " +
                                $"FILENAME = '{ AttachDBFilename }', " +
                                $"SIZE = { Size ?? "8MB" }, " +
                                $"MAXSIZE = { MaxSize ?? "131072MB" }, " +
                                $"FILEGROWTH = { FileGrowTh ?? "1%" }" +
                            $") " +
                            $"LOG ON" +
                            $"(" +
                                $"NAME = '{ LogInitialCatalog }', " +
                                $"FILENAME = '{ LogAttachDBFilename }', " +
                                $"SIZE = { LogSize ?? "8MB" }, " +
                                $"MAXSIZE = { LogMaxSize ?? "8192MB" }, " +
                                $"FILEGROWTH = { LogFileGrowTh ?? "1%" }" +
                            $") END TRY BEGIN CATCH SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage; END CATCH;";
                        _log4net.Debug($"CreateScript { CreateScript }");
                        return CreateScript;
                    }
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
            }
            return null;
        }

        public virtual bool Create(string connectionString = null, string connectionStringName = null, string settingsJsonFileName = null)
        {
            try
            {
                CreateScript = GetCreateScript(connectionString, connectionStringName, settingsJsonFileName);
                if (null != CreateScript && !string.IsNullOrWhiteSpace(CreateScript))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(AttachDBFilename)))
                    {
                        _log4net.Debug($"Create directory { Path.GetDirectoryName(AttachDBFilename) }");
                        Directory.CreateDirectory(Path.GetDirectoryName(AttachDBFilename));
                    }
                    if (
                        Directory.Exists(Path.GetDirectoryName(AttachDBFilename)) &&
                        !File.Exists(AttachDBFilename)
                        )
                    {
                        SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
                        SqlConnection sqlConnection = null;
                        SqlCommand sqlCommand = null;
                        if (null != sqlConnectionStringBuilder)
                        {
                            sqlConnection = new SqlConnection($"Data Source={ sqlConnectionStringBuilder.DataSource }; Integrated Security={ sqlConnectionStringBuilder.IntegratedSecurity }");
                            sqlCommand = new SqlCommand(CreateScript, sqlConnection);
                        }
                        try
                        {
                            if (null != sqlConnection && null != sqlCommand)
                            {
                                sqlConnection.Open();
                                sqlCommand.ExecuteNonQuery();
                                _log4net.Debug($"Sql command execute non query { CreateScript } OK");
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                        }
                        finally
                        {
                            if (null != sqlConnection && sqlConnection.State == ConnectionState.Open)
                            {
                                sqlConnection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
            }
            return false;
        }

        public virtual bool Drop(string connectionString = null, string connectionStringName = null, string settingsJsonFileName = null)
        {
            throw new NotImplementedException();
        }
    }
}