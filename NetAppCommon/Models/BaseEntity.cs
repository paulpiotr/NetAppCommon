#region using

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using NetAppCommon.Helpers.Object;
using Newtonsoft.Json;

#endregion

namespace NetAppCommon.Models
{
    #region public class BaseEntity : INotifyPropertyChanged

    /// <summary>
    /// BaseEntity - wspólna klasa bazowa zawierająca domyślne pola tabel
    /// BaseEntity - common base class containing default table fields
    /// </summary>
    [Index(nameof(Id), IsUnique = true)]
    [Index(nameof(UniqueIdentifierOfTheLoggedInUser))]
    [Index(nameof(DateOfCreate))]
    [Index(nameof(DateOfModification))]
    public class BaseEntity : INotifyPropertyChanged
    {
        #region public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     PropertyChangedEventHandler PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region public void SetId(string s)

        /// <summary>
        ///     Ustaw identyfikator z dowolnego unikalnego ciągu znaków @string
        ///     Set the identifier from any unique string @string
        /// </summary>
        /// <param name="s">
        ///     Unikalny ciąg znaków @string jako string
        ///     Unique string @string as string
        /// </param>
        public void SetId(string @string)
        {
            Id = new Guid();
            Id = ObjectHelper.GuidFromString(@string);
        }

        #endregion

        #region public void SetUniqueIdentifierOfTheLoggedInUser()

        /// <summary>
        ///     Ustaw jednoznaczny identyfikator zalogowanego użytkownika
        ///     Set a unique identifier for the logged in user
        /// </summary>
        public void SetUniqueIdentifierOfTheLoggedInUser()
        {
            UniqueIdentifierOfTheLoggedInUser =
                HttpContextAccessor.AppContext.GetCurrentUserIdentityName();
        }

        #endregion

        #region protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)

        /// <summary>
        ///     protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        #endregion

        #region protected void OnPropertyChanged(string propertyName)

        /// <summary>
        ///     protected void OnPropertyChanged(string propertyName)
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region private Guid _id; public Guid Id

        private Guid _id;

        /// <summary>
        ///     Guid Id identyfikator klucz główny
        ///     Guid Id identifier of the primary key
        /// </summary>
        [XmlIgnore]
        [Key]
        [JsonProperty(nameof(Id))]
        [Display(Name = "Identyfikator", Prompt = "Wpisz identyfikator",
            Description = "Identyfikator, klucz główny w bazie danych jako Guid")]
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        #endregion

        #region private string _uniqueIdentifierOfTheLoggedInUser; public string UniqueIdentifierOfTheLoggedInUser

        private string _uniqueIdentifierOfTheLoggedInUser;

        /// <summary>
        ///     Jednoznaczny identyfikator zalogowanego użytkownika
        ///     Unique identifier of the logged in user
        /// </summary>
        [XmlIgnore]
        [JsonProperty(nameof(UniqueIdentifierOfTheLoggedInUser))]
        [Display(Name = "Użytkownik",
            Prompt = "Wybierz identyfikator zalogowanego użytkownika",
            Description = "Identyfikator zalogowanego użytkownika")]
        [MinLength(1)]
        [MaxLength(256)]
        [StringLength(256)]
        public string UniqueIdentifierOfTheLoggedInUser
        {
            get => _uniqueIdentifierOfTheLoggedInUser;
            private set
            {
                if (_uniqueIdentifierOfTheLoggedInUser != value)
                {
                    _uniqueIdentifierOfTheLoggedInUser = value;
                    OnPropertyChanged(nameof(UniqueIdentifierOfTheLoggedInUser));
                }
            }
        }

        #endregion

        #region private DateTime _dateOfCreate; public DateTime DateOfCreate

        private DateTime _dateOfCreate;

        /// <summary>
        ///     Data utworzenia
        ///     Date of create
        /// </summary>
        [XmlIgnore]
        [JsonProperty(nameof(DateOfCreate))]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Data utworzenia", Prompt = "Wpisz lub wybierz datę utworzenia",
            Description = "Data utworzenia")]
        [DataType(DataType.Date)]
        public DateTime DateOfCreate
        {
            get => _dateOfCreate;
            set
            {
                if (_dateOfCreate != value)
                {
                    _dateOfCreate = value;
                    OnPropertyChanged(nameof(DateOfCreate));
                }
            }
        }

        #endregion

        #region private DateTime? _dateOfModification; public DateTime? DateOfModification

        private DateTime? _dateOfModification;

        /// <summary>
        ///     Data modyfikacji
        ///     Date of modification
        /// </summary>
        [XmlIgnore]
        [JsonProperty(nameof(DateOfModification))]
        [Display(Name = "Data modyfikacji", Prompt = "Wpisz lub wybierz datę modyfikacji",
            Description = "Data modyfikacji")]
        [DataType(DataType.Date)]
        public DateTime? DateOfModification
        {
            get => _dateOfModification;
            set
            {
                if (_dateOfModification != value)
                {
                    _dateOfModification = value;
                    OnPropertyChanged(nameof(DateOfModification));
                }
            }
        }

        #endregion
    }

    #endregion
}