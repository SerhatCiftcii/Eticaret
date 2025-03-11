namespace Eticaret.WebUI.ExtensionMethods;
using Newtonsoft.Json;

    public static class SessionExtensionsMethods
{
    public static void SetJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));//gelicek keyi al sonra jsona çevir
    }
    public static T? GetJson<T>(this ISession session, string key) where T : class
    {
        var sessionData = session.GetString(key);
        return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData); //objeyi nesneye çevir
    }
}
