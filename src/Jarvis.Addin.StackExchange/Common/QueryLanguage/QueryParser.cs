using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Jarvis.Addin.StackExchange.Common.QueryLanguage
{
    public class QueryParser<TOption> : IQueryParser<TOption> where TOption : new()
    {
        private const string KeyGroup = "key";
        private const string ValueGroup = "value";
        private readonly Regex _kvpRegexp= new Regex($@"(?<{KeyGroup}>[\S^:]+):(?<{ValueGroup}>[\S^:]+)", RegexOptions.Compiled);
        private readonly Dictionary<string, PropertyInfo> _propertyInfos;

        public QueryParser()
        {
            _propertyInfos = typeof(TOption)
                .GetProperties()
                .ToDictionary(p => p.Name, p => p, StringComparer.InvariantCultureIgnoreCase);
        }

        public IDictionary<string, IList<string>> Interpretate(string query, out string unmatched)
        {
            var result = new Dictionary<string, IList<string>>();
            var matches = _kvpRegexp.Matches(query);
            unmatched = query;

            foreach (Match match in matches)
            {
                var key = match.Groups[KeyGroup].Value;
                var value = match.Groups[ValueGroup].Value;
                unmatched = unmatched.Replace(match.Value, string.Empty).Trim();

                if(!result.TryGetValue(key, out var values))
                {
                    values = new List<string>();
                    result[key] = values;
                }

                values.Add(value);
            }

            return result;
        }

        public TOption Bind(string query, out IDictionary<string, IList<string>> unbound, out string unmatched)
        {
            var option = new TOption();
            unbound = new  Dictionary<string, IList<string>>();
            var available = Interpretate(query, out unmatched);

            foreach (var key in available.Keys)
            {
                if (_propertyInfos.TryGetValue(key, out var propInfo))
                {
                    if (propInfo.PropertyType.IsAssignableFrom(typeof(string)))
                    {
                        propInfo.SetValue(option, available[key].LastOrDefault());
                        continue;
                    }
                    if (propInfo.PropertyType.IsAssignableFrom(typeof(IList<string>)))
                    {
                        propInfo.SetValue(option, available[key]);
                        continue;
                    }
                    if (propInfo.PropertyType.IsAssignableFrom(typeof(DateTime)))
                    {
                        if (DateTime.TryParse(available[key].LastOrDefault(), out var dateTime))
                        {
                            propInfo.SetValue(option, dateTime);
                            continue;
                        }
                    }
                    if (propInfo.PropertyType.IsEnum)
                    {
                        try
                        {
                            var parsedEnum = Enum.Parse(propInfo.PropertyType, available[key].LastOrDefault(), true);
                            propInfo.SetValue(option, parsedEnum);
                        }
                        catch (Exception)
                        {
                            /* Pokemon handling */
                        }
                    }
                }
                else
                {
                    unbound[key] = available[key];
                }
            }

            return option;
        }
    }
}
