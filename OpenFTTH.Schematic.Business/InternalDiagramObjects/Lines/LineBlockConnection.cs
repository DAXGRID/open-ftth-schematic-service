namespace OpenFTTH.Schematic.Business.Lines
{
    public class LineBlockConnection
    {
        public BlockPortTerminal FromTerminal { get; set; }
        public BlockPortTerminal ToTerminal { get; set; }

        public string Label { get; set; }
        public string Style { get; set; }
        public int DrawingOrder { get; set; }
    }
}
