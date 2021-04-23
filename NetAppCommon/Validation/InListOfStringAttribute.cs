#region using

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public class InListOfStringAttribute : ValidationAttribute
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     private readonly ILog _log4Net
        /// </summary>
        private readonly ILog log4net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public InListOfStringAttribute(string separatedListOfAttributes)

        /// <summary>
        ///     Konstruktor z parametrem separatedListOfAttributes jako string
        ///     A constructor with the separatedListOfAttributes parameter as a string
        /// </summary>
        /// <param name="separatedListOfAttributes">
        ///     Lista atrubytów rozdzielona separatorem ',', ';' jako string
        ///     A list of attributes separated by ',', ';' as a string
        /// </param>
        public InListOfStringAttribute(string separatedListOfAttributes)
        {
            try
            {
                char[] delimiterChars = {',', ';'};
                ListOfAttributes = new List<string>(separatedListOfAttributes.Split(delimiterChars))
                    .Select(x => x.Trim()).ToList();
            }
            catch (Exception e)
            {
                log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }
        }

        #endregion

        #region public InListOfStringAttribute(string errorMessage, string separatedListOfAttributes)

        /// <summary>
        ///     Konstruktor z parametrem errorMessage jako string i separatedListOfAttributes jako string
        ///     Constructor with errorMessage as string and separatedListOfAttributes as string
        /// </summary>
        /// <param name="errorMessage">
        ///     Treść informacji o błędzie
        ///     The content of the error information
        /// </param>
        /// <param name="separatedListOfAttributes">
        ///     Lista atrubytów rozdzielona separatorem ',', ';' jako string
        ///     A list of attributes separated by ',', ';' as a string
        /// </param>
        public InListOfStringAttribute(string errorMessage, string separatedListOfAttributes)
        {
            try
            {
                char[] delimiterChars = {',', ';'};
                ListOfAttributes = new List<string>(separatedListOfAttributes.Split(delimiterChars))
                    .Select(x => x.Trim()).ToList();
                ErrorMessage = errorMessage;
            }
            catch (Exception e)
            {
                log4net.Error(
                    string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message,
                        e.StackTrace), e);
            }
        }

        #endregion

        #region public new string ErrorMessage { get; set; }

        /// <summary>
        ///     Treść informacji o błędzie
        ///     The content of the error information
        /// </summary>
        public new string ErrorMessage { get; set; }

        #endregion

        #region public string SeparatedListOfAttributes { get; set; }

        /// <summary>
        ///     Lista atrubytów rozdzielona separatorem ',', ';' jako string
        ///     A list of attributes separated by ',', ';' as a string
        /// </summary>
        public string SeparatedListOfAttributes { get; set; }

        #endregion

        #region private List<string> ListOfAttributes { get; set; }

        /// <summary>
        ///     Lista atrubytów rozdzielona separatorem ',', ';' jako lista elementów string
        ///     A list of attributes separated by ',', ';' as a list of string elements
        /// </summary>
        private List<string> ListOfAttributes { get; }

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
            if (null != value && null != ListOfAttributes && ListOfAttributes.Count > 0)
            {
                if (ListOfAttributes.Contains((string)value))
                {
                    return ValidationResult.Success;
                }

                if (null != ErrorMessage)
                {
                    return new ValidationResult(string.Format(ErrorMessage, value));
                }

                return new ValidationResult(string.Format("Wartość {0} nie występuje w podanej liście argumentów {1}",
                    (string)value, string.Join(",", ListOfAttributes)));
            }

            return ValidationResult.Success;
        }

        #endregion
    }

    #endregion
}
