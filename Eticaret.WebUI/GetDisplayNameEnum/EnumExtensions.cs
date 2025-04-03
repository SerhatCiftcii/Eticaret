using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Eticaret.WebUI.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString(); // Eğer Display adı yoksa, enum ismini döndür
        }
    }
}
