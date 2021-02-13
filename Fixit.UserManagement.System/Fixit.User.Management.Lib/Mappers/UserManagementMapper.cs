using AutoMapper;
using Fixit.Core.DataContracts.Users.Operations.Profile;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.User.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Account;
using Fixit.Core.DataContracts.Users.Operations.Account;
using Fixit.Core.DataContracts;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Connectors.DataContracts;

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
        .ForMember(userCreate => userCreate.FirstName, opts => opts.MapFrom(document => document != null ? document.FirstName : default))
        .ForMember(userCreate => userCreate.LastName, opts => opts.MapFrom(document => document != null ? document.LastName : default))
        .ForMember(userCreate => userCreate.LastName, opts => opts.MapFrom(document => document != null ? document.LastName : default))
        .ForMember(userCreate => userCreate.Role, opts => opts.MapFrom(document => document != null ? document.Role : default))
        .ReverseMap();

      CreateMap<UserAccountCreateRequestDto, CreateDocumentDto<UserDocument>>()
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
