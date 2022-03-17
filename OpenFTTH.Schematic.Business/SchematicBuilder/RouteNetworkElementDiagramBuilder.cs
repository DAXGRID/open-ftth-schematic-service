using FluentResults;
using Microsoft.Extensions.Logging;
using OpenFTTH.CQRS;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.QueryHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class RouteNetworkElementDiagramBuilder
    {
        private readonly ILogger<GetDiagramQueryHandler> _logger;
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly Diagram _diagram = new Diagram()
        {
            Margin = 0.01
        };

        private readonly double _extraSpaceBetweenNodeContainerAndDetachedSpanSections = 80;
        private readonly double _spaceBetweenSections = 20;

        private Guid _routeNetworkElementId;
        private RouteNetworkElementRelatedData _data;

        public RouteNetworkElementDiagramBuilder(ILogger<GetDiagramQueryHandler> logger, IQueryDispatcher queryDispatcher)
        {
            _logger = logger;
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

            // order by drawing order
            _diagram.OrderDiagramObjects();


            return Task.FromResult((Result.Ok<Diagram>(_diagram)));
        }

        private double AddNodeContainerToDiagram(double yOffsetInitial)
        {
            double yOffset = yOffsetInitial + _extraSpaceBetweenNodeContainerAndDetachedSpanSections;

            if (_data.NodeContainer != null)
            {
                var readModel = new NodeContainerViewModel(_data);

                var builder = new NodeContainerBuilder(_logger, readModel);

                var size = builder.CreateDiagramObjects(_diagram, 0, yOffset);

                yOffset += size.Height + _extraSpaceBetweenNodeContainerAndDetachedSpanSections;
            }

            return yOffset;
        }

        private double AddDetachedSpanEquipmentsToDiagram(double yOffsetInitial)
        {
            var detachedSpanEquipments = _data.SpanEquipments.Where(s => !s.IsAttachedToNodeContainer(_data));

            //var orderedDetachedSpanEquipments = detachedSpanEquipments.OrderBy(s => s.IsCable).ThenBy(s => s.IsPassThrough(_data)).ThenBy(s => s.IsMultiLevel(_data)).Reverse();

            List<SpanEquipmentViewModel> unorderedReadModels = new();

            foreach (var spanEquipment in detachedSpanEquipments)
            {
                var readModel = new SpanEquipmentViewModel(_logger, _routeNetworkElementId, spanEquipment.Id, _data);

                if (!readModel.IsCableWithinConduit)
                {
                    unorderedReadModels.Add(readModel);
                }
            }

            double yOffset = yOffsetInitial;

            var orderedReadModels = unorderedReadModels.
                OrderBy(s => s.SpanEquipment.IsPassThrough(_data)).
                ThenBy(s => s.SpanEquipment.IsCable).
                ThenBy(s => s.SpanEquipment.IsMultiLevel(_data)).
                ThenBy(s => s.GetOutgoingLabel(s.SpanEquipment.SpanStructures[0].SpanSegments[0].Id))
                .Reverse();


            foreach (var readModel in orderedReadModels)
            {
                var builder = new DetachedSpanEquipmentBuilder(_logger, readModel);

                var size = builder.CreateDiagramObjects(_diagram, 0, yOffset);

                yOffset += size.Height + _spaceBetweenSections;
            }


            return yOffset;
        }

       
    }
}
