using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FWTL.Infrastructure.Cache
{
    public static class CacheKeyBuilder
    {
        public static string Build<TKey, TModel>(TModel model)
        {
            var name = typeof(TKey).Name;
            PropertyInfo[] properties = typeof(TModel).GetProperties();
            List<string> values = new List<string>();
            foreach (PropertyInfo property in properties)
            {
                switch (property.PropertyType.Name)
                {
                    case (nameof(DateTime)):
                        {
                            var value = (DateTime)property.GetValue(model);
                            values.Add(value.Ticks.ToString());
                            break;
                        }
                    default:
                        {
                            if ((property.PropertyType.IsClass && property.PropertyType != typeof(string)) || property.PropertyType.IsArray)
                            {
                                throw new NotImplementedException("Only simple types are allowed");
                            }

                            values.Add(property.GetValue(model)?.ToString() ?? "null");
                            break;
                        }
                }
            }

            return $"{name}." + string.Join(".", values);
        }

        public static string Build<TKey, TModel>(TModel model, params Expression<Func<TModel, object>>[] properties)
        {
            var name = typeof(TKey).Name;
            List<string> values = new List<string>();
            foreach (var property in properties)
            {
                object value = null;
                try
                {
                    value = property.Compile()(model);
                }
                catch { }

                if (value == null)
                {
                    values.Add("null");
                    continue;
                }

                Type propertyType = value.GetType();

                switch (propertyType.Name)
                {
                    case (nameof(DateTime)):
                        {
                            long ticks = ((DateTime)value).Ticks;
                            values.Add(ticks.ToString());
                            break;
                        }
                    default:
                        {
                            if ((propertyType.IsClass && propertyType != typeof(string)) || propertyType.IsArray)
                            {
                                throw new NotImplementedException("Only simple types are allowed");
                            }

                            values.Add(value?.ToString() ?? "null");
                            break;
                        }
                }
            }

            return $"{name}." + string.Join(".", values);
        }
    }
}