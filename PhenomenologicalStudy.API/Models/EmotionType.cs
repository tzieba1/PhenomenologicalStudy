using System.Text.Json.Serialization;

namespace PhenomenologicalStudy.API.Models
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum EmotionType
  {
    Happy,
    Sad,
    Angry,
    Confused,
    Crying,
    Frustrated,
    Overwhelmed,
    Shy,
    Laughing
  }
}
