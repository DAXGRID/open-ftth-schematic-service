using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenFTTH.Schematic.API.Model.DiagramLayout
{
    public class Diagram
    {
        List<DiagramObject> _diagramObjects = new List<DiagramObject>();

        public double Margin { get; init; }

        public Envelope Envelope
        {
            get
            {
                Envelope envelope = new Envelope();

                foreach (var diagramObject in DiagramObjects)
                    envelope.ExpandToInclude(diagramObject.Geometry.EnvelopeInternal);


                // Set marking on left and right side
                envelope.ExpandToInclude(0 - Margin, 0);
                envelope.ExpandToInclude(envelope.MaxX + Margin, 0);

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

        public void OrderDiagramObjects()
        {
            _diagramObjects = _diagramObjects.OrderBy(d => d.DrawingOrder).ToList();
        }
    }
}
