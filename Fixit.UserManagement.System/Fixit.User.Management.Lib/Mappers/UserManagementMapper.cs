﻿using AutoMapper;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.User.Management.Lib.Models;

namespace Fixit.User.Management.Lib.Mappers
{
  public class UserManagementMapper : Profile
  {
    public UserManagementMapper()
    {
      #region UserAccountConfiguration
      #endregion

      #region UserProfileConfiguration
      CreateMap<UserProfileDto, UserDocument>()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto.ProfilePictureUrl))
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto.FirstName))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto.LastName))
        .ForMember(document => document.Address, opts => opts.MapFrom(dto => dto.Address))
        .ForMember(document => document.CreatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.Documents, opts => opts.Ignore())
        .ForMember(document => document.EntityId, opts => opts.Ignore())
        .ForMember(document => document.Gender, opts => opts.Ignore())
        .ForMember(document => document.id, opts => opts.Ignore())
        .ForMember(document => document.Rating, opts => opts.Ignore())
        .ForMember(document => document.Role, opts => opts.Ignore())
        .ForMember(document => document.State, opts => opts.Ignore())
        .ForMember(document => document.Status, opts => opts.Ignore())
        .ForMember(document => document.TelephoneNumber, opts => opts.Ignore())
        .ForMember(document => document.UpdatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.UserPrincipalName, opts => opts.Ignore())
        .ReverseMap();

      CreateMap<UserProfileUpdateRequestDto, UserDocument>()
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto.FirstName))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto.LastName))
        .ForMember(document => document.Address, opts => opts.MapFrom(dto => dto.Address))
        .ForMember(document => document.CreatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.Documents, opts => opts.Ignore())
        .ForMember(document => document.EntityId, opts => opts.Ignore())
        .ForMember(document => document.Gender, opts => opts.Ignore())
        .ForMember(document => document.id, opts => opts.Ignore())
        .ForMember(document => document.ProfilePictureUrl, opts => opts.Ignore())
        .ForMember(document => document.Rating, opts => opts.Ignore())
        .ForMember(document => document.Role, opts => opts.Ignore())
        .ForMember(document => document.State, opts => opts.Ignore())
        .ForMember(document => document.Status, opts => opts.Ignore())
        .ForMember(document => document.TelephoneNumber, opts => opts.Ignore())
        .ForMember(document => document.UpdatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.UserPrincipalName, opts => opts.Ignore())
        .ReverseMap();

      CreateMap<UserProfileInformationDto, UserDocument>()
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto.FirstName))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto.LastName))
        .ForMember(document => document.Address, opts => opts.MapFrom(dto => dto.Address))
        .ForMember(document => document.CreatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.Documents, opts => opts.Ignore())
        .ForMember(document => document.EntityId, opts => opts.Ignore())
        .ForMember(document => document.Gender, opts => opts.Ignore())
        .ForMember(document => document.id, opts => opts.Ignore())
        .ForMember(document => document.ProfilePictureUrl, opts => opts.Ignore())
        .ForMember(document => document.Rating, opts => opts.Ignore())
        .ForMember(document => document.Role, opts => opts.Ignore())
        .ForMember(document => document.State, opts => opts.Ignore())
        .ForMember(document => document.Status, opts => opts.Ignore())
        .ForMember(document => document.TelephoneNumber, opts => opts.Ignore())
        .ForMember(document => document.UpdatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.UserPrincipalName, opts => opts.Ignore())
        .ReverseMap();

      CreateMap<UserProfilePictureUpdateRequestDto, UserDocument>()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto.ProfilePictureUrl))
        .ForMember(document => document.Address, opts => opts.Ignore())
        .ForMember(document => document.CreatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.Documents, opts => opts.Ignore())
        .ForMember(document => document.EntityId, opts => opts.Ignore())
        .ForMember(document => document.FirstName, opts => opts.Ignore())
        .ForMember(document => document.Gender, opts => opts.Ignore())
        .ForMember(document => document.id, opts => opts.Ignore())
        .ForMember(document => document.LastName, opts => opts.Ignore())
        .ForMember(document => document.Rating, opts => opts.Ignore())
        .ForMember(document => document.Role, opts => opts.Ignore())
        .ForMember(document => document.State, opts => opts.Ignore())
        .ForMember(document => document.Status, opts => opts.Ignore())
        .ForMember(document => document.TelephoneNumber, opts => opts.Ignore())
        .ForMember(document => document.UpdatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.UserPrincipalName, opts => opts.Ignore())
        .ReverseMap();

      CreateMap<UserProfilePictureDto, UserDocument>()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto.ProfilePictureUrl))
        .ForMember(document => document.Address, opts => opts.Ignore())
        .ForMember(document => document.CreatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.Documents, opts => opts.Ignore())
        .ForMember(document => document.EntityId, opts => opts.Ignore())
        .ForMember(document => document.FirstName, opts => opts.Ignore())
        .ForMember(document => document.Gender, opts => opts.Ignore())
        .ForMember(document => document.id, opts => opts.Ignore())
        .ForMember(document => document.LastName, opts => opts.Ignore())
        .ForMember(document => document.Rating, opts => opts.Ignore())
        .ForMember(document => document.Role, opts => opts.Ignore())
        .ForMember(document => document.State, opts => opts.Ignore())
        .ForMember(document => document.Status, opts => opts.Ignore())
        .ForMember(document => document.TelephoneNumber, opts => opts.Ignore())
        .ForMember(document => document.UpdatedTimestampsUtc, opts => opts.Ignore())
        .ForMember(document => document.UserPrincipalName, opts => opts.Ignore())
        .ReverseMap();
      #endregion
    }
  }
}