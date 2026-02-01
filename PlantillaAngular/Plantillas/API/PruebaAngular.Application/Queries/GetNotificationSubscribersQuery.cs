using MediatR;
using System.Collections.Generic;

namespace PruebaAngular.Application.Queries
{
    public class GetNotificationSubscribersQuery : IRequest<IReadOnlyList<string>>
    {
    }
}
