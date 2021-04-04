#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NetAppCommon.Extensions.Caching.Distributed;
using NetAppCommon.Logging.ClientMessageInspector.Models.Base;
using NetAppCommon.Logging.ClientMessageInspector.Repositories.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace NetAppCommon.Logging.ClientMessageInspector.Repositories
{
    public sealed class DistributedCacheRepository : IDistributedCacheRepository
    {
        private readonly ICommonDistributedCache _cache;

        private readonly Guid _guid = Helpers.Object.ObjectHelper.GuidFromString(Assembly.GetExecutingAssembly().FullName);

        private IDictionary<string, MessageInspectorModel> _dictionary;

        public DistributedCacheRepository()
        {
            IOptions<MemoryDistributedCacheOptions> options = Options.Create(new MemoryDistributedCacheOptions());
            _cache = new CommonMemoryDistributedCache(options);
            _dictionary = Get();
        }

        public DistributedCacheRepository(Guid guid)
        {
            _guid = guid;
            IOptions<MemoryDistributedCacheOptions> options = Options.Create(new MemoryDistributedCacheOptions());
            _cache = new CommonMemoryDistributedCache(options);
            _dictionary = Get();
        }

        public DistributedCacheRepository(ICommonDistributedCache cache)
        {
            _cache = cache;
            _dictionary = Get();
        }

        public Guid GetGuid() => _guid;

        public string GetGuidAsString() => _guid.ToString();

        public IDictionary<string, MessageInspectorModel> Get(Guid guid)
        {
            try
            {
                return (IDictionary<string, MessageInspectorModel>)_cache
                           ?.Get<IDictionary<string, MessageInspectorModel>>(guid.ToString()) ??
                       new Dictionary<string, MessageInspectorModel>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public void Remove() => _cache?.Remove(GetGuid().ToString());

        public void Remove(string key) => _cache?.Remove(key);

        public void Remove(Guid guid) => _cache?.Remove(guid.ToString());

        public IDictionary<string, MessageInspectorModel> Get()
        {
            try
            {
                return (IDictionary<string, MessageInspectorModel>)_cache
                           ?.Get<IDictionary<string, MessageInspectorModel>>(_guid.ToString()) ??
                       new Dictionary<string, MessageInspectorModel>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public List<MessageInspectorModel> ToList()
        {
            try
            {
                _dictionary = _dictionary = Get();
                if (null != _dictionary &&
                    _dictionary.Values.Count > 0
                )
                {
                    return _dictionary.Values?.ToList();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public List<TDestination> ToList<TDestination>() where TDestination : MessageInspectorModel, new()
        {
            try
            {
                _dictionary = _dictionary = Get();
                if (null != _dictionary &&
                    _dictionary.Values.Count > 0
                )
                {
                    return _dictionary.Values?.Select(i => i.Cast<TDestination>()).ToList();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public List<TDestination> ToList<TDestination>(Guid guid) where TDestination : MessageInspectorModel, new()
        {
            try
            {
                _dictionary = _dictionary = Get(guid);
                if (null != _dictionary &&
                    _dictionary.Values.Count > 0
                )
                {
                    return _dictionary.Values?.Select(i => i.Cast<TDestination>()).ToList();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public MessageInspectorModel Set(string key, MessageInspectorModel model)
        {
            try
            {
                _dictionary = Get();

                if (null != _cache &&
                    null != _dictionary
                )
                {
                    if (!_dictionary.ContainsKey(key))
                    {
                        _dictionary.Add(key, model);
                    }

                    _cache
                        .Set<IDictionary<string, MessageInspectorModel>>(
                            _guid.ToString(), _dictionary);
                }

                return model;
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public MessageInspectorModel Get(string key)
        {
            try
            {
                _dictionary = Get();

                if (null != _cache &&
                    null != _dictionary &&
                    _dictionary.TryGetValue(key, out MessageInspectorModel value))
                {
                    return value;
                }

                return null;
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}
