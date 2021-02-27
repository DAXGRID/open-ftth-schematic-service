using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.Schematic.API.Queries;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.QueryHandler
{
    public class GetDiagramQueryHandler : IQueryHandler<GetDiagram, Result<GetDiagramResult>>
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public GetDiagramQueryHandler(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public Task<Result<GetDiagramResult>> HandleAsync(GetDiagram query)
        {
            // TODO change to use logic that can generate a complete diagram for a giving route element

            return null;
        }
    }
}
