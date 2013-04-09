using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ToileDeFond.Utilities
{
    public static class Extensions
    {
        #region Reflection

        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        public static PropertyInfo ScanTypeForPropertyNamedOfType(this Type type, string name, Type propertyType)
        {
            return type.GetProperties().FirstOrDefault(property => property.PropertyType == propertyType && property.Name == name);
        }

        public static bool Inherits(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        public static string ScanTypeForPropertyNamedOrFirstPropertyOfType(this Type type, string name, Type propertyType)
        {
            var propertyNamedName = type.GetProperty(name);

            if (propertyNamedName != null && propertyNamedName.PropertyType == propertyType)
                return name;

            foreach (var property in type.GetProperties().Where(property => property.PropertyType == propertyType))
            {
                return property.Name;
            }

            throw new NotImplementedException("No property of type string for many-to-many purpose");
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers["X-Requested-With"] == "XMLHttpRequest"));
        }

        public static IEnumerable<PropertyInfo> GetOwnProperties(this Type type)
        {
            return from property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                   let getMethod = property.GetGetMethod(false)
                   where getMethod.GetBaseDefinition() == getMethod
                   select property;
        }

        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }



        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static Type GetTypeOrUnderlyingTypeIfNullable(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nc = new NullableConverter(type);
                type = nc.UnderlyingType;
            }

            return type;
        }

        public static bool HasAttribute<TType>(this MemberInfo memberInfo)
        {
            return Attribute.IsDefined(memberInfo, typeof(TType));
        }

        #region Resources

        public static IEnumerable<string> GetResourceNamesUnder(this Assembly assembly, string folder)
        {
            return assembly.GetManifestResourceNames().Where(r => r.StartsWith(folder));
        }

        public static string GetTextOfResource(this Assembly assembly, string resourceName)
        {
            string text = null;

            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream != null)
                    using (var streamReader = new StreamReader(resourceStream))
                    {
                        text = streamReader.ReadToEnd();
                    }
            }

            return text;
        }

        #endregion

        #endregion

        #region Strings

        public static string PathCombine(this string string1, string string2)
        {
            char separator = '\\';
            const char inversedSeparator = '/';

            if (string1.Contains(inversedSeparator) || string2.Contains(inversedSeparator))
                separator = inversedSeparator;

            var spliter = new[] { separator };

            var words = string1.Split(spliter).ToList();
            words.AddRange(string2.Split(spliter));

            return string.Join(separator.ToString(), words);
        }

        public static bool ContainsCaseInsensitive(this string source, string value)
        {
            return source.ToLower().Contains(value.ToLower());
        }

        public static string Format(this string format, params object[] args)
        {
            return String.Format(format, args);
        }

        public static bool IsNullOrEmpty(this string s1)
        {
            return String.IsNullOrEmpty(s1);
        }

        public static string ReplaceLast(this string text, string search, string replace)
        {
            if (text == null)
                return null;
            var pos = text.LastIndexOf(search, StringComparison.OrdinalIgnoreCase);

            return pos < 0 ? text : text.Substring(0, pos) + replace
                + text.Substring(pos + search.Length);
        }

        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            var sb = new StringBuilder();

            var previousIndex = 0;
            var index = str.IndexOf(oldValue, comparison);

            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }

            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.OrdinalIgnoreCase);

            return pos < 0 ? text : text.Substring(0, pos) + replace
                + text.Substring(pos + search.Length);
        }

        public static string SubstringBeforeLastIndexOf(this string text, char character)
        {
            return text.Substring(0, text.LastIndexOf(character));
        }

        public static string SubstringAfterLastIndexOf(this string text, char character)
        {
            var lastIndexOf = text.LastIndexOf(character);

            if (lastIndexOf < 0)
            {
                return text;
            }

            lastIndexOf += 1;

            return text.Substring(lastIndexOf, text.Length - lastIndexOf);
        }

        #endregion

        #region To Organize

        public static bool TryGetFirst<TSource>(this IEnumerable<TSource> source,
                                         Func<TSource, bool> predicate,
                                         out TSource first)
        {
            foreach (TSource item in source)
            {
                if (predicate(item))
                {
                    first = item;
                    return true;
                }
            }

            first = default(TSource);
            return false;
        }

        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        // Works in C#3/VS2008:
        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

        public static string DecodeBase64(this string str)
        {
            return System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(str));
        }

        public static byte[] ToByteArray<T>(this string str) where T : Encoding
        {
            Encoding enc = Activator.CreateInstance<T>();
            return enc.GetBytes(str);
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dic, K key, V defaultVal)
        {
            V ret;
            bool found = dic.TryGetValue(key, out ret);
            if (found) { return ret; }
            return defaultVal;
        }

        public static bool IsGenericList(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return true;
            }

            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType)
                {
                    if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        // if needed, you can also return the type used as generic argument
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<Type> GetTopLevelInterfaces(this Type t)
        {
            var allInterfaces = t.GetInterfaces();

            if (t.BaseType == null)
                return allInterfaces;

            return allInterfaces
                .Where(x => !allInterfaces.Any(y => y.GetInterfaces().Contains(x)))
                .Except(t.BaseType.GetInterfaces());
        }

        public static T GetValue<T>(this NameValueCollection collection, string key)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            var value = collection[key];
            var type = typeof(T);

            if (string.IsNullOrEmpty(value))
            {
                if (type == typeof(string) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    return default(T);

                throw new ArgumentOutOfRangeException("key");
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nc = new NullableConverter(type);
                type = nc.UnderlyingType;
            }

            if (type.IsEnum)
                return (T)Enum.Parse(type, value, false);

            var converter = TypeDescriptor.GetConverter(type);

            if (converter.CanConvertFrom(typeof(string)))
            {
                var converted = default(T);

                try
                {
                    converted = (T)converter.ConvertFrom(value);
                }
                catch
                {
                }

                return converted;
            }

            return (T)Convert.ChangeType(value, type);
        }

        public static bool Contains(this string str, string value, StringComparison comparison)
        {
            return str.IndexOf(value, comparison) != -1;
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> pItems) where T : class
        {
            return pItems.Where(i => i != null);
        }

        public static void ForEach<T>(this IEnumerable<T> pItems, Action<T> pAction)
        {
            foreach (var item in pItems)
            {
                pAction(item);
            }
        }

        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }

        /// <summary>
        ///     Capitalise la première lettre d'une chaine de caractère.
        ///     Ex.: george harrison => George harrison
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Capitalize(this string str)
        {
            if (str.Length == 0)
                return str;

            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        /// <summary>
        ///     Convertie la chaîne caractère en TitleCase.
        ///     Ex.: geoRGe hARrison => George Harrison
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            if (str.Length == 0)
                return str;

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str);
        }

        public static string ToAccentInsensitive(this string str)
        {
            if (IsNullOrEmpty(str))
                return str;

            str = str.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            for (var ich = 0; ich < str.Length; ich++)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(str[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(str[ich]);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).Replace("&", string.Empty);
        }

        public static string ToCodeSafe(this string str)
        {
            var result = str.ToAccentInsensitive();
            result = Regex.Replace(result, "[^a-zA-Z0-9\\s]", "");
            result = Regex.Replace(result, "[\\s]", "");
            result = result.TrimStart().TrimEnd();
            result = result.Trim();

            return result;
        }

        /// <summary>
        ///     Convertie une chaîne de caractère en array de bytes.
        /// </summary>
        public static byte[] ToBytes(this string str, Encoding charEncoding)
        {
            return charEncoding.GetBytes(str);
        }

        public static void Raise(this EventHandler eventHandler,
                                 object sender, EventArgs e)
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }

        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static bool IsRegexMatch(this string inputString, string matchPattern)
        {
            return Regex.IsMatch(inputString, matchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        public static bool EqualsIgnoreCase(this string s1, string s2)
        {
            if (s1 == null)
            {
                return (s2 == null);
            }
            return s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
        }


        public static IDictionary<string, string> ToDictionary(this NameValueCollection source)
        {
            return source.Cast<string>().Select(s => new { Key = s, Value = source[s] }).ToDictionary(p => p.Key, p => p.Value);
        }

        public static int ToInt(this string source, int defaultValue = 0)
        {
            var result = defaultValue;
            Int32.TryParse(source, out result);
            return result;
        }

        public static int? ToNullableInt(this string source)
        {
            int result;
            if (Int32.TryParse(source, out result))
            {
                return result;
            }

            return null;
        }

        public static DateTime? ToNullableDateTime(this string source)
        {
            DateTime result;
            if (DateTime.TryParse(source, out result))
            {
                return result;
            }

            return null;
        }

        public static bool ToBool(this string source, bool defaultValue = false)
        {
            var result = defaultValue;
            Boolean.TryParse(source, out result);
            return result;
        }

        public static Guid ToGuid(this string source)
        {
            if (source.IsGuid())
            {
                return new Guid(source);
            }

            return Guid.Empty;
        }

        public static bool IsGuid(this string candidate)
        {
            Guid dummy;
            return candidate.IsGuid(out dummy);
        }

        public static bool IsGuid(this string candidate, out Guid output)
        {
            var isValid = false;

            output = Guid.Empty;

            if (candidate != null)
            {
                if (RegularExpressions.Guid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }

            return isValid;
        }

        public static bool IsGuid(this string candidate, ref Guid? output)
        {
            var isValid = false;

            if (candidate != null)
            {
                if (RegularExpressions.Guid.IsMatch(candidate))
                {
                    output = new Guid(candidate);
                    isValid = true;
                }
            }

            return isValid;
        }

        #endregion

        #region URI

        public static StringBuilder AddQueryParam(this StringBuilder source, string key, string value)
        {
            bool hasQuery = false;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == '?')
                {
                    hasQuery = true;
                    break;
                }
            }

            string delim;
            if (!hasQuery)
            {
                delim = "?";
            }
            else if ((source[source.Length - 1] == '?')
                || (source[source.Length - 1] == '&'))
            {
                delim = string.Empty;
            }
            else
            {
                delim = "&";
            }

            return source.Append(delim).Append(HttpUtility.UrlEncode(key))
                .Append("=").Append(HttpUtility.UrlEncode(value));
        }

        public static StringBuilder AddQueryParamIfAlreadyThere(this StringBuilder source, string key)
        {
            if(!HttpContext.Current.Request.QueryString.AllKeys.Contains(key))
            {
                return source;
            }

            foreach (var value in HttpContext.Current.Request.QueryString.GetValues(key))
            {
                source.AddQueryParam(key, value);
            }

            return source;
        }

        public static string AddQueryParam(this string source, string key, string value)
        {
            string delim;
            if ((source == null) || !source.Contains("?"))
            {
                delim = "?";
            }
            else if (source.EndsWith("?") || source.EndsWith("&"))
            {
                delim = string.Empty;
            }
            else
            {
                delim = "&";
            }

            return source + delim + HttpUtility.UrlEncode(key)
                + "=" + HttpUtility.UrlEncode(value);
        }


        #endregion
    }
}