using System.ComponentModel;

namespace ResponseCrafter.Dtos;

public class ErrorResponse
{
   [DefaultValue("0HN8FG4CDDD6R:00000003")]
   public required string RequestId { get; set; }

   [DefaultValue("b224190af31b6eb950ce4b84c9d44fae")]
   public required string TraceId { get; set; }

   [DefaultValue("POST - localhost/users")]
   public required string Instance { get; set; }

   [DefaultValue(400)]
   public int StatusCode { get; init; }

   [DefaultValue("BadRequestException")]
   public required string Type { get; set; }

   [DefaultValue(null)]
   public Dictionary<string, string>? Errors { get; set; }

   [DefaultValue("duplicate_user")]
   public required string Message { get; set; }
}