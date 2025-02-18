﻿using System.Linq;
using AutoMapper;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Connectors.DataContracts;
using Fixit.Core.DataContracts.Users.Ratings;
using Fixit.Core.DataContracts.Users.Operations.Ratings;
using Fixit.Core.DataContracts.Users.Address;
using System;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Operations.Addresses;
using Fixit.Core.DataContracts.Users.Operations.Licenses;
using Fixit.Core.DataContracts.Users.License;

namespace Fixit.User.Management.Lib.Mappers
{
  public class UserManagementMapper : Profile
  {
    public UserManagementMapper()
    {
      #region UserAccountConfiguration
      CreateMap<UserDocument, UserAccountCreateRequestDto>()
        .ForMember(userCreate => userCreate.Id, opts => opts.MapFrom(document => document != null ? document.id : default))
        .ForMember(userCreate => userCreate.UserPrincipalName, opts => opts.MapFrom(document => document != null ? document.UserPrincipalName : default))
        .ForMember(userCreate => userCreate.Licenses, opts => opts.MapFrom(document => document != null ? document.Licenses: default))
        .ForMember(userCreate => userCreate.FirstName, opts => opts.MapFrom(document => document != null ? document.FirstName : default))
        .ForMember(userCreate => userCreate.LastName, opts => opts.MapFrom(document => document != null ? document.LastName : default))
        .ForMember(userCreate => userCreate.Availability, opts => opts.MapFrom(document => document != null ? document.Availability : default))
        .ForMember(userCreate => userCreate.Role, opts => opts.MapFrom(document => document != null ? document.Role : default))
        .ReverseMap();

      CreateMap<UserDocument, UserAccountDto>()
        .ForMember(userCreate => userCreate.Id, opts => opts.MapFrom(document => document != null ? document.id : default))
        .ForMember(userCreate => userCreate.UserPrincipalName, opts => opts.MapFrom(document => document != null ? document.UserPrincipalName : default))
        .ForMember(userCreate => userCreate.FirstName, opts => opts.MapFrom(document => document != null ? document.FirstName : default))
        .ForMember(userCreate => userCreate.LastName, opts => opts.MapFrom(document => document != null ? document.LastName : default))
        .ForMember(userCreate => userCreate.LastName, opts => opts.MapFrom(document => document != null ? document.LastName : default))
        .ForMember(userCreate => userCreate.Role, opts => opts.MapFrom(document => document != null ? document.Role : default))
        .ForMember(userCreate => userCreate.Licenses, opts => opts.MapFrom(document => document != null ? document.Licenses : default))
        .ForMember(userCreate => userCreate.Availability, opts => opts.MapFrom(document => document != null ? document.Availability : default))
        .ReverseMap();

      CreateMap<UserAccountDto, CreateDocumentDto<UserDocument>>()
        .ForMember(createdDocumentDto => createdDocumentDto.Document, opts => opts.MapFrom(user => user))
        .ForMember(createdDocumentDto => createdDocumentDto.IsOperationSuccessful, opts => opts.MapFrom(userRole => userRole != null ? userRole.IsOperationSuccessful : default))
        .ForMember(createdDocumentDto => createdDocumentDto.OperationException, opts => opts.MapFrom(userRole => userRole != null ? userRole.OperationException : default))
        .ForMember(createdDocumentDto => createdDocumentDto.OperationMessage, opts => opts.MapFrom(userRole => userRole != null ? userRole.OperationMessage : default))
        .ReverseMap();

      CreateMap<UserDocument, UserAccountRoleResponseDto>()
        .ForMember(userRole => userRole.Role, opts => opts.MapFrom(document => document != null ? document.Role : default))
        .ForMember(userRole => userRole.UserId, opts => opts.MapFrom(document => document != null ? document.id : default))
        .ReverseMap();

      CreateMap<UserDocument, UserAccountStateDto>()
        .ForMember(userState => userState.State, opts => opts.MapFrom(document => document != null ? document.State : default))
        .ReverseMap();

      CreateMap<UserAccountStateDto, DocumentCollectionDto<UserDocument>>()
        .ForMember(documentCollectionDto => documentCollectionDto.IsOperationSuccessful, opts => opts.MapFrom(userState => userState != null ? userState.IsOperationSuccessful : default))
        .ForMember(documentCollectionDto => documentCollectionDto.OperationException, opts => opts.MapFrom(userState => userState != null ? userState.OperationException : default))
        .ForMember(documentCollectionDto => documentCollectionDto.OperationMessage, opts => opts.MapFrom(userState => userState != null ? userState.OperationMessage : default))
        .ReverseMap();

      CreateMap<UserAccountStateDto, ConnectorDto<UserAccountStateDto>>()
        .ForMember(connectorDto => connectorDto.Result, opts => opts.MapFrom(userState => userState))
        .ForMember(connectorDto => connectorDto.IsOperationSuccessful, opts => opts.MapFrom(userState => userState != null ? userState.IsOperationSuccessful : default))
        .ForMember(connectorDto => connectorDto.OperationException, opts => opts.MapFrom(userState => userState != null ? userState.OperationException : default))
        .ForMember(connectorDto => connectorDto.OperationMessage, opts => opts.MapFrom(userState => userState != null ? userState.OperationMessage : default))
        .ReverseMap();

      CreateMap<OperationStatus, UserAccountStateDto>()
        .ForMember(userState => userState.IsOperationSuccessful, opts => opts.MapFrom(connectorDto => connectorDto != null ? connectorDto.IsOperationSuccessful : default))
        .ForMember(userState => userState.OperationException, opts => opts.MapFrom(connectorDto => connectorDto != null ? connectorDto.OperationException : default))
        .ForMember(userState => userState.OperationMessage, opts => opts.MapFrom(connectorDto => connectorDto != null ? connectorDto.OperationMessage : default))
        .ReverseMap();

      CreateMap<UserAccountStateDto, UserAccountStateDto>();

      CreateMap<OperationStatus, DocumentCollectionDto<UserDocument>>()
        .ForMember(documentCollectionDto => documentCollectionDto.IsOperationSuccessful, opts => opts.MapFrom(operationStatus => operationStatus != null ? operationStatus.IsOperationSuccessful : default))
        .ForMember(documentCollectionDto => documentCollectionDto.OperationException, opts => opts.MapFrom(operationStatus => operationStatus != null ? operationStatus.OperationException : default))
        .ForMember(documentCollectionDto => documentCollectionDto.OperationMessage, opts => opts.MapFrom(operationStatus => operationStatus != null ? operationStatus.OperationMessage : default))
        .ReverseMap();

      #endregion

      #region UserProfileConfiguration
      CreateMap<UserProfileDto, UserDocument>()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto != null ? dto.ProfilePictureUrl : default))
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto != null ? dto.FirstName : default))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto != null ? dto.LastName : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserDocument, UserProfileDto>()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto != null ? dto.ProfilePictureUrl : default))
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto != null ? dto.FirstName : default))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto != null ? dto.LastName : default))
        .ForMember(document => document.Address, opts => opts.MapFrom(dto => dto != null && dto != null && dto.SavedAddresses != null ? dto.SavedAddresses.FirstOrDefault(item => item.IsCurrentAddress) : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserProfileUpdateRequestDto, UserDocument>()
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto != null ? dto.FirstName : default))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto != null ? dto.LastName : default))
        .ForMember(document => document.Availability, opts => opts.MapFrom(dto => dto != null ? dto.Availability : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserProfileInformationDto, UserDocument>()
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto != null ? dto.FirstName : default))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto != null ? dto.LastName : default))
        .ForMember(document => document.Availability, opts => opts.MapFrom(dto => dto != null ? dto.Availability : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserDocument, UserProfileInformationDto>()
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto != null ? dto.FirstName : default))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto != null ? dto.LastName : default))
        .ForMember(document => document.Availability, opts => opts.MapFrom(dto => dto != null ? dto.Availability : default))
        .ForMember(document => document.Address, opts => opts.MapFrom(dto => dto != null && dto != null && dto.SavedAddresses != null ? dto.SavedAddresses.FirstOrDefault(item => item.IsCurrentAddress) : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserProfilePictureUpdateRequestDto, UserDocument>()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto != null ? dto.ProfilePictureUrl : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserProfilePictureDto, UserDocument>()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto != null ? dto.ProfilePictureUrl : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserDocument, UserProfilePictureDto > ()
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto != null ? dto.ProfilePictureUrl : default))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserSummaryDto, UserDocument>()
        .ForMember(document => document.SavedAddresses, opts => opts.MapFrom(dto => dto != null && dto.SavedAddresses != null ? dto.SavedAddresses : default))
        .ForMember(document => document.Licenses, opts => opts.MapFrom(dto => dto != null && dto.Licenses != null ? dto.Licenses : default))
        .ForMember(document => document.EntityId, opts => opts.MapFrom(dto => dto != null ? dto.Id : default))
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto != null ? dto.FirstName : default))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto != null ? dto.LastName : default))
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto != null ? dto.ProfilePictureUrl : default))
        .ForMember(document => document.Role, opts => opts.MapFrom(dto => dto != null ? dto.Role : default))
        .ForMember(document => document.Status, opts => opts.MapFrom(dto => dto != null ? dto.Status : default))
        .ForMember(document => document.UserPrincipalName, opts => opts.MapFrom(dto => dto != null ? dto.UserPrincipalName : null))
        .ForAllOtherMembers(item => item.Ignore());

      CreateMap<UserDocument, UserSummaryDto>()
        .ForMember(document => document.SavedAddresses, opts => opts.MapFrom(dto => dto != null && dto.SavedAddresses != null ? dto.SavedAddresses : default))
        .ForMember(document => document.Licenses, opts => opts.MapFrom(dto => dto != null && dto.Licenses != null ? dto.Licenses : default))
        .ForMember(document => document.Id, opts => opts.MapFrom(dto => dto != null ? Guid.Parse(dto.id) : default))
        .ForMember(document => document.FirstName, opts => opts.MapFrom(dto => dto != null ? dto.FirstName : default))
        .ForMember(document => document.LastName, opts => opts.MapFrom(dto => dto != null ? dto.LastName : default))
        .ForMember(document => document.ProfilePictureUrl, opts => opts.MapFrom(dto => dto != null ? dto.ProfilePictureUrl : default))
        .ForMember(document => document.Role, opts => opts.MapFrom(dto => dto != null ? dto.Role : default))
        .ForMember(document => document.Status, opts => opts.MapFrom(dto => dto != null ? dto.Status : default))
        .ForMember(document => document.UserPrincipalName, opts => opts.MapFrom(dto => dto != null ? dto.UserPrincipalName : null))
        .ForAllOtherMembers(item => item.Ignore());
      #endregion

      #region UserRatingConfiguration
      CreateMap<RatingsDto, RatingsDocument>()
        .ForMember(document => document.Ratings, opts => opts.MapFrom(dto => dto != null ? dto.Ratings.ToList() : default))
        .ForMember(document => document.id, opts => opts.Ignore())
        .ForMember(document => document.RatingsOfUser, opts => opts.Ignore())
        .ForMember(document => document.CreatedTimestampUtc, opts => opts.Ignore())
        .ForMember(document => document.UpdatedTimestampUtc, opts => opts.Ignore())
        .ForMember(document => document.AverageRating, opts => opts.Ignore())
        .ForMember(document => document.EntityId, opts => opts.Ignore())
        .ReverseMap();

      CreateMap<UserRatingsCreateOrUpdateRequestDto, RatingDto>()
        .ForMember(ratingDto => ratingDto.Score, opts => opts.MapFrom(requestDto => requestDto != null ? requestDto.Score : default))
        .ForMember(ratingDto => ratingDto.Comment, opts => opts.MapFrom(requestDto => requestDto != null ? requestDto.Comment : default))
        .ForMember(ratingDto => ratingDto.ReviewedByUser, opts => opts.MapFrom(requestDto => requestDto != null ? requestDto.ReviewedByUser : default))
        .ForMember(ratingDto => ratingDto.ReviewedUser, opts => opts.MapFrom(requestDto => requestDto != null ? requestDto.ReviewedUser : default))
        .ForMember(ratingDto => ratingDto.Type, opts => opts.MapFrom(requestDto => requestDto != null ? requestDto.Type : default))
        .ForMember(ratingDto => ratingDto.Id, opts => opts.Ignore())
        .ForMember(ratingDto => ratingDto.CreatedTimestampUtc, opts => opts.Ignore())
        .ForMember(ratingDto => ratingDto.UpdatedTimestampUtc, opts => opts.Ignore())
        .ReverseMap();
      #endregion

      #region UserAddressesConfiguration 

      CreateMap<UserAddressUpsertRequestDto, UserAddressDto>()
        .ForMember(userAddressDto => userAddressDto.Id, opts => opts.Ignore())
        .ReverseMap();

      #endregion

      #region UserLicensesConfiguration
      CreateMap<UserLicenseUpsertRequestDto, UserLicenseDto>()
        .ForMember(userLicenseDto => userLicenseDto.Id, opts => opts.Ignore())
        .ReverseMap();
      #endregion
    }
  }
}
