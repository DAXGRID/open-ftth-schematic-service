using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

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

                if (Data.NodeContainer.TerminalEquipmentReferences != null && Data.NodeContainer.TerminalEquipmentReferences.Length > 0)
                    return true;

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
                        RackId = rack.Id,
                        Name = rack.Name,
                        SpecName = rackSpec.ShortName,
                        MinHeightInUnits = rack.HeightInUnits,
                        TerminalEquipments = GetTerminalEquipmentViewModelsForRack(rack.Id)
                    });
                }
            }

            return rackViewModels;
        }

        public List<TerminalEquipmentViewModel> GetStandaloneTerminalEquipmentViewModels()
        {
            List<TerminalEquipmentViewModel> viewModels = new();

            if (Data.NodeContainer.TerminalEquipmentReferences != null)
            {
                foreach (var terminalEquipmentRef in Data.NodeContainer.TerminalEquipmentReferences)
                {
                    var terminalEquipment = Data.TerminalEquipments[terminalEquipmentRef];
                    var terminalEquipmentSpecification =Data.TerminalEquipmentSpecifications[terminalEquipment.SpecificationId];

                    viewModels.Add(new TerminalEquipmentViewModel()
                    {
                        TerminalEquipmentId = terminalEquipment.Id,
                        Name = terminalEquipment.Name,
                        SpecName = terminalEquipmentSpecification.ShortName
                    });
                }
            }

            return viewModels;
        }

        public List<TerminalEquipmentViewModel> GetTerminalEquipmentViewModelsForRack(Guid rackId)
        {
            List<TerminalEquipmentViewModel> viewModels = new();

            if (Data.NodeContainer.Racks != null)
            {
                var rack = Data.NodeContainer.Racks.First(r => r.Id == rackId);

                foreach (var subrack in rack.SubrackMounts)
                {
                    var terminalEquipment = Data.TerminalEquipments[subrack.TerminalEquipmentId];
                    var terminalEquipmentSpecification = Data.TerminalEquipmentSpecifications[terminalEquipment.SpecificationId];

                    viewModels.Add(new TerminalEquipmentViewModel()
                    {
                        TerminalEquipmentId = terminalEquipment.Id,
                        SubrackPosition = subrack.Position,
                        SubrackHeight = subrack.HeightInUnits,
                        Name = terminalEquipment.Name,
                        SpecName = terminalEquipmentSpecification.ShortName
                    });
                }
            }
          
            return viewModels;
        }
    }
 }
