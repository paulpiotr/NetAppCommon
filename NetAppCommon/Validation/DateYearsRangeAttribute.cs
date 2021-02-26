#region using

using System;
using System.ComponentModel.DataAnnotations;
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
    public class DateYearsRangeAttribute : ValidationAttribute
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     private readonly ILog _log4Net
        /// </summary>
        private static readonly ILog Log4net =
            Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        public DateYearsRangeAttribute(object min, object max)
        {
            DateTimeMin = DateTime.Now.AddYears((int)min);
            DateTimeMax = DateTime.Now.AddYears((int)max);
        }

        public int Min { get; set; }

        public int Max { get; set; }

        public DateTime DateTimeMin { get; }

        public DateTime DateTimeMax { get; }

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
                if ((DateTime)value > DateTimeMax || (DateTime)value < DateTimeMin)
                {
                    return null != ErrorMessage
                        ? new ValidationResult(string.Format(ErrorMessage, value))
                        : new ValidationResult(string.Format("{0}, {1} must be between {2} and {3}",
                            validationContext.DisplayName, (DateTime)value, DateTimeMin, DateTimeMax));
                }
            }

            return ValidationResult.Success;
        }
    }

    #endregion
}
