using Newtonsoft.Json;
using System;

namespace PvReport.Services.Storage
{
    public static class SerializationService
    {
        public static T JsonDeserialize<T>(string jsonString)
        {
            try
            {
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
            return JsonConvert.SerializeObject(obj);
        }
    }
}
