using Newtonsoft.Json.Linq;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using System.IO;
using System.Text;

namespace OpenFTTH.Schematic.Business.IO
{
    public class GeoJsonExporter
    {
        Diagram _diagram;

        int _rowId = 1;

        public GeoJsonExporter(Diagram diagram)
        {
            _diagram = diagram;
        }

        public void Export(string fileName)
        {
            var writer = new NetTopologySuite.IO.GeoJsonWriter();

            StreamWriter geoJsonFile = new StreamWriter(fileName, false, Encoding.UTF8);

            // Start geojson feature collection
            geoJsonFile.WriteLine("{ \"type\": \"FeatureCollection\", \"features\": [");

            bool first = true;

            foreach (var diagramObject in _diagram.DiagramObjects)
            {
                if (!first)
                    geoJsonFile.Write(",");

                var geometryJson = writer.Write(diagramObject.Geometry);
                var propertyJson = writer.Write(CreatePropertiesJsonObject(diagramObject));

                string line = "{ \"type\": \"Feature\", \"geometry\":" + geometryJson + ",\"properties\":" + propertyJson + "}";

                geoJsonFile.WriteLine(line);

                if (first)
                    first = false;
            }

            // End geojson feature collection
            geoJsonFile.WriteLine(" ] }");

            geoJsonFile.Close();
        }

        private JObject CreatePropertiesJsonObject(DiagramObject diagramObject)
        {
            JObject jsonProperties = new JObject();

            jsonProperties.Add(new JProperty("rowid", _rowId));
            _rowId++;

            jsonProperties.Add(new JProperty("Style", diagramObject.Style));

            jsonProperties.Add(new JProperty("Label", diagramObject.Label));

            if (diagramObject.IdentifiedObject != null)
            {
                jsonProperties.Add(new JProperty("RefId", diagramObject.IdentifiedObject.RefId));
                jsonProperties.Add(new JProperty("RefClass", diagramObject.IdentifiedObject.RefClass));
            }

            return jsonProperties;
        }
    }
}
