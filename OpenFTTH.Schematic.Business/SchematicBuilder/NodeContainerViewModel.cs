using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;

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
    }
 }
