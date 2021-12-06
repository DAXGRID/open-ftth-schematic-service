using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.RouteNetwork.API.Queries;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork.Tracing;
using OpenFTTH.UtilityGraphService.API.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using RouteNetworkTrace = OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork.Tracing.RouteNetworkTrace;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class RouteNetworkElementRelatedData
    {
        public Guid RouteNetworkElementId { get; set; }
        public LookupCollection<SpanEquipmentSpecification> SpanEquipmentSpecifications { get; set; }
        public LookupCollection<SpanStructureSpecification> SpanStructureSpecifications { get; set; }
        public LookupCollection<NodeContainerSpecification> NodeContainerSpecifications { get; set; }
        public LookupCollection<TerminalEquipmentSpecification> TerminalEquipmentSpecifications { get; set; }
        public LookupCollection<RackSpecification> RackSpecifications { get; set; }
        public LookupCollection<RouteNetworkElement> RouteNetworkElements { get; set; }
        public LookupCollection<RouteNetworkInterest> RouteNetworkInterests { get; set; }
        public LookupCollection<SpanEquipmentWithRelatedInfo> SpanEquipments { get; set; }
        public LookupCollection<TerminalEquipment> TerminalEquipments { get; set; }
        public LookupCollection<RouteNetworkTrace> RouteNetworkTraces { get; set; }
        public Dictionary<Guid, RouteNetworkElementInterestRelation> InterestRelations { get; set; }
        public NodeContainer NodeContainer { get; set; }
        public Guid NodeContainerRouteNetworkElementId { get; set; }

        public Dictionary<Guid, List<Guid>> CableToConduitSegmentParentRelations = new();
        public Dictionary<Guid, List<Guid>> ConduitSegmentToCableChildRelations = new();

        public static Result<RouteNetworkElementRelatedData> FetchData(IQueryDispatcher queryDispatcher, Guid routeNetworkElementId)
        {
            RouteNetworkElementRelatedData result = new RouteNetworkElementRelatedData();

            result.RouteNetworkElementId = routeNetworkElementId;

            // TODO: All specifications should be cached and only re-queried if some spec is missing.

            // Query all span equipment specifications
            result.SpanEquipmentSpecifications = queryDispatcher.HandleAsync<GetSpanEquipmentSpecifications, Result<LookupCollection<SpanEquipmentSpecification>>>(new GetSpanEquipmentSpecifications()).Result.Value;

            // Query all span structure specifications
            result.SpanStructureSpecifications = queryDispatcher.HandleAsync<GetSpanStructureSpecifications, Result<LookupCollection<SpanStructureSpecification>>>(new GetSpanStructureSpecifications()).Result.Value;

            // Query all node container specifications
            result.NodeContainerSpecifications = queryDispatcher.HandleAsync<GetNodeContainerSpecifications, Result<LookupCollection<NodeContainerSpecification>>>(new GetNodeContainerSpecifications()).Result.Value;

            // Query all rack specifications
            result.RackSpecifications = queryDispatcher.HandleAsync<GetRackSpecifications, Result<LookupCollection<RackSpecification>>>(new GetRackSpecifications()).Result.Value;

            // Query all terminal equipment specifications
            result.TerminalEquipmentSpecifications = queryDispatcher.HandleAsync<GetTerminalEquipmentSpecifications, Result<LookupCollection<TerminalEquipmentSpecification>>>(new GetTerminalEquipmentSpecifications()).Result.Value;


            // Query all route node interests
            var routeNetworkInterestQuery = new GetRouteNetworkDetails(new RouteNetworkElementIdList() { routeNetworkElementId })
            {
                RelatedInterestFilter = RelatedInterestFilterOptions.ReferencesFromRouteElementAndInterestObjects
            };

            Result<GetRouteNetworkDetailsResult> interestsQueryResult = queryDispatcher.HandleAsync<GetRouteNetworkDetails, Result<GetRouteNetworkDetailsResult>>(routeNetworkInterestQuery).Result;

            if (interestsQueryResult.IsFailed)
                return Result.Fail(interestsQueryResult.Errors.First());

            result.InterestRelations = interestsQueryResult.Value.RouteNetworkElements.First().InterestRelations.ToDictionary(r => r.RefId);

            result.RouteNetworkInterests = interestsQueryResult.Value.Interests;

            var interestIdList = new InterestIdList();
            interestIdList.AddRange(result.InterestRelations.Values.Select(r => r.RefId));

            // Only query for equipments if interests are returned from the route network query
            if (interestIdList.Count > 0)
            {
                // Query all the equipments related to the route network element
                var equipmentQueryResult = queryDispatcher.HandleAsync<GetEquipmentDetails, Result<GetEquipmentDetailsResult>>(
                    new GetEquipmentDetails(interestIdList)
                    {
                        EquipmentDetailsFilter = new EquipmentDetailsFilterOptions() { IncludeRouteNetworkTrace = true }
                    }
                ).Result;

                if (equipmentQueryResult.IsFailed)
                    return Result.Fail(equipmentQueryResult.Errors.First());

                result.SpanEquipments = equipmentQueryResult.Value.SpanEquipment;
                result.RouteNetworkTraces = equipmentQueryResult.Value.RouteNetworkTraces;

                if (equipmentQueryResult.Value.NodeContainers != null && equipmentQueryResult.Value.NodeContainers.Count > 0)
                {
                    result.NodeContainer = equipmentQueryResult.Value.NodeContainers.First();
                    result.NodeContainerRouteNetworkElementId = interestsQueryResult.Value.Interests[result.NodeContainer.InterestId].RouteNetworkElementRefs[0];
                }

                // Query all route network elements of all the equipments
                var routeNetworkElementsQuery = new GetRouteNetworkDetails(interestIdList);
                Result<GetRouteNetworkDetailsResult> routeElementsQueryResult = queryDispatcher.HandleAsync<GetRouteNetworkDetails, Result<GetRouteNetworkDetailsResult>>(routeNetworkElementsQuery).Result;

                result.RouteNetworkElements = routeElementsQueryResult.Value.RouteNetworkElements;
            }
            else
            {
                result.RouteNetworkElements = new LookupCollection<RouteNetworkElement>();
                result.SpanEquipments = new LookupCollection<SpanEquipmentWithRelatedInfo>();
            }

            // Query terminal equipments
            List<Guid> terminalEquipmentIds = new();

            if (result.NodeContainer != null)
            {
                if (result.NodeContainer.Racks != null)
                {
                    foreach (var rack in result.NodeContainer.Racks)
                    {
                        foreach (var mount in rack.SubrackMounts)
                            terminalEquipmentIds.Add(mount.TerminalEquipmentId);
                    }
                }

                if (result.NodeContainer.TerminalEquipmentReferences != null)
                {
                    foreach (var terminalEquipmentReference in result.NodeContainer.TerminalEquipmentReferences)
                    {
                        terminalEquipmentIds.Add(terminalEquipmentReference);
                    }
                }
            }

            if (terminalEquipmentIds.Count > 0)
            {
                var terminalEquipmentQueryResult = queryDispatcher.HandleAsync<GetEquipmentDetails, Result<GetEquipmentDetailsResult>>(
                    new GetEquipmentDetails(new EquipmentIdList(terminalEquipmentIds))
                ).Result;

                if (terminalEquipmentQueryResult.IsFailed)
                    return Result.Fail(terminalEquipmentQueryResult.Errors.First());

                result.TerminalEquipments = terminalEquipmentQueryResult.Value.TerminalEquipment;
            }

            BuildCableToConduitIndex(result);

            return Result.Ok(result);
        }

        private static void BuildCableToConduitIndex(RouteNetworkElementRelatedData data)
        {
            HashSet<Guid> accessibleConduitSpanSegmentIds = new();

            foreach (var conduit in data.SpanEquipments.Where(s => !s.IsCable))
            {
                foreach (var structure in conduit.SpanStructures)
                {
                    var model = new SpanEquipmentViewModel(null, data.RouteNetworkElementId, conduit.Id, data);
                    var spanStructureInfo = model.GetSpanDiagramInfoForStructure("", structure);

                    if (spanStructureInfo.IngoingSegmentId != Guid.Empty)
                    {
                        if (!accessibleConduitSpanSegmentIds.Contains(spanStructureInfo.IngoingSegmentId))
                            accessibleConduitSpanSegmentIds.Add(spanStructureInfo.IngoingSegmentId);
                    }

                    if (spanStructureInfo.OutgoingSegmentId != Guid.Empty)
                    {
                        if (!accessibleConduitSpanSegmentIds.Contains(spanStructureInfo.OutgoingSegmentId))
                            accessibleConduitSpanSegmentIds.Add(spanStructureInfo.OutgoingSegmentId);
                    }
                }

            }

            foreach (var cable in data.SpanEquipments.Where(s => s.IsCable))
            {
                if (cable.ParentAffixes != null && cable.ParentAffixes.Length > 0)
                {
                    foreach (var parrentAffix in cable.ParentAffixes)
                    {
                        if (accessibleConduitSpanSegmentIds.Contains(parrentAffix.SpanSegmentId))
                        {
                            if (data.CableToConduitSegmentParentRelations.ContainsKey(cable.Id))
                                data.CableToConduitSegmentParentRelations[cable.Id].Add(parrentAffix.SpanSegmentId);
                            else
                                data.CableToConduitSegmentParentRelations[cable.Id] = new List<Guid>() { parrentAffix.SpanSegmentId };


                            if (data.ConduitSegmentToCableChildRelations.ContainsKey(parrentAffix.SpanSegmentId))
                                data.ConduitSegmentToCableChildRelations[parrentAffix.SpanSegmentId].Add(cable.Id);
                            else
                                data.ConduitSegmentToCableChildRelations[parrentAffix.SpanSegmentId] = new List<Guid>() { cable.Id };
                        }
                    }
                }

            }
        }

        public string GetCableEquipmentLabel(Guid cableId)
        {
            var cable = SpanEquipments[cableId];

            string label = SpanEquipmentSpecifications[cable.SpecificationId].Name;

            return label;
        }

        public string GetCableEquipmentLineLabel(Guid cableId)
        {
            var cable = SpanEquipments[cableId];

            string label = SpanEquipmentSpecifications[cable.SpecificationId].Name;

            return cable.Name + " (" + (cable.SpanStructures.Length - 1).ToString() + ")";
        }

    }
}
