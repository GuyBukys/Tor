using FluentResults;
using MediatR;
using Tor.Application.Abstractions;
using Tor.Application.Businesses.Queries.GetAllBusinesses;
using Tor.Application.Clients.Queries.GetAppointments;
using Tor.Application.Common.Models;
using Tor.Domain.BusinessAggregate;
using Tor.Domain.BusinessAggregate.Entities;
using Tor.Domain.ClientAggregate;

namespace Tor.Application.Common.Behaviors;
internal class SignUrlsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase
{
    private readonly IStorageManager _storageManager;

    public SignUrlsBehavior(IStorageManager storageManager)
    {
        _storageManager = storageManager;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next();

        if (result.IsFailed)
        {
            return result;
        }

        // Business
        if (result is Result<Business> businessResult)
        {
            if (businessResult.Value.Logo is not null)
            {
                businessResult.Value.Logo = businessResult.Value.Logo with { Url = await _storageManager.SignUrl(businessResult.Value.Logo.Name, cancellationToken) };
            }

            if (businessResult.Value.Cover is not null)
            {
                businessResult.Value.Cover = businessResult.Value.Cover with { Url = await _storageManager.SignUrl(businessResult.Value.Cover.Name, cancellationToken) };
            }

            for (int i = 0; i < businessResult.Value.Portfolio.Count; i++)
            {
                businessResult.Value.Portfolio[i] = businessResult.Value.Portfolio[i] with
                {
                    Url = await _storageManager.SignUrl(businessResult.Value.Portfolio[i].Name, cancellationToken),
                };
            }

            foreach (var staffMember in businessResult.Value.StaffMembers)
            {
                if (staffMember.ProfileImage is not null)
                {
                    staffMember.ProfileImage = staffMember.ProfileImage with { Url = await _storageManager.SignUrl(staffMember.ProfileImage.Name, cancellationToken) };
                }
            }
        }

        if (result is Result<PagedList<BusinessSummary>> businessesResult)
        {
            foreach (var business in businessesResult.Value.Items)
            {
                if (business.Logo is not null)
                {
                    business.Logo.Url = await _storageManager.SignUrl(business.Logo.Name, cancellationToken);
                }

                if (business.Cover is not null)
                {
                    business.Cover.Url = await _storageManager.SignUrl(business.Cover.Name, cancellationToken);
                }
            }
        }

        // Staff member
        if (result is Result<StaffMember> staffMemberResult)
        {
            if (staffMemberResult.Value.ProfileImage is not null)
            {
                staffMemberResult.Value.ProfileImage = staffMemberResult.Value.ProfileImage with
                {
                    Url = await _storageManager.SignUrl(staffMemberResult.Value.ProfileImage.Name, cancellationToken)
                };
            }
        }

        if (result is Result<List<StaffMember>> staffMembersResult)
        {
            foreach (var staffMember in staffMembersResult.Value)
            {
                if (staffMember.ProfileImage is not null)
                {
                    staffMember.ProfileImage = staffMember.ProfileImage with
                    {
                        Url = await _storageManager.SignUrl(staffMember.ProfileImage.Name, cancellationToken)
                    };
                }
            }
        }

        // Client
        if (result is Result<Client> clientResult)
        {
            if (clientResult.Value.ProfileImage is not null)
            {
                clientResult.Value.ProfileImage = clientResult.Value.ProfileImage with
                {
                    Url = await _storageManager.SignUrl(clientResult.Value.ProfileImage.Name, cancellationToken)
                };
            }
        }

        if (result is Result<List<Client>> clientsResult)
        {
            foreach (var client in clientsResult.Value)
            {
                if (client.ProfileImage is not null)
                {
                    client.ProfileImage = client.ProfileImage with
                    {
                        Url = await _storageManager.SignUrl(client.ProfileImage.Name, cancellationToken)
                    };
                }
            }
        }

        if (result is Result<List<ClientAppointmentResult>> clientAppointmentsResult)
        {
            foreach (var clientAppointment in clientAppointmentsResult.Value)
            {
                if (clientAppointment.BusinessDetails.Logo is not null)
                {
                    clientAppointment.BusinessDetails.Logo.Url = await _storageManager.SignUrl(clientAppointment.BusinessDetails.Logo.Name, cancellationToken);
                }

                if (clientAppointment.BusinessDetails.Cover is not null)
                {
                    clientAppointment.BusinessDetails.Cover.Url = await _storageManager.SignUrl(clientAppointment.BusinessDetails.Cover.Name, cancellationToken);
                }
            }
        }

        return result;
    }
}
