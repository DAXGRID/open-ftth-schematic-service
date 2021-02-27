using FluentResults;

namespace OpenFTTH.Schematic.API.Queries
{
    public class GetDiagramError : Error
    {
        public GetDiagramErrorCodes Code { get; }
        public GetDiagramError(GetDiagramErrorCodes errorCode, string errorMsg) : base(errorCode.ToString() + ": " + errorMsg)
        {
            this.Code = errorCode;
            Metadata.Add("ErrorCode", errorCode.ToString());
        }
    }
}
