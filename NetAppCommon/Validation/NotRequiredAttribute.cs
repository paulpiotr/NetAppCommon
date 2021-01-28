#region using

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using log4net;

#endregion

namespace NetAppCommon.Validation
{
    public class NotRequiredAttribute : ValidationAttribute
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Instancja do klasy Log4netLogger
        ///     Instance to Log4netLogger class
        /// </summary>
        private readonly ILog _log4Net =
            Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) =>
            ValidationResult.Success;
    }
}
