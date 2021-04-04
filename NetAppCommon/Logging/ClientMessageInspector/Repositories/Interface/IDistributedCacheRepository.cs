#region using

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetAppCommon.Logging.ClientMessageInspector.Models.Base;

#endregion

namespace NetAppCommon.Logging.ClientMessageInspector.Repositories.Interface
{
    public interface IDistributedCacheRepository
    {
        /// <summary>
        /// Pobierz Guid, identyfikator pamięci rozproszonej
        /// Get Guid, distributed memory identifier
        /// </summary>
        /// <returns>
        /// Identyfikator pamięci rozproszonej jako Guid
        /// Distributed memory identifier as Guid
        /// </returns>
        public Guid GetGuid();

        /// <summary>
        /// Pobierz string Guid, identyfikator pamięci rozproszonej
        /// Get string Guid, distributed memory identifier
        /// </summary>
        /// <returns>
        /// Identyfikator pamięci rozproszonej jako string z Guid
        /// Distributed memory identifier as string from Guid
        /// </returns>
        public string GetGuidAsString();

        /// <summary>
        /// Pobierz słownik elementów MessageInspectorModel z pamięci rozproszonej
        /// Get the Dictionary of MessageInspectorModel items from distributed memory
        /// </summary>
        /// <returns>
        /// Słownik elementów MessageInspectorModel z pamięci rozproszonej jako IDictionary
        /// Dictionary of MessageInspectorModel elements from distributed memory as IDictionary
        /// </returns>
        public IDictionary<string, MessageInspectorModel> Get();

        public List<MessageInspectorModel> ToList();

        public List<TDestination> ToList<TDestination>() where TDestination : MessageInspectorModel, new();

        public List<TDestination> ToList<TDestination>(Guid guid) where TDestination : MessageInspectorModel, new();

        public MessageInspectorModel Set(string key, MessageInspectorModel model);

        public MessageInspectorModel Get(string key);

        public IDictionary<string, MessageInspectorModel> Get(Guid guid);

        /// <summary>
        /// Usuwa wartość z pamięci rozproszonej
        /// Remove the value from distributed memory
        /// </summary>
        void Remove();

        /// <summary>
        /// Usuwa wartość z podanym kluczem.
        /// Removes the value with the given key.
        /// </summary>
        /// <param name="key">
        /// Ciąg identyfikujący żądaną wartość.
        /// A string identifying the requested value.
        /// </param>
        void Remove(string key);

        /// <summary>
        /// Usuwa wartość z pamięci rozproszonej według klucza Guid
        /// Remove the value from distributed memory by Guid key
        /// </summary>
        /// <param name="guid">
        /// Guid identyfikujący żądaną wartość
        /// A Guid identifying the requested value.
        /// </param>
        void Remove(Guid guid);
    }
}
