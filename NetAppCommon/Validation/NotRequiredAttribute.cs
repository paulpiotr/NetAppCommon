using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NetAppCommon.Validation
{
    public class NotRequiredAttribute : ValidationAttribute
    {
        #region private readonly log4net.ILog log4net
        /// <summary>
        /// Instancja do klasy Log4netLogger
        /// Instance to Log4netLogger class
        /// </summary>
        private readonly log4net.ILog _log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }
}
