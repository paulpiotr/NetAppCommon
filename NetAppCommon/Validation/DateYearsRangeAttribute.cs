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
    public class DateYearsRangeAttribute : ValidationAttribute
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog Log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public int Min { get; set; }

        public int Max { get; set; }

        public DateTime DateTimeMin { get; private set; }

        public DateTime DateTimeMax { get; private set; }

        #region public new string ErrorMessage { get; set; }
        /// <summary>
        /// Treść informacji o błędzie
        /// The content of the error information
        /// </summary>
        public new string ErrorMessage { get; set; }
        #endregion

        public DateYearsRangeAttribute(object min, object max)
        {
            DateTimeMin = DateTime.Now.AddYears((int)min);
            DateTimeMax = DateTime.Now.AddYears((int)max);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (null != value)
            {
                if ((DateTime)value > DateTimeMax || (DateTime)value < DateTimeMin)
                {
                    return null != ErrorMessage
                        ? new ValidationResult(string.Format(ErrorMessage, value))
                        : new ValidationResult(string.Format("{0}, {1} must be between {2} and {3}", validationContext.DisplayName, (DateTime)value, DateTimeMin, DateTimeMax));
                }
            }
            return ValidationResult.Success;
        }
    }
    #endregion
}