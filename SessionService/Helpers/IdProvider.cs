using RT.Comb;

namespace SessionControlService.Helpers;

public class IdProvider
{
    public static Guid NewId()
    {
        return Provider.PostgreSql.Create();
    }
}
