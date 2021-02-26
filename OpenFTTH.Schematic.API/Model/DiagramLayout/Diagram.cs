using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFTTH.Schematic.API.Model.DiagramLayout
{
    public class Diagram
    {
        List<DiagramObject> _diagramObjects = new List<DiagramObject>();

        public Envelope Envelope
        {
            get
            {
                Envelope envelope = new Envelope();

                foreach (var diagramObject in DiagramObjects)
                    envelope.ExpandToInclude(diagramObject.Geometry.EnvelopeInternal);

                return envelope;
            }
        }

        public List<DiagramObject> DiagramObjects 
        {
            get
            {
                return _diagramObjects;
            }
        }

        public void AddDiagramObject(DiagramObject diagramObject)
        {
            _diagramObjects.Add(diagramObject);
        }
    }
}
