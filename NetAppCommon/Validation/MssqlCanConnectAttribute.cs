using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NetAppCommon.Validation
{
    #region public class InListOfStringAttribute : ValidationAttribute
    /// <summary>
    /// Sprawdź, czy wartość występuje w podanej liści argumentów rozdzielonych separatorem ',', ';'
    /// Sprawdź, czy wartość ceny w podanej liści argumentów rozdzielonych separatorem ',', ';'
    /// </summary>
    public class MssqlCanConnectAttribute : ValidationAttribute
    {
        #region private static readonly log4net.ILog _log4net
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public new string ErrorMessage { get; set; }
        /// <summary>
        /// Treść informacji o błędzie
        /// The content of the error information
        /// </summary>
        public new string ErrorMessage { get; set; }
        #endregion

        #region protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        /// <summary>
        /// Sprawdź, czy wartość występuje w podanej liści argumentów rozdzielonych separatorem ',', ';'
        /// Sprawdź, czy wartość ceny w podanej liści argumentów rozdzielonych separatorem ',', ';'
        /// </summary>
        /// <param name="value">
        /// Przekazana wartość jako object
        /// /// Value passed as object
        /// </param>
        /// <param name="validationContext">
        /// Kontekst klasy nadrzędnej jako ValidationContext
        /// The context of the parent class as ValidationContext
        /// </param>
        /// <returns>
        /// Rezultat walidacji jako ValidationResult
        /// Validation result as ValidationResult
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (null != value)
            {
                SqlConnection sqlConnection = null;
                try
                {
                    using (sqlConnection = new SqlConnection((string)value))
                    {
                        sqlConnection.Open();
                        return ValidationResult.Success;
                    }
                }
                catch (Exception e)
                {
                    _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
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
                        _log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
                    }
                }
                if (null != ErrorMessage)
                {
                    return new ValidationResult(string.Format(ErrorMessage, value));
                }
                else
                {
                    return new ValidationResult(string.Format("Nie można utworzyć połączenia do bazy danych Mssql używając danych: {0}", value));
                }
            }
            return ValidationResult.Success;
        }
        #endregion
    }
    #endregion
}