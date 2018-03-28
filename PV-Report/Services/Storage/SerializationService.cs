using Newtonsoft.Json;
using System;
using System.Globalization;

namespace PvReport.Services.Storage
{
    public static class SerializationService
    {
        public static T JsonDeserialize<T>(string jsonString)
        {
            try
            {
                SetJsonSettings();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception e)
            {
                // todo: handle error
            }

            return default(T);
        }

        public static string JsonSerialize(object obj)
        {
            SetJsonSettings();
            return JsonConvert.SerializeObject(obj);
        }

        private static void SetJsonSettings()
        {
            JsonConvert.DefaultSettings =
                () => new JsonSerializerSettings
                {
#if DEBUG
                    Formatting = Formatting.Indented,
#endif
                    Culture = new CultureInfo("de-DE"),
                    //DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    //DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    NullValueHandling = NullValueHandling.Include
                };
        }
    }
}
