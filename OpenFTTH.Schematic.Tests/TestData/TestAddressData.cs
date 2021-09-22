using NetTopologySuite.Geometries;
using OpenFTTH.Address.API.Model;
using System;
using System.Collections.Generic;

namespace OpenFTTH.TestData
{
    public static class TestAddressData
    {
        public static List<IAddress> AccessAddresses
        {
            get
            {
                List<IAddress> result = new();

                // Engum Møllevej 3, Vejle Ø
                result.Add(
                    new AccessAddress(
                        id: Guid.Parse("02a0b95e-b7f1-4888-bd10-074ef49f196c"),
                        addressPoint: new Point(541305.42, 6177593.58),
                        unitAddressIds: new Guid[] { Guid.Parse("d81c1428-1fe2-44bf-be71-57a5cfe8ac6c") }
                    )
                    {
                        HouseHumber = "3",
                        PostDistrictCode = "7120",
                        PostDistrict = "Vejle Øst",
                        ExternalId = Guid.Parse("0a3f5090-b718-32b8-e044-0003ba298018"),
                        MunicipalCode = "0630",
                        RoadCode = "0547",
                        RoadName = "Engum Møllevej"
                    }
                );

                result.Add(
                    new UnitAddress(Guid.Parse("d81c1428-1fe2-44bf-be71-57a5cfe8ac6c"), Guid.Parse("02a0b95e-b7f1-4888-bd10-074ef49f196c"))
                    {
                        ExternalId = Guid.Parse("0a3f50bc-aa89-32b8-e044-0003ba298018")
                    }
                );


                // Vesterbrogade 7A, Hedensted
                result.Add(
                    new AccessAddress(
                        id: Guid.Parse("3ddbdf9f-c7bf-448b-962f-d2e3f1d1511a"),
                        addressPoint: new Point(543767.8757586802, 6180577.642967158),
                        unitAddressIds: new Guid[] { Guid.Parse("5d639c7c-64e7-42c7-828e-5f615a13424b") }
                    )
                    {
                        HouseHumber = "7A",
                        PostDistrictCode = "8722",
                        PostDistrict = "Hedensted",
                        ExternalId = Guid.Parse("0a3f508f-8504-32b8-e044-0003ba298018"),
                        MunicipalCode = "0766",
                        RoadCode = "1626",
                        RoadName = "Vesterbrogade"
                    }
                );

                result.Add(
                    new UnitAddress(Guid.Parse("5d639c7c-64e7-42c7-828e-5f615a13424b"), Guid.Parse("3ddbdf9f-c7bf-448b-962f-d2e3f1d1511a"))
                    {
                        ExternalId = Guid.Parse("0a3f50bb-2d46-32b8-e044-0003ba298018")
                    }
                );

                // Rådhusgade 3, Horsens
                result.Add(
                    new AccessAddress(
                        id: Guid.Parse("0c5a203e-b989-4b88-ab90-283c6e7aafc7"),
                        addressPoint: new Point(553089.64, 6190980.39),
                        unitAddressIds: new Guid[] { 
                            Guid.Parse("9fe3d78b-0f22-48a2-afb4-fc76a9120e92"),
                            Guid.Parse("ea4d5132-c605-4c5a-9fc6-81f7935a16d9"),
                            Guid.Parse("5e6293be-b8f6-4eb4-b871-ed08541f5951"),
                            Guid.Parse("f2817327-2856-4435-8951-271ea0d65c38")
                        }
                    )
                    {
                        HouseHumber = "3",
                        PostDistrictCode = "8700",
                        PostDistrict = "Horsens",
                        ExternalId = Guid.Parse("6d66b677-5eea-43f2-afd1-354217159a81"),
                        MunicipalCode = "0615",
                        RoadCode = "0547",
                        RoadName = "Rådhusgade"
                    }
                );

                // Basement or something like that
                result.Add(
                    new UnitAddress(Guid.Parse("9fe3d78b-0f22-48a2-afb4-fc76a9120e92"), Guid.Parse("0c5a203e-b989-4b88-ab90-283c6e7aafc7"))
                    {
                        ExternalId = Guid.Parse("28a29822-3714-4285-ac71-f200027d5dda")
                    }
                );
                
                // St (living floor)
                result.Add(
                    new UnitAddress(Guid.Parse("ea4d5132-c605-4c5a-9fc6-81f7935a16d9"), Guid.Parse("0c5a203e-b989-4b88-ab90-283c6e7aafc7"))
                    {
                        ExternalId = Guid.Parse("3bc4989e-c838-4b42-bf43-cbb027587074"),
                        FloorName = "st"
                    }
                );

                // 1 floor
                result.Add(
                    new UnitAddress(Guid.Parse("5e6293be-b8f6-4eb4-b871-ed08541f5951"), Guid.Parse("0c5a203e-b989-4b88-ab90-283c6e7aafc7"))
                    {
                        ExternalId = Guid.Parse("5d992557-7a28-4dcd-a0dd-fcddccad2c41"),
                        FloorName = "1"
                    }
                );

                // 2 floor
                result.Add(
                   new UnitAddress(Guid.Parse("f2817327-2856-4435-8951-271ea0d65c38"), Guid.Parse("0c5a203e-b989-4b88-ab90-283c6e7aafc7"))
                   {
                       ExternalId = Guid.Parse("d1a8f126-c731-4a78-85ce-fcf631fdad88"),
                       FloorName = "2"
                   }
                );

                return result;
            }
        }
    }
}
