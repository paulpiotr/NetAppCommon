#region using

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetAppCommon.Helpers.Object;
using Newtonsoft.Json;

#endregion

#nullable enable annotations

namespace NetAppCommon.Models
{
    #region public class BaseEntity : INotifyPropertyChanged

    /// <summary>
    ///     BaseEntity - wspólna klasa bazowa zawierająca domyślne pola tabel
    ///     BaseEntity - common base class containing default table fields
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

        #region public BaseEntity()
        /// <summary>
        /// Konstruktor
        /// </summary>
        public BaseEntity()
        {
            SetUniqueIdentifierOfTheLoggedInUser();
        }

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
        public void SetUniqueIdentifierOfTheLoggedInUser() =>
            UniqueIdentifierOfTheLoggedInUser =
                HttpContextAccessor.AppContext.GetCurrentUserIdentityName();

        #endregion

        #region protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)

        /// <summary>
        ///     protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        #endregion

        #region protected void OnPropertyChanged(string propertyName)

        /// <summary>
        ///     protected void OnPropertyChanged(string propertyName)
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        #endregion

        #region private Guid _id; public Guid Id

        private Guid _id;

        /// <summary>
        ///     Guid Id identyfikator klucz główny
        ///     Guid Id identifier of the primary key
        /// </summary>
        [Key]
        [JsonProperty(nameof(Id), Order = -1)]
        [Display(Name = "Identyfikator", Prompt = "Wpisz identyfikator",
            Description = "Identyfikator, klucz główny w bazie danych jako Guid")]
        public virtual Guid Id
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    var dateTimeNowYear = new DateTime(DateTime.Now.Year, 1, 1);
                    _id = ObjectHelper.GuidFromString($"{ DateTime.Now }{ (DateTime.Now - dateTimeNowYear).TotalMilliseconds + Math.Abs(GetHashCode()).ToString() }");
                    OnPropertyChanged(nameof(Id));
                }

                return _id;
            }
            set
            {
                if (_id != value)
                {
                    if (value == Guid.Empty)
                    {
                        var dateTimeNowYear = new DateTime(DateTime.Now.Year, 1, 1);
                        _id = ObjectHelper.GuidFromString($"{ DateTime.Now }{ (DateTime.Now - dateTimeNowYear).TotalMilliseconds + Math.Abs(GetHashCode()).ToString() }");
                        OnPropertyChanged(nameof(Id));
                    }

                    else
                    {
                        _id = value;
                        OnPropertyChanged(nameof(Id));
                    }
                }
            }
        }

        #endregion

        #region private string _uniqueIdentifierOfTheLoggedInUser?; public string UniqueIdentifierOfTheLoggedInUser?

        private string? _uniqueIdentifierOfTheLoggedInUser;

        /// <summary>
        ///     Jednoznaczny identyfikator zalogowanego użytkownika
        ///     Unique identifier of the logged in user
        /// </summary>
        [Column(Order = 1)]
        [JsonProperty(nameof(UniqueIdentifierOfTheLoggedInUser), Order = -1)]
        [Display(Name = "Użytkownik",
            Prompt = "Wybierz identyfikator zalogowanego użytkownika",
            Description = "Identyfikator zalogowanego użytkownika")]
        [MinLength(1)]
        [MaxLength(256)]
        [StringLength(256)]
        public virtual string? UniqueIdentifierOfTheLoggedInUser
        {
            get => _uniqueIdentifierOfTheLoggedInUser;
            set
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
        [Column(Order = 3)]
        [JsonProperty(nameof(DateOfCreate), Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Data utworzenia", Prompt = "Wpisz lub wybierz datę utworzenia",
            Description = "Data utworzenia")]
        [DataType(DataType.Date)]
        public virtual DateTime DateOfCreate
        {
            get
            {
                if (DateTime.MinValue == _dateOfCreate)
                {
                    _dateOfCreate = DateTime.Now;
                    OnPropertyChanged(nameof(DateOfCreate));
                }

                return _dateOfCreate;
            }
            set
            {
                if (DateTime.MinValue == _dateOfCreate)
                {
                    _dateOfCreate = DateTime.Now;
                    OnPropertyChanged(nameof(DateOfCreate));
                }
                else if (_dateOfCreate != value)
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
        [Column(Order = 3)]
        [JsonProperty(nameof(DateOfModification), Order = 3)]
        [Display(Name = "Data modyfikacji", Prompt = "Wpisz lub wybierz datę modyfikacji",
            Description = "Data modyfikacji")]
        [DataType(DataType.Date)]
        public virtual DateTime? DateOfModification
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

        #region public string AsSHA512Hash()

        /// <summary>
        ///     Konwersja wartości właściwości obiektu do skrótu SHA512
        ///     Convert object property value to SHA512 hash 
        /// </summary>
        public string AsSHA512Hash()
        {
            return ObjectHelper.ConvertObjectValuesToSHA512Hash(this, ";");
        }

        #endregion

    }

    #endregion
}
