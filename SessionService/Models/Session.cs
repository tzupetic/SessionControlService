using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SessionControlService.Helpers;

namespace SessionControlService.Models;

public class Session
{
    public Guid Id { get; set; } = IdProvider.NewId();
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string ContentType { get; set; }
    public SessionState State { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime LastActivityTime { get; set; }
}

[JsonConverter(typeof(CamelCaseStringEnumConverter))]
public enum SessionState
{
    [EnumMember(Value = "play")]
    Play,

    [EnumMember(Value = "pause")]
    Pause,

    [EnumMember(Value = "resume")]
    Resume,

    [EnumMember(Value = "seek")]
    Seek,

    [EnumMember(Value = "stop")]
    Stop,

    [EnumMember(Value = "closed")]
    Closed
}
