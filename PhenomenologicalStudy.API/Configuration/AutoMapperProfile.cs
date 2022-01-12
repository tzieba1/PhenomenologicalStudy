using AutoMapper;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Capture;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Child;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Comment;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Emotion;
using PhenomenologicalStudy.API.Models.DataTransferObjects.Reflection;
using PhenomenologicalStudy.API.Models.DataTransferObjects.ReflectionChild;
using PhenomenologicalStudy.API.Models.DataTransferObjects.User;
using PhenomenologicalStudy.API.Models.ManyToMany;

namespace PhenomenologicalStudy.API.Configuration
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<Reflection, GetReflectionDto>();            // _mapper.Map<GetReflectionDto>({Reflection})
      CreateMap<AddReflectionByteDataDto, Reflection>();    // _mapper.Map<Reflection>({AddReflectionByteDataDto})

      CreateMap<Capture, GetCaptureDto>();                  // _mapper.Map<GetCaptureDto>({Capture})
      CreateMap<AddCaptureDto, Capture>();                  // _mapper.Map<Capture>({AddCaptureDto})

      CreateMap<Comment, GetCommentDto>();                  // _mapper.Map<GetCommentDto>({Comment})
      CreateMap<ReflectionChild, GetReflectionChildDto>();  // _mapper.Map<GetReflectionChildDto>({ReflectionChild})
      CreateMap<Child, GetChildDto>();                      // _mapper.Map<GetChildDto>({Child})
      CreateMap<Emotion, GetEmotionDto>();                  // _mapper.Map<GetEmotionDto>({Emotion})
      CreateMap<Reflection, GetUserReflectionDto>();        // _mapper.Map<GetReflectionUserDto>({Reflection})
      CreateMap<User, GetUserDto>();                        // _mapper.Map<GetUserDto>({User})

      CreateMap<AddChildDto, Child>();                      // _mapper.Map<Child>({AddChildDto})
      CreateMap<AddEmotionDto, Emotion>();                  // _mapper.Map<Emotion>({AddEmotionDto})
      CreateMap<AddUserRoleDto, Role>();                    // _mapper.Map<Role>({AddUserRoleDto})
    }
  }
}
