#region using

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore.Migrations;

#endregion

namespace NetAppCommon.Helpers.Db.Mssql;

public class AuditMigration
{
    #region private readonly log4net.ILog log4net

    /// <summary>
    ///     Log4net Logger
    /// </summary>
    private static readonly ILog Log4net =
        Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

    #endregion

    #region public static bool Create(MigrationBuilder migrationBuilder, string tableName)

    /// <summary>
    ///     Utwórz skrypt migracji tabeli i wyzwalacza audytu i wykonaj na serwerze MSSQL
    ///     Create table migration script and audit trigger and execute on MSSQL server
    /// </summary>
    /// <param name="migrationBuilder">
    ///     Konstruktor migracji migrationBuilder jako Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    ///     Migration constructor of migrationBuilder as Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    /// </param>
    /// <param name="schemaName">
    ///     Nazwa schematu schemaName jako string
    ///     The name of the schema schemaName as a string
    /// </param>
    /// <param name="tableName">
    ///     Nazwa tabeli tableName jako string
    ///     The name of the table tableName as a string
    /// </param>
    /// <returns>
    ///     prawda jeśli powodzenie, fałsz jeśli wyjątek
    ///     true if successful, false if exception
    /// </returns>
    public static bool Create(MigrationBuilder migrationBuilder, string schemaName, string tableName)
    {
        try
        {
            var auditCreateTableScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "Db",
                "Mssql", "AuditCreateTableScript.sql");
            var auditCreateTriggerScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "Db",
                "Mssql", "AuditCreateTriggerScript.sql");
            if (File.Exists(auditCreateTableScriptPath) && File.Exists(auditCreateTriggerScriptPath))
            {
                var sql = File.ReadAllText(auditCreateTableScriptPath).Replace("%SchemaName%", schemaName)
                    .Replace("%TableName%", tableName);
                Log4net.Debug(string.Format("Execute SQL: {0}", sql));
                migrationBuilder.Sql(sql);
                Log4net.Debug("OK");
                sql = File.ReadAllText(auditCreateTriggerScriptPath).Replace("%SchemaName%", schemaName)
                    .Replace("%TableName%", tableName);
                Log4net.Debug(string.Format("Execute SQL: {0}", sql));
                migrationBuilder.Sql(sql);
                Log4net.Debug("OK");
            }
            else
            {
                throw new FileNotFoundException(
                    $"File not found {auditCreateTableScriptPath} or {auditCreateTriggerScriptPath}");
            }
        }
        catch (Exception e)
        {
            Log4net.Error(
                string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                    e.StackTrace), e);
            return false;
        }

        return true;
    }

    #endregion

    #region public static bool CreateAsync(MigrationBuilder migrationBuilder, string schemaName, string tableName)

    /// <summary>
    ///     Utwórz skrypt migracji tabeli i wyzwalacza audytu i wykonaj na serwerze MSSQL (asynchronicznie)
    ///     Create table migration script and audit trigger and execute on MSSQL server (asynchronously)
    /// </summary>
    /// <param name="migrationBuilder">
    ///     Konstruktor migracji migrationBuilder jako Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    ///     Migration constructor of migrationBuilder as Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    /// </param>
    /// <param name="schemaName">
    ///     Nazwa schematu schemaName jako string
    ///     The name of the schema schemaName as a string
    /// </param>
    /// <param name="tableName">
    ///     Nazwa tabeli tableName jako string
    ///     The name of the table tableName as a string
    /// </param>
    /// <returns>
    ///     prawda jeśli powodzenie, fałsz jeśli wyjątek
    ///     true if successful, false if exception
    /// </returns>
    public static async Task<bool> CreateAsync(MigrationBuilder migrationBuilder, string schemaName,
        string tableName)
    {
        try
        {
            return await Task.Run(() => Create(migrationBuilder, schemaName, tableName));
        }
        catch (Exception e)
        {
            await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            return false;
        }
    }

    #endregion

    #region public static bool Drop(MigrationBuilder migrationBuilder, string tableName)

    /// <summary>
    ///     Utwórz skrypt wycofania migracji tabeli i wyzwalacza audytu i wykonaj na serwerze MSSQL
    ///     Create table migration rollback script and audit trigger and execute on MSSQL server
    /// </summary>
    /// <param name="migrationBuilder">
    ///     Konstruktor migracji migrationBuilder jako Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    ///     Migration constructor of migrationBuilder as Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    /// </param>
    /// <param name="schemaName">
    ///     Nazwa schematu schemaName jako string
    ///     The name of the schema schemaName as a string
    /// </param>
    /// <param name="tableName">
    ///     Nazwa tabeli tableName jako string
    ///     The name of the table tableName as a string
    /// </param>
    /// <returns>
    ///     prawda jeśli powodzenie, fałsz jeśli wyjątek
    ///     true if successful, false if exception
    /// </returns>
    public static bool Drop(MigrationBuilder migrationBuilder, string schemaName, string tableName)
    {
        try
        {
            var auditDropTableScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "Db",
                "Mssql", "AuditDropTableScript.sql");
            var auditDropTriggerScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "Db",
                "Mssql", "AuditDropTriggerScript.sql");
            if (File.Exists(auditDropTableScriptPath) && File.Exists(auditDropTriggerScriptPath))
            {
                var sql = File.ReadAllText(auditDropTableScriptPath).Replace("%SchemaName%", schemaName)
                    .Replace("%TableName%", tableName);
                Log4net.Debug(string.Format("Execute SQL: {0}", sql));
                migrationBuilder.Sql(sql);
                Log4net.Debug("OK");
                sql = File.ReadAllText(auditDropTriggerScriptPath).Replace("%SchemaName%", schemaName)
                    .Replace("%TableName%", tableName);
                Log4net.Debug(string.Format("Execute SQL: {0}", sql));
                migrationBuilder.Sql(sql);
                Log4net.Debug("OK");
            }
            else
            {
                throw new FileNotFoundException(
                    $"File not found {auditDropTableScriptPath} or {auditDropTriggerScriptPath}");
            }
        }
        catch (Exception e)
        {
            Log4net.Error(
                string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                    e.StackTrace), e);
            return false;
        }

        return true;
    }

    #endregion

    #region public static bool DropAsync(MigrationBuilder migrationBuilder, string tableName)

    /// <summary>
    ///     Utwórz skrypt wycofania migracji tabeli i wyzwalacza audytu i wykonaj na serwerze MSSQL (asynchronicznie)
    ///     Create table migration rollback script and audit trigger and execute on MSSQL server (asynchronously)
    /// </summary>
    /// <param name="migrationBuilder">
    ///     Konstruktor migracji migrationBuilder jako Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    ///     Migration constructor of migrationBuilder as Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder
    /// </param>
    /// <param name="schemaName">
    ///     Nazwa schematu schemaName jako string
    ///     The name of the schema schemaName as a string
    /// </param>
    /// <param name="tableName">
    ///     Nazwa tabeli tableName jako string
    ///     The name of the table tableName as a string
    /// </param>
    /// <returns>
    ///     prawda jeśli powodzenie, fałsz jeśli wyjątek
    ///     true if successful, false if exception
    /// </returns>
    public static async Task<bool> DropAsync(MigrationBuilder migrationBuilder, string schemaName, string tableName)
    {
        try
        {
            return await Task.Run(() => Drop(migrationBuilder, schemaName, tableName));
        }
        catch (Exception e)
        {
            await Task.Run(() => Log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            return false;
        }
    }

    #endregion
}
