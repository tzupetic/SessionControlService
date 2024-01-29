using System.Text.Json.Serialization;
using System.Text.Json;

namespace SessionControlService.Helpers;

public class CamelCaseStringEnumConverter : JsonStringEnumConverter
{
    public CamelCaseStringEnumConverter()
        : base(JsonNamingPolicy.CamelCase) { }
}
