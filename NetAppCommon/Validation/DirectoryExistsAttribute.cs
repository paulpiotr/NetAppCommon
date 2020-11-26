using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;

namespace NetAppCommon.Validation
{
    #region public class InListOfStringAttribute : ValidationAttribute
    /// <summary>
    /// Sprawdź, czy wartość występuje w podanej liści argumentów rozdzielonych separatorem ',', ';'
    /// Sprawdź, czy wartość ceny w podanej liści argumentów rozdzielonych separatorem ',', ';'
    /// </summary>
    public class DirectoryExistsAttribute : ValidationAttribute
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public new string ErrorMessage { get; set; }
        /// <summary>
        /// Treść informacji o błędzie
        /// The content of the error information
        /// </summary>
        public new string ErrorMessage { get; set; }
        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (null != value)
            {
                if (Directory.Exists((string)value))
                {
                    return ValidationResult.Success;
                }
                if (null != ErrorMessage)
                {
                    return new ValidationResult(string.Format(ErrorMessage, value));
                }
                else
                {
                    return new ValidationResult(string.Format("Katalog {0} nie istnieje lub nie masz do niego uprawnień!", (string)value));
                }
            }
            return ValidationResult.Success;
        }
    }
    #endregion
}