using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Reusables.Util.Extensions;

namespace Reusables.Serialization.Newtonsoft.Extensions
{
    public static class GenericExtensions
    {
        public static string ToJson<T>(this T source)
        {
            return JsonConvert.SerializeObject(source);
        }

        public static T FromJson<T>(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return JsonConvert.DeserializeObject<T>(source);
        }

        public static string ToXml<T>(this T source)
        {
            using (var sourceStream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof (T));

                serializer.WriteObject(sourceStream, source);
                sourceStream.TryToRewind();

                using (var streamReader = new StreamReader(sourceStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static T FromXml<T>(this string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(source);
                    writer.Flush();
                    stream.TryToRewind();

                    var serializer = new DataContractSerializer(typeof (T));

                    return (T) serializer.ReadObject(stream);
                }
            }
        }
    }
}
