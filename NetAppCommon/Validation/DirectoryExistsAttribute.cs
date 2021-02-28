#region using

using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using log4net;

#endregion

namespace NetAppCommon.Validation
{
    #region public class InListOfStringAttribute : ValidationAttribute

    /// <summary>
    ///     Sprawdź, czy wartość występuje w podanej liści argumentów rozdzielonych separatorem ',', ';'
    ///     Sprawdź, czy wartość ceny w podanej liści argumentów rozdzielonych separatorem ',', ';'
    /// </summary>
    public class DirectoryExistsAttribute : ValidationAttribute
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

                return new ValidationResult(string.Format("Katalog {0} nie istnieje lub nie masz do niego uprawnień!",
                    (string)value));
            }

            return ValidationResult.Success;
        }
    }

    #endregion
}
