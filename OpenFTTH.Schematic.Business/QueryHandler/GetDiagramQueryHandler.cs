using FluentResults;
using Microsoft.Extensions.Logging;
using OpenFTTH.CQRS;
using OpenFTTH.Schematic.API.Queries;
using OpenFTTH.Schematic.Business.SchematicBuilder;
using System.Linq;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.QueryHandler
{
    public class GetDiagramQueryHandler : IQueryHandler<GetDiagram, Result<GetDiagramResult>>
    {
        private readonly ILogger<GetDiagramQueryHandler> _logger;
        private readonly IQueryDispatcher _queryDispatcher;

        public GetDiagramQueryHandler(ILogger<GetDiagramQueryHandler> logger, IQueryDispatcher queryDispatcher)
        {
            _logger = logger;
            _queryDispatcher = queryDispatcher;
        }

        public Task<Result<GetDiagramResult>> HandleAsync(GetDiagram query)
        {
            var builder = new RouteNetworkElementDiagramBuilder(_logger, _queryDispatcher);

            var diagramBuildingResult = builder.GetDiagram(query.RouteNetworkElementId).Result;

            if (diagramBuildingResult.IsFailed)
            {
                return Task.FromResult(
                       Result.Fail<GetDiagramResult>(new GetDiagramError(GetDiagramErrorCodes.DIAGRAM_BUILDING_FAILED, $"Error building diagram for route network element with id: {query.RouteNetworkElementId}")).
                       WithError(diagramBuildingResult.Errors.First())
                   );
            }

            return Task.FromResult(
                Result.Ok<GetDiagramResult>(
                    new GetDiagramResult(diagramBuildingResult.Value)
                )
            );
        }
    }
}
