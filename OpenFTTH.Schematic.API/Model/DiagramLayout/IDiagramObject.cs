using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFTTH.Schematic.API.Model.DiagramLayout
{
    public interface IDiagramObject
    {
        Guid MRID { get; }

        Geometry Geometry { get;  }
    }
}
