using OpenFTTH.Schematic.API.Model.DiagramLayout;

namespace OpenFTTH.Schematic.API.Queries
{
    public record GetDiagramResult 
    {
        public Diagram Diagram { get; set; }
    }
}
