using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Mail;

namespace Inventory.Web
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            // SetString موجود فقط لو ISession معرف بشكل صحيح
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
