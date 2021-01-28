#region using

using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace NetAppCommon.ObjectMapper.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ObjectMapperAttribute : Attribute
    {
        private string _assemblyNameSource;

        private string[] _destinationMap;

        private string _propertyNameSource;

        public ObjectMapperAttribute(string assemblyNameSource, string propertyNameSource,
            params string[] destinationMap)
        {
            AssemblyNameSource = assemblyNameSource;
            PropertyNameSource = propertyNameSource;
            DestinationMap = destinationMap;
        }

        public string AssemblyNameSource
        {
            get => _assemblyNameSource;
            private set
            {
                if (value != _assemblyNameSource)
                {
                    _assemblyNameSource = value;
                }
            }
        }

        public string PropertyNameSource
        {
            get => _propertyNameSource;
            private set
            {
                if (value != _propertyNameSource)
                {
                    _propertyNameSource = value;
                }
            }
        }

        public string[] DestinationMap
        {
            get => _destinationMap;
            set
            {
                if (value != _destinationMap)
                {
                    _destinationMap = value;
                }
            }
        }

        public static object GetCustomAttribute(object @object, string propertyName)
        {
            try
            {
                PropertyInfo objectPropertyInfo = @object?.GetType()?.GetProperty(propertyName);
                if (null != objectPropertyInfo)
                {
                    var objectAttribute =
                        (ObjectMapperAttribute)GetCustomAttribute(objectPropertyInfo, typeof(ObjectMapperAttribute));
                    if (null != objectAttribute)
                    {
                        return objectAttribute;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public static object GetCustomAttributeValue(object @object, string propertyName, string attributeName)
        {
            try
            {
                PropertyInfo objectPropertyInfo = @object?.GetType()?.GetProperty(propertyName);
                if (null != objectPropertyInfo)
                {
                    var objectAttribute =
                        (ObjectMapperAttribute)GetCustomAttribute(objectPropertyInfo, typeof(ObjectMapperAttribute));
                    if (null != objectAttribute)
                    {
                        return objectAttribute?.GetType()?.GetProperty(attributeName)?.GetValue(objectAttribute);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public static TDestination SimpleJsonMap<TSource, TDestination>(TSource source, TDestination destination = null)
            where TDestination : class, new()
        {
            try
            {
                destination ??= new TDestination();
                var jsonSource = JsonConvert.SerializeObject(source);
                source.GetType().GetProperties().ToList().ForEach(sourceProperty =>
                {
                    var assemblyNameSource =
                        (string)GetCustomAttributeValue(source, sourceProperty.Name, "AssemblyNameSource");
                    var propertyNameSource =
                        (string)GetCustomAttributeValue(source, sourceProperty.Name, "PropertyNameSource");
                    if (null != assemblyNameSource && source.GetType().Name == assemblyNameSource &&
                        null != propertyNameSource)
                    {
                        var destinationMap =
                            (string[])GetCustomAttributeValue(source, sourceProperty.Name, "DestinationMap");
                        destinationMap.ToList().ForEach(item =>
                        {
                            var destinationMapObject = JObject.Parse(item.ToString());
                            var assemblyNameDestination = destinationMapObject.Value<string>("AssemblyNameDestination");
                            var propertyNameDestination = destinationMapObject.Value<string>("PropertyNameDestination");
                            if (null != assemblyNameDestination && null != propertyNameDestination &&
                                assemblyNameDestination == destination.GetType().Name)
                            {
                                //Console.WriteLine($"assemblyNameSource { assemblyNameSource } propertyNameSource {propertyNameSource} assemblyNameDestination { assemblyNameDestination } propertyNameDestination: { propertyNameDestination }");
                                jsonSource = jsonSource.Replace(propertyNameSource, propertyNameDestination);
                                //Console.WriteLine($"Source {jsonSource}");
                            }
                        });
                    }
                });
                return JsonConvert.DeserializeObject<TDestination>(jsonSource);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return destination;
        }
    }
}
