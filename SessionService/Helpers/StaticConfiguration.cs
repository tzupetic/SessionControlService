namespace SessionControlService.Helpers;

public class StaticConfiguration
{
    private static IConfiguration _configuration;

    public static string ConnectionStringsDB =>
        _configuration.GetValue<string>("ConnectionStrings:SessionDB");

    public static string ConnectionStringsRedisDB =>
        _configuration.GetValue<string>("ConnectionStrings:RedisDB");
    public static string AppSettingsSecret => _configuration.GetValue<string>("AppSettings:Secret");
    public static string AppSettingsExpirationDays =>
        _configuration.GetValue<string>("AppSettings:ExpirationDays");

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}
