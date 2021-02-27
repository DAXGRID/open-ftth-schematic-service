using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class RouteNetworkElementRelatedData
    {
        public LookupCollection<SpanEquipmentSpecification> SpanEquipmentSpecifications { get; set; }
        public LookupCollection<SpanStructureSpecification> SpanStructureSpecifications { get; set; }
        public LookupCollection<RouteNetworkElement> RouteNetworkElements { get; set; }
        public LookupCollection<SpanEquipmentWithRelatedInfo> SpanEquipments { get; set; }
        public Dictionary<Guid, RouteNetworkElementInterestRelation> InterestRelations { get; set; }
    }
}
