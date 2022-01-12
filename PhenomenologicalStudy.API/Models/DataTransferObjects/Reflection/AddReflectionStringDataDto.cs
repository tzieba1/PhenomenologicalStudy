﻿using PhenomenologicalStudy.API.Models.DataTransferObjects.Capture;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection
{
  public class AddReflectionStringDataDto
  {
    public StringCaptureDto Capture { get; set; }
    public DateTimeOffset CreationTime { get; set; }
  }
}
