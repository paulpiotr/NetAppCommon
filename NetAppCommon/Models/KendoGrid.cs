#region using

using Newtonsoft.Json;

#endregion

namespace NetAppCommon.Models
{
    #region public class KendoGrid<T>

    /// <summary>
    ///     Klasa modelu dla widoku KendoGrid
    ///     The model class for the KendoGrid view
    /// </summary>
    /// <typeparam name="T">
    ///     Typ danych jako T
    ///     Data type as T
    /// </typeparam>
    public class KendoGrid<T>
    {
        #region public int Total { get; set; }

        /// <summary>
        ///     Ilość rekordów jako int
        ///     Number of records as int
        /// </summary>
        [JsonProperty(nameof(Total))]
        public int Total { get; set; }

        #endregion

        #region public T Data { get; set; }

        /// <summary>
        ///     Dane jako typ T
        ///     Data as T type
        /// </summary>
        [JsonProperty(nameof(Data))]
        public T Data { get; set; }

        #endregion
    }

    #endregion
}
