using NetTopologySuite.Geometries;
using System;

namespace OpenFTTH.Schematic.API.Model.DiagramLayout
{
    public class DiagramObject : IDiagramObject
    {
        // Diagram object uuid
        private readonly Guid _mRID;
        public Guid MRID => _mRID;

        // Diagram reference
        private readonly Diagram _diagram;
        public Diagram Diagram => _diagram;

        public int DrawingOrder { get; set; }

        // Geometry
        public Geometry Geometry { get; init; }

        // Optional stuff
        public IdentifiedObjectReference IdentifiedObject { get; set; }
        public string Style { get; set; }
        public string Label { get; set; }
        public double Rotation { get; set; }

        public DiagramObject(Diagram diagram)
        {
            _mRID = Guid.NewGuid();
            _diagram = diagram;
            _diagram.AddDiagramObject(this);
        }

        public void SetReference(Guid refId, string refClass)
        {
            IdentifiedObject = new IdentifiedObjectReference() { RefId = refId, RefClass = refClass };
        }
    }
}
