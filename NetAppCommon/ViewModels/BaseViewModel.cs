#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#endregion

#region namespace

namespace NetAppCommon.ViewModels
{
    public class BaseViewModel<TCollection> : INotifyPropertyChanged
    {
        #region public BaseViewModel...

        public BaseViewModel()
        {
        }

        #endregion

        #region public BaseViewModel...

        public BaseViewModel(DateTime? dateOfCreateFrom, DateTime? dateOfCreateTo, int total = 0,
            ICollection<TCollection> data = null)
        {
            DateOfCreateFrom = dateOfCreateFrom;
            DateOfCreateTo = dateOfCreateTo;
            Total = total;
            Data = data;
        }

        #endregion

        #region private DateTime _dateOfCreateFrom; public DateTime DateOfCreateFrom

        private DateTime? _dateOfCreateFrom = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        [JsonProperty(nameof(DateOfCreateFrom))]
        [Display(Name = "Data utworzenia od", Prompt = "Wpisz lub wybierz datę utworzenia od",
            Description = "Data utworzenia od")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? DateOfCreateFrom
        {
            get => _dateOfCreateFrom;
            set
            {
                if (value != _dateOfCreateFrom)
                {
                    _dateOfCreateFrom = value;
                    OnPropertyChanged(nameof(DateOfCreateFrom));
                }
            }
        }

        #endregion

        #region private DateTime _dateOfCreateTo; public DateTime DateOfCreateTo

        private DateTime? _dateOfCreateTo = new(DateTime.Now.Year, DateTime.Now.Month,
            DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        [JsonProperty(nameof(DateOfCreateTo))]
        [Display(Name = "Data utworzenia do", Prompt = "Wpisz lub wybierz datę utworzenia do",
            Description = "Data utworzenia do")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? DateOfCreateTo
        {
            get => _dateOfCreateTo;
            set
            {
                if (value != _dateOfCreateTo)
                {
                    _dateOfCreateTo = value;
                    OnPropertyChanged(nameof(DateOfCreateTo));
                }
            }
        }

        #endregion

        #region private string _text;

        private string _text;

        [JsonProperty(nameof(Text))]
        [Display(Name = "Tekst wyszukiwania", Prompt = "Wpisz tekst wyszukiwania",
            Description = "Tekst wyszukiwania, tekst może być rozdzielony separatorem , lub ;")]
        public string Text
        {
            get => _text;
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        #endregion

        #region private int? _total = 0; public int? Total

        private int? _total = 0;

        [JsonProperty(nameof(Total))]
        [Display(Name = "Ilość znalezionych rekordów", Prompt = "Wpisz ilość znalezionych rekordów",
            Description = "Ilość znalezionych rekordów")]
        public int? Total
        {
            get => _total;
            private set
            {
                if (value != _total)
                {
                    _total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        #endregion

        #region private ICollection<TCollection> _data...

        private ICollection<TCollection> _data;

        [JsonProperty("Data")]
        [Display(Name = "Lista znalezionych rekordów", Prompt = "Ustaw listę znalezionych rekordów",
            Description = "Ilość znalezionych rekordów")]
        public ICollection<TCollection> Data
        {
            get => _data;
            set
            {
                if (!Equals(value, _data))
                {
                    _data = value;
                    _total = _data.Count;
                    OnPropertyChanged(nameof(Data));
                }
            }
        }

        #endregion

        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #region private virtual void OnPropertyChanged(PropertyChangedEventArgs args)

        /// <summary>
        ///     private virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        /// </summary>
        /// <param name="args"></param>
        public void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        #endregion

        #region public void OnPropertyChanged(string propertyName)

        /// <summary>
        ///     public void OnPropertyChanged(string propertyName)
        ///     public void OnPropertyChanged(string propertyName)
        /// </summary>
        /// <param name="propertyName">
        /// </param>
        public void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        #endregion

        #endregion
    }
}

#endregion
