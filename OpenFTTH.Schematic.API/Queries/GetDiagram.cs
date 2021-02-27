using FluentResults;
using OpenFTTH.CQRS;
using System;

namespace OpenFTTH.Schematic.API.Queries
{
    public class GetDiagram : IQuery<Result<GetDiagramResult>>
    {
        public Guid RouteNetworkElementId { get; }
        public GetDiagram(Guid routeNetworkElementId)
        {
            RouteNetworkElementId = routeNetworkElementId;
        }
    }
}
