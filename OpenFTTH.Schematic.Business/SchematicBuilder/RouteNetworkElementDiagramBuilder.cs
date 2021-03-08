using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class RouteNetworkElementDiagramBuilder
    {
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly Diagram _diagram = new Diagram()
        {
            Margin = 0.01
        };

        private readonly double _extraSpaceBetweenNodeContainerAndDetachedSpanSections = 80;
        private readonly double _spaceBetweenSections = 20;

        private Guid _routeNetworkElementId;
        private RouteNetworkElementRelatedData _data;

        public RouteNetworkElementDiagramBuilder(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public Task<Result<Diagram>> GetDiagram(Guid routeNetworkElementId)
        {
            _routeNetworkElementId = routeNetworkElementId;

            var fetchNeedeDataResult = RouteNetworkElementRelatedData.FetchData(_queryDispatcher, routeNetworkElementId);

            if (fetchNeedeDataResult.IsFailed)
                return Task.FromResult(Result.Fail<Diagram>(fetchNeedeDataResult.Errors.First()));
            else
                _data = fetchNeedeDataResult.Value;

            // If no equipment found, just return an empty diagram
            if (_data.SpanEquipments.Count == 0 && _data.NodeContainer == null)
            {
                return Task.FromResult(Result.Ok<Diagram>(new Diagram()));
            }

            double yOffset = 0;

            yOffset = AddDetachedSpanEquipmentsToDiagram(yOffset);
            yOffset = AddNodeContainerToDiagram(yOffset);


            return Task.FromResult((Result.Ok<Diagram>(_diagram)));
        }

        private double AddNodeContainerToDiagram(double yOffsetInitial)
        {
            double yOffset = yOffsetInitial + _extraSpaceBetweenNodeContainerAndDetachedSpanSections;

            if (_data.NodeContainer != null)
            {
                var readModel = new NodeContainerViewModel(_data);

                var builder = new NodeContainerBuilder(readModel);

                var size = builder.CreateDiagramObjects(_diagram, 0, yOffset);

                yOffset += size.Height + _extraSpaceBetweenNodeContainerAndDetachedSpanSections;
            }

            return yOffset;
        }

        private double AddDetachedSpanEquipmentsToDiagram(double yOffsetInitial)
        {
            double yOffset = yOffsetInitial;

            var detachedSpanEquipments = _data.SpanEquipments.Where(s => !s.IsAttachedToNodeContainer(_data));

            var orderedDetachedSpanEquipments = detachedSpanEquipments.OrderBy(s => s.IsPassThrough(_data)).Reverse();

            foreach (var spanEquipment in orderedDetachedSpanEquipments)
            {
                var readModel = new SpanEquipmentViewModel(_routeNetworkElementId, spanEquipment.Id, _data);

                var builder = new DetachedSpanEquipmentBuilder(readModel);

                var size = builder.CreateDiagramObjects(_diagram, 0, yOffset);

                yOffset += size.Height + _spaceBetweenSections;
            }

            return yOffset;
        }

       
    }
}
