using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    /// <summary>
    /// View model serving diagram creation of a node container
    /// </summary>
    public class NodeContainerViewModel
    {
        private readonly Guid _elementNodeId;
        private readonly RouteNetworkElementRelatedData _data;

        public RouteNetworkElementRelatedData Data => _data;
        public NodeContainer NodeContainer { get; }
        public bool HasRacksOrTerminalEquipments
        {
            get {
                if (Data.NodeContainer.Racks != null && Data.NodeContainer.Racks.Length > 0)
                    return true;

                // TODO: Check if any equipments placed directly in node
                return false;
            }
        }

        public NodeContainerViewModel(RouteNetworkElementRelatedData data)
        {
            _data = data;
            
            if (_data.NodeContainer == null)
                throw new ApplicationException("This view model requires a RouteNetworkElementRelatedData object with a non-null NodeContainer object!");

            NodeContainer = _data.NodeContainer;
        }

        public string GetNodeContainerTypeLabel()
        {
            return _data.NodeContainerSpecifications[_data.NodeContainer.SpecificationId].Name;
        }

        public List<RackViewModel> GetRackViewModels()
        {
            List<RackViewModel> rackViewModels = new();

            if (Data.NodeContainer.Racks != null)
            {
                foreach (var rack in Data.NodeContainer.Racks)
                {
                    var rackSpec = Data.RackSpecifications[rack.SpecificationId];

                    rackViewModels.Add(new RackViewModel()
                    {
                        Id = rack.Id,
                        Name = rack.Name,
                        SpecName = rackSpec.ShortName,
                        MinHeightInUnits = rack.HeightInUnits
                    });
                }
            }

            return rackViewModels;
        }
    }
 }
