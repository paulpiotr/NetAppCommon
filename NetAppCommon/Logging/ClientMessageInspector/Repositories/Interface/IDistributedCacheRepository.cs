#region using

using System;
using System.Collections.Generic;
using NetAppCommon.Logging.ClientMessageInspector.Models.Base;

#endregion

namespace NetAppCommon.Logging.ClientMessageInspector.Repositories.Interface
{
    public interface IDistributedCacheRepository
    {
        public Guid GetGuid();

        public string GetGuidAsString();

        public IDictionary<string, MessageInspectorModel> Get();

        public List<MessageInspectorModel> ToList();

        public List<TDestination> ToList<TDestination>() where TDestination : MessageInspectorModel, new();

        public MessageInspectorModel Set(string key, MessageInspectorModel model);

        public MessageInspectorModel Get(string key);
    }
}
