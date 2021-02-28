#region using

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using log4net;
using Microsoft.Data.SqlClient;

#endregion

namespace NetAppCommon.Validation
{
    #region public class InListOfStringAttribute : ValidationAttribute

    /// <summary>
    ///     Sprawdź, czy wartość występuje w podanej liści argumentów rozdzielonych separatorem ',', ';'
    ///     Sprawdź, czy wartość ceny w podanej liści argumentów rozdzielonych separatorem ',', ';'
    /// </summary>
    public class MssqlCanConnectAttribute : ValidationAttribute
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     private readonly ILog _log4Net
        /// </summary>
        private readonly ILog log4net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public new string ErrorMessage { get; set; }

        /// <summary>
        ///     Treść informacji o błędzie
        ///     The content of the error information
        /// </summary>
        public new string ErrorMessage { get; set; }

        #endregion

        #region public string Exception { get; set; }

        /// <summary>
        ///     Ciąg wyjątku Exception jako string
        ///     An Exception string as a string
        /// </summary>
        public string Exception { get; set; }

        #endregion

        #region private bool CheckForConnection { get; set; } = true;

        /// <summary>
        ///     Sprawdź możliwość podłączenia do bazy danych z wpisania parametru Ciąg połączenia do bazy danych Mssql
        ///     Check the possibility of connecting to the database by entering the Mssql database connection string parameter
        /// </summary>
        private bool CheckForConnection { get; set; } = true;

        #endregion

        #region private bool CheckForConnection { get; set; } = true;

        /// <summary>
        ///     Sprawdź możliwość podłączenia do bazy danych z wpisania parametru Ciąg połączenia do bazy danych Mssql
        ///     Check the possibility of connecting to the database by entering the Mssql database connection string parameter
        /// </summary>
        private string ConnectionSettings { get; set; }

        #endregion

        #region private void SetValidationContext(ValidationContext validationContext)

        /// <summary>
        ///     Ustaw właściwości klasy walidatora z instancji przekazanego kontekstu
        ///     Set the validator class properties from the instance of the passed context
        /// </summary>
        /// <param name="validationContext">
        ///     Przekazany kontekst do walidatora jako ValidationContext
        ///     The context passed to the validator as ValidationContext
        /// </param>
        private void SetValidationContext(ValidationContext validationContext)
        {
            try
            {
                CheckForConnection = (bool)validationContext.ObjectInstance.GetType()
                    .GetProperty("CheckForConnection", BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(validationContext.ObjectInstance);
            }
            catch (Exception)
            {
                CheckForConnection = false;
            }

            try
            {
                ConnectionSettings = (string)validationContext.ObjectInstance.GetType()
                    .GetMethod("GetConnectionString", BindingFlags.Public | BindingFlags.Instance)
                    .Invoke(validationContext.ObjectInstance, new object[] { });
            }
            catch (Exception)
            {
                ConnectionSettings = null;
            }
        }

        #endregion

        #region protected override ValidationResult IsValid(object value, ValidationContext validationContext)

        /// <summary>
        ///     Sprawdź, czy wartość występuje w podanej liści argumentów rozdzielonych separatorem ',', ';'
        ///     Sprawdź, czy wartość ceny w podanej liści argumentów rozdzielonych separatorem ',', ';'
        /// </summary>
        /// <param name="value">
        ///     Przekazana wartość jako object
        ///     /// Value passed as object
        /// </param>
        /// <param name="validationContext">
        ///     Kontekst klasy nadrzędnej jako ValidationContext
        ///     The context of the parent class as ValidationContext
        /// </param>
        /// <returns>
        ///     Rezultat walidacji jako ValidationResult
        ///     Validation result as ValidationResult
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (null != validationContext)
            {
                SetValidationContext(validationContext);
            }

            if (null != value && CheckForConnection)
            {
                value = ConnectionSettings ?? value;
                SqlConnection sqlConnection = null;
                try
                {
                    using (sqlConnection = new SqlConnection(DatabaseMssql.ParseConnectionString((string)value)))
                    {
                        sqlConnection.Open();
                        return ValidationResult.Success;
                    }
                }
                catch (Exception e)
                {
                    Exception = string.Format("{0}, {1}.", e.Message, e.StackTrace);
                    log4net.Error(Exception, e);
                }
                finally
                {
                    try
                    {
                        if (null != sqlConnection)
                        {
                            sqlConnection.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Exception = string.Format("{0}, {1}.", e.Message, e.StackTrace);
                        log4net.Error(Exception, e);
                    }
                }

                if (null != ErrorMessage)
                {
                    return new ValidationResult(string.Format(ErrorMessage, value));
                }

                return new ValidationResult(string.Format(
                    "Nie można utworzyć połączenia do bazy danych Mssql używając danych: {0} {1}", value, Exception));
            }

            return ValidationResult.Success;
        }

        #endregion
    }

    #endregion
}
