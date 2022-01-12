using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReflectionAPI.Models
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
