using System;

namespace csharpwebsite.Server.Helpers
{
    static class Extension
    {
        public static string ToBase64(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");
        }

        public static Guid ToGuid(this string base64)
        {
           Guid guid = default(Guid);
           base64 = base64.Replace("-", "/").Replace("_", "+") + "==";

           try {
               guid = new Guid(Convert.FromBase64String(base64));
           }
           catch (Exception ex) {
               throw new Exception("Bad Base64 conversion to GUID", ex);
           }

           return guid;
        }
    }
}