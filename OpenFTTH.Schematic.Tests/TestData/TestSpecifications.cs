using CSharpFunctionalExtensions;
using OpenFTTH.CQRS;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Commands;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Queries;
using System;
using System.Threading;

namespace OpenFTTH.TestData
{
    public class TestSpecifications
    {
        private static bool _specificationsCreated = false;
        private static readonly object _myLock = new object();

        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;

        public TestSpecifications(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public static Guid Manu_GMPlast = Guid.Parse("47e87d16-a1f0-488a-8c3e-cb3a4f3e8926");
        public static Guid Manu_Emtelle = Guid.Parse("fd457db0-ad32-444c-9946-a9e5e8a14d17");
        public static Guid Manu_Fiberpowertech = Guid.Parse("e845dc91-f3b9-407b-a622-2c300d43aaad");
        public static Guid Manu_Cubis = Guid.Parse("6b02e4aa-19f1-46a5-85e8-c1faab236ef0");

        public static Guid Well_Cubis_STAKKAbox_MODULA_600x450 = Guid.Parse("0fb389b5-4bbd-4ebf-b506-bfc636001171");
        public static Guid Well_Cubis_STAKKAbox_MODULA_900x450 = Guid.Parse("8251e1d3-c586-4632-952a-41332aa61a47");

        public static Guid Well_Fiberpowertech_37_EK_378_400x800 = Guid.Parse("7fd8266e-44e1-46ee-a183-bc3068deadf3");
        public static Guid Well_Fiberpowertech_37_EK_338_550x1165 = Guid.Parse("b93c3bcf-3013-4b6c-814d-06ff14d9139f");
        public static Guid Well_Fiberpowertech_37_EK_328_800x800 = Guid.Parse("6c1c9ab8-b1f2-4021-bece-d9b4f65c6723");

        // 50mm Straight In-line Elongated Enclosure (gammel model)
        public static Guid Conduit_Closure_Emtelle_Straight_In_line = Guid.Parse("a7bf7613-6ed3-4b38-a509-ea1c34e62660");

        // Branch box 50mm (ny model)
        public static Guid Conduit_Closure_Emtelle_Branch_Box = Guid.Parse("ded31f47-9161-4080-ae82-1251ae2fc8c0");

        // Kompesssionsmuffer
        public static Guid Conduit_Connector_Fiberpowertech_Straight_40 = Guid.Parse("7f2f1a7e-9e2d-45c4-958a-ce049a69a9a3");
        public static Guid Conduit_Connector_Fiberpowertech_Straight_50 = Guid.Parse("5eb03c1f-41f3-4cf2-81b0-13f91ad11432");

        // Span Structures
        public static Guid Ø7_Blue = Guid.Parse("94d5bc20-31da-4191-b4dc-9a701bba83d6");
        public static Guid Ø7_Yellow = Guid.Parse("6416641c-1c31-4e6f-9bc4-41a435915a69");
        public static Guid Ø7_White = Guid.Parse("6c861c63-0134-45ac-9e1e-c982b543e74e");
        public static Guid Ø7_Green = Guid.Parse("55af46cb-5c53-4046-a4d6-0069ad60bbf8");
        public static Guid Ø7_Black = Guid.Parse("b96a1ecb-4665-4929-a016-82ee48cae57e");
        public static Guid Ø7_Red = Guid.Parse("ef1c6787-eb76-4941-96e0-4ca2fac5b57b");
        public static Guid Ø7_Orange = Guid.Parse("def493b9-71a1-43c5-9f09-02e8229bc785");
        public static Guid Ø7_Pink = Guid.Parse("1bb0ee9b-3f41-44f0-8f5b-ccae8cb63a23");
        public static Guid Ø7_Silver = Guid.Parse("16c77f06-3439-4843-8ff9-76bb3a7c8b20");
        public static Guid Ø7_Brown = Guid.Parse("4e9373e8-c6bd-4a85-856d-6f2ef57f6833");
        public static Guid Ø7_Turquoise = Guid.Parse("72d88e01-178a-4656-9fe5-8819544c94c9");
        public static Guid Ø7_Violet = Guid.Parse("3672dd91-886a-444e-93a2-afc0d44d6541");

        public static Guid Ø10_Blue = Guid.Parse("980a2a21-cf40-4b70-91ae-69af79be9e80");
        public static Guid Ø10_Yellow = Guid.Parse("779a8d88-1c52-4fca-b2d5-0aabfa652393");
        public static Guid Ø10_White = Guid.Parse("ec75555f-33ea-432f-9235-d1f111cebd68");
        public static Guid Ø10_Green = Guid.Parse("c1734de6-1ca2-4b2f-af74-86c6ca45f6e5");
        public static Guid Ø10_Black = Guid.Parse("30f3f962-274c-43fc-951b-f3e2213f9ba7");
        public static Guid Ø10_Red = Guid.Parse("2ef21422-fc35-4c87-8051-b235568c1def");
        public static Guid Ø10_Orange = Guid.Parse("0aa07b3a-8168-4ab8-8ccf-62c163e5be28");
        public static Guid Ø10_Pink = Guid.Parse("f9d5f15e-d4f2-4d53-9fce-7808a579e853");
        public static Guid Ø10_Silver = Guid.Parse("7e21f619-039b-4a58-b43c-ee5496f0cbb2");
        public static Guid Ø10_Brown = Guid.Parse("b4cde3bf-56ff-43f3-a62a-270cf9afa24d");
        public static Guid Ø10_Turquoise = Guid.Parse("9d05b556-2eb4-4a30-89d6-4b813c10dabe");
        public static Guid Ø10_Violet = Guid.Parse("c09ca8f5-cd37-4cd8-9d32-b6274f3c2c64");

        public static Guid Ø12_Orange = Guid.Parse("5e0bef90-6838-49a7-9063-7d099a06e23d");
        public static Guid Ø12_Red = Guid.Parse("f90dfced-3244-4cde-9839-2ca8b49a4483");

        public static Guid Ø32_Orange = Guid.Parse("e7dea74e-df7f-4a0d-a752-98f9e02be1a8");
        public static Guid Ø40_Orange = Guid.Parse("ac417fea-b6f6-4a5a-9c9e-10ee05ecbf56");
        public static Guid Ø40_Red = Guid.Parse("be4deb0f-8d15-49ba-bbeb-fafb4ed66de5");
        public static Guid Ø50_Orange = Guid.Parse("7960355a-4dab-4d60-b3a5-e20ac4301176");
        public static Guid Ø110_Red = Guid.Parse("e078e830-f79d-4220-bd9a-87ed7cf81f1d");

        // Multi Conduit Span Equipment
        public static Guid Multi_Ø32_3x10 = Guid.Parse("b11a4fce-2116-4437-9108-3ca467124d99");

        public static Guid Multi_Ø40_5x10 = Guid.Parse("7ca9dcbb-524f-4d61-945c-16bf2679326e");
        public static Guid Multi_Ø40_5x10_Red = Guid.Parse("9262c4f2-06fc-4315-be71-12fd9c57aedd");
        public static Guid Multi_Ø40_5x10_BlackBlack = Guid.Parse("21294bdc-9daa-4ac1-b118-1dca603f1c3f");

        public static Guid Multi_Ø40_6x10 = Guid.Parse("f8d15ef6-b07f-440b-8357-4c7a3f84f156");
        public static Guid Multi_Ø40_12x7 = Guid.Parse("eac5660c-446b-4404-b0af-01032ebf94da");

        public static Guid Multi_Ø50_10x10 = Guid.Parse("1c2a1e9e-03e6-4eb9-ae89-e723fea1e59c");
        public static Guid Multi_Ø50_10x7_5x10 = Guid.Parse("5c3e36f7-1278-4613-9f67-fbcf93195fa7");
        public static Guid Multi_Ø50_12x7_5x10 = Guid.Parse("36f0deaf-0d77-4cae-be06-1e6e0cf84ae2");
        public static Guid Multi_Ø50_12x7_5x10_BlueYellow = Guid.Parse("dc83bdd3-142b-49ff-8a80-d8d7e1d794b3");
        public static Guid Multi_Ø50_12x7_5x10_GreenWhite = Guid.Parse("2fe1a566-6477-4f24-b7df-e242bd6c7d7d");

        public static Guid Flex_Ø40_Red = Guid.Parse("6df48525-ac10-4b02-b1cd-05283b549ab2");
        public static Guid Tomrør_Ø110_Red = Guid.Parse("233e77b1-b580-4093-91c1-48ac636cb300");
        public static Guid Tomrør_Ø40_Orange = Guid.Parse("9c933058-1ea6-475f-930a-b389c421023d");
        public static Guid Tomrør_Ø50_Orange = Guid.Parse("5c805317-8216-4b00-88c3-6ec9d542f831");

        // Single Conduit SpanEquipment
        public static Guid SingleConduit_Ø7_Blue = Guid.Parse("abaaf52d-2b74-4f45-b7a7-700969a9b13e");
        public static Guid SingleConduit_Ø7_Yellow = Guid.Parse("49182083-3f67-4991-aa0b-4a83f8c9faca");
        public static Guid SingleConduit_Ø7_White = Guid.Parse("c6eee17a-85e5-4665-9ca4-83bb57fef025");
        public static Guid SingleConduit_Ø7_Green = Guid.Parse("2ebad22f-ff39-42bb-8dad-b7cef8122caf");
        public static Guid SingleConduit_Ø7_Black = Guid.Parse("8597992f-8d7c-46cf-b962-901417899c05");
        public static Guid SingleConduit_Ø7_Red = Guid.Parse("6d4c9ce6-dc1b-4b63-92cb-c9b734acc2af");
        public static Guid SingleConduit_Ø7_Orange = Guid.Parse("0dd594fa-7b5a-4700-9032-580eaef9bc04");
        public static Guid SingleConduit_Ø7_Pink = Guid.Parse("144d1546-e52c-41c3-b51b-2ace90ba3494");
        public static Guid SingleConduit_Ø7_Silver = Guid.Parse("ac6b6f0a-6d38-4787-983f-9889490ad15f");
        public static Guid SingleConduit_Ø7_Brown = Guid.Parse("ef4c187d-db67-4f36-9767-8394a11c14e4");
        public static Guid SingleConduit_Ø7_Turquoise = Guid.Parse("e96f2c04-5a15-476b-aec4-a99527984c8b");
        public static Guid SingleConduit_Ø7_Violet = Guid.Parse("0c69a8cf-3eb1-41c1-876c-bd658b077eda");

        public static Guid SingleConduit_Ø10_Blue = Guid.Parse("22869ff0-e5e5-4269-b92a-b38502c80b04");
        public static Guid SingleConduit_Ø10_Yellow = Guid.Parse("aac8a871-f32c-420b-ab78-637762451c33");
        public static Guid SingleConduit_Ø10_White = Guid.Parse("ef14f165-9fd7-4413-a49b-f573e768427e");
        public static Guid SingleConduit_Ø10_Green = Guid.Parse("5f5b702a-06fd-4bfe-ab13-ce829f29477b");
        public static Guid SingleConduit_Ø10_Black = Guid.Parse("44875813-b7f7-4763-88f5-bb8c9f53ec54");
        public static Guid SingleConduit_Ø10_Red = Guid.Parse("6e254bfe-9e98-4f3c-9a23-c34a89066611");
        public static Guid SingleConduit_Ø10_Orange = Guid.Parse("16d65e91-427d-4875-8ebe-1aa407e290c2");
        public static Guid SingleConduit_Ø10_Pink = Guid.Parse("e81c89f5-9cf0-4970-9f81-37dbc3333086");
        public static Guid SingleConduit_Ø10_Silver = Guid.Parse("5a72e1ce-5ecf-42cd-ac56-ac1eec42bb24");
        public static Guid SingleConduit_Ø10_Brown = Guid.Parse("fae6deb1-cbc4-4848-9f98-cdddb5442a17");
        public static Guid SingleConduit_Ø10_Turquoise = Guid.Parse("65cc9205-3884-4f52-abac-2fcbadfc1674");
        public static Guid SingleConduit_Ø10_Violet = Guid.Parse("fbfa0de1-1e1d-4592-bb90-0edad2f9ee46");

        public static Guid SingleConduit_Ø12_Red = Guid.Parse("0b6be410-a6e3-4696-9964-3aff3e827dc8");
        public static Guid SingleConduit_Ø12_Orange = Guid.Parse("12c5e369-9fcc-49c7-81f5-208769501b7d");


        public FluentResults.Result<TestSpecifications> Run()
        {
            lock (_myLock)
            {
                var manufacturerQueryResult = _queryDispatcher.HandleAsync<GetManufacturer, Result<LookupCollection<Manufacturer>>>(new GetManufacturer()).Result;

                if (manufacturerQueryResult.Value.ContainsKey(Manu_GMPlast))
                    return FluentResults.Result.Fail("Test specification already present in system");

                AddManufactures();

                AddNodeContainerSpecifications();

                AddSpanStructureSpecifications();

                AddSpanEquipmentSpecifications();

                Thread.Sleep(100);

                _specificationsCreated = true;

                return FluentResults.Result.Ok(this);
            }
        }

        private void AddNodeContainerSpecifications()
        {
            // Man Holes
            AddSpecification(new NodeContainerSpecification(Well_Cubis_STAKKAbox_MODULA_600x450, "ManHole", "STAKKAbox 600x450")
            {
                Description = "STAKKAbox MODULA 600x450mm",
                ManufacturerRefs = new Guid[] { Manu_Cubis }
            });

            AddSpecification(new NodeContainerSpecification(Well_Cubis_STAKKAbox_MODULA_900x450, "ManHole", "STAKKAbox 900x450")
            {
                Description = "STAKKAbox MODULA 900x450mm",
                ManufacturerRefs = new Guid[] { Manu_Cubis }
            });

            AddSpecification(new NodeContainerSpecification(Well_Fiberpowertech_37_EK_378_400x800, "ManHole", "EK 378 400x800")
            {
                Description = "37-EK 378 400x800mm",
                ManufacturerRefs = new Guid[] { Manu_Fiberpowertech }
            });

            AddSpecification(new NodeContainerSpecification(Well_Fiberpowertech_37_EK_338_550x1165, "ManHole", "EK 338 550x1165")
            {
                Description = "37-EK 338 550x1165mm",
                ManufacturerRefs = new Guid[] { Manu_Fiberpowertech }
            });

            AddSpecification(new NodeContainerSpecification(Well_Fiberpowertech_37_EK_328_800x800, "ManHole", "EK 328 800x800")
            {
                Description = "37-EK 328 800x800mm",
                ManufacturerRefs = new Guid[] { Manu_Fiberpowertech }
            });

            // Conduit Closures
            AddSpecification(new NodeContainerSpecification(Conduit_Closure_Emtelle_Branch_Box, "ConduitClosure", "Branch box 50mm")
            {
                Description = "Branch box 50mm",
                ManufacturerRefs = new Guid[] { Manu_Emtelle }
            });

            AddSpecification(new NodeContainerSpecification(Conduit_Closure_Emtelle_Straight_In_line, "ConduitClosure", "Straight 50mm")
            {
                Description = "50mm Straight In-line Elongated Enclosure",
                ManufacturerRefs = new Guid[] { Manu_Emtelle }
            });

            // Conduit Compression Connectors
            AddSpecification(new NodeContainerSpecification(Conduit_Connector_Fiberpowertech_Straight_40, "CompressionConduitConnector", "Straight 40mm")
            {
                Description = "Straight 40mm (kompressionsmuffe)",
                ManufacturerRefs = new Guid[] { Manu_Fiberpowertech }
            });

            AddSpecification(new NodeContainerSpecification(Conduit_Connector_Fiberpowertech_Straight_50, "CompressionConduitConnector", "Straight 50mm")
            {
                Description = "Straight 50mm (kompressionsmuffe)",
                ManufacturerRefs = new Guid[] { Manu_Fiberpowertech }
            });

        }

        private void AddSpanEquipmentSpecifications()
        {
            AddSpecification(new SpanEquipmentSpecification(Multi_Ø32_3x10, "Conduit", "Ø32 3x10",
                new SpanStructureTemplate(Ø32_Orange, 1, 1,
                    new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>())
                    })
                )
            {
                Description = "ø32 mm Multirør 3x10",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });


            // Ø40

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø40_5x10, "Conduit", "Ø40 5x10",
                new SpanStructureTemplate(Ø40_Orange, 1, 1,
                    new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>())
                    })
                )
            {
                Description = "ø40 mm Multirør 5x10",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø40_5x10_Red, "Conduit", "Ø40 5x10",
               new SpanStructureTemplate(Ø40_Orange, 1, 1,
                   new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Red, 2, 5, Array.Empty<SpanStructureTemplate>())
                   })
               )
            {
                Description = "ø40 mm Multirør 5x10 (Blå, Gul, Hvid, Grøn, Rød)",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø40_5x10_BlackBlack, "Conduit", "Ø40 5x10",
               new SpanStructureTemplate(Ø40_Orange, 1, 1,
                   new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>())
                   })
               )
            {
                Description = "ø40 mm Multirør 5x10 (Blå, Hvid, Grøn, Sort, Sort)",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });



            AddSpecification(new SpanEquipmentSpecification(Multi_Ø40_6x10, "Conduit", "Ø40 6x10",
                new SpanStructureTemplate(Ø40_Orange, 1, 1,
                    new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Red, 2, 6, Array.Empty<SpanStructureTemplate>())
                    })
                )
            {
                Description = "ø40 mm Multirør 6x10",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            // Ø 40
            AddSpecification(new SpanEquipmentSpecification(Multi_Ø50_10x10, "Conduit", "Ø50 10x10",
                new SpanStructureTemplate(Ø50_Orange, 1, 1,
                    new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Red, 2, 6, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Orange, 2, 7, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Pink, 2, 8, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Silver, 2, 9, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Brown, 2, 10, Array.Empty<SpanStructureTemplate>())
                    })
                )
            {
                Description = "ø50 mm Multirør 10x10",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø40_12x7, "Conduit", "Ø40 12x7",
                new SpanStructureTemplate(Ø40_Orange, 1, 1,
                    new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø7_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Black, 2, 5, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Red, 2, 6, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Orange, 2, 7, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Pink, 2, 8, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Silver, 2, 9, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Brown, 2, 10, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Turquoise, 2, 11, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Violet, 2, 12, Array.Empty<SpanStructureTemplate>())
                    })
                )
            {
                Description = "ø40 mm Multirør 12x7 standard farver",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø50_10x7_5x10, "Conduit", "Ø50 5x10+10x7",
                new SpanStructureTemplate(Ø50_Orange, 1, 1,
                    new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>()),

                        new SpanStructureTemplate(Ø7_Blue, 2, 6, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 7, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 8, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 9, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Black, 2, 10, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Red, 2, 11, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Orange, 2, 12, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Pink, 2, 13, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Silver, 2, 14, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Brown, 2, 15, Array.Empty<SpanStructureTemplate>()),
                    })
                )
            {
                Description = "ø50 mm Multirør 5x10 + 10x7 standard farver",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø50_12x7_5x10, "Conduit", "Ø50 5x10+12x7",
              new SpanStructureTemplate(Ø50_Orange, 1, 1,
                  new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>()),

                        new SpanStructureTemplate(Ø7_Blue, 2, 6, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 7, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 8, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 9, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Black, 2, 10, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Red, 2, 11, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Orange, 2, 12, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Pink, 2, 13, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Silver, 2, 14, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Brown, 2, 15, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Turquoise, 2, 16, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Violet, 2, 17, Array.Empty<SpanStructureTemplate>())
                  })
              )
            {
                Description = "ø50 mm Multirør 5x10 + 10x7 standard farver",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø50_12x7_5x10_BlueYellow, "Conduit", "Ø50 5x10+12x7",
             new SpanStructureTemplate(Ø50_Orange, 1, 1,
                 new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>()),

                        new SpanStructureTemplate(Ø7_Blue, 2, 6, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Blue, 2, 7, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Blue, 2, 8, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Blue, 2, 9, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Blue, 2, 10, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Blue, 2, 11, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 12, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 13, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 14, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 15, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 16, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Yellow, 2, 17, Array.Empty<SpanStructureTemplate>())
                 })
             )
            {
                Description = "ø50 mm Multirør 5x10 + 12x7 Blue Yellow",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø50_12x7_5x10_GreenWhite, "Conduit", "Ø50 5x10+12x7",
             new SpanStructureTemplate(Ø50_Orange, 1, 1,
                 new SpanStructureTemplate[] {
                        new SpanStructureTemplate(Ø10_Blue, 2, 1, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Yellow, 2, 2, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_White, 2, 3, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Green, 2, 4, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø10_Black, 2, 5, Array.Empty<SpanStructureTemplate>()),

                        new SpanStructureTemplate(Ø7_Green, 2, 6, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 7, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 8, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 9, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 10, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_Green, 2, 11, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 12, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 13, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 14, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 15, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 16, Array.Empty<SpanStructureTemplate>()),
                        new SpanStructureTemplate(Ø7_White, 2, 17, Array.Empty<SpanStructureTemplate>())
                 })
             )
            {
                Description = "ø50 mm Multirør 5x10 + 12x7 Green White",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = true
            });


            // Tomrør
            AddSpecification(new SpanEquipmentSpecification(Flex_Ø40_Red, "Conduit", "Ø40 Flex", new SpanStructureTemplate(Ø40_Red, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø40 mm Flexrør",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = false,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Tomrør_Ø40_Orange, "Conduit", "Ø40", new SpanStructureTemplate(Ø40_Orange, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø40 mm orange tomrør",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = false,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Tomrør_Ø50_Orange, "Conduit", "Ø50", new SpanStructureTemplate(Ø50_Orange, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø50 mm orange tomrør",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = false,
                IsMultiLevel = true
            });

            AddSpecification(new SpanEquipmentSpecification(Tomrør_Ø110_Red, "Conduit", "Ø110", new SpanStructureTemplate(Ø110_Red, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø110 mm rød tomrør",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = false,
                IsMultiLevel = true
            });

            // Ø7 Enkelt rør
            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Blue, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Blue, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 blå",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Yellow, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Yellow, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 gul",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_White, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_White, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 hvid",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Green, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Green, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 grøn",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Black, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Black, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 sort",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Red, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Red, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 rød",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Orange, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Orange, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 orange",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Pink, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Pink, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 pink",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Silver, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Silver, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 grå",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Brown, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Brown, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 brun",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Turquoise, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Turquoise, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 turkis",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø7_Violet, "SingleConduit", "Ø7", new SpanStructureTemplate(Ø7_Violet, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø7/5 violet",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });


            // Ø10 single conduit
            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Blue, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Blue, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 blå",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Yellow, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Yellow, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 gul",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_White, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_White, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 hvid",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Green, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Green, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 grøn",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Black, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Black, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 sort",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Red, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Red, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 rød",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Orange, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Orange, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 orange",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Pink, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Pink, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 pink",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Silver, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Silver, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 grå",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Brown, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Brown, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 brun",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Turquoise, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Turquoise, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 turkis",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø10_Violet, "SingleConduit", "Ø10", new SpanStructureTemplate(Ø10_Violet, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø10/8 violet",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            // Ø12 single conduit
            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø12_Orange, "SingleConduit", "Ø12", new SpanStructureTemplate(Ø12_Orange, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø12/10 orange",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });

            // Ø12 single conduit
            AddSpecification(new SpanEquipmentSpecification(SingleConduit_Ø12_Red, "SingleConduit", "Ø12", new SpanStructureTemplate(Ø12_Red, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø12/10 rød",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle },
                IsFixed = true,
                IsMultiLevel = false
            });


        }

        private void AddSpanStructureSpecifications()
        {
            AddSpecification(new SpanStructureSpecification(Ø7_Blue, "Conduit", "Ø7/5", "Blue") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Yellow, "Conduit", "Ø7/5", "Yellow") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_White, "Conduit", "Ø7/5", "White") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Green, "Conduit", "Ø7/5", "Green") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Black, "Conduit", "Ø7/5", "Black") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Red, "Conduit", "Ø7/5", "Red") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Orange, "Conduit", "Ø7/5", "Orange") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Pink, "Conduit", "Ø7/5", "Pink") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Silver, "Conduit", "Ø7/5", "Silver") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Brown, "Conduit", "Ø7/5", "Brown") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Turquoise, "Conduit", "Ø7/5", "Turquoise") { OuterDiameter = 7, InnerDiameter = 5 });
            AddSpecification(new SpanStructureSpecification(Ø7_Violet, "Conduit", "Ø7/5", "Violet") { OuterDiameter = 7, InnerDiameter = 5 });


            AddSpecification(new SpanStructureSpecification(Ø10_Blue, "Conduit", "Ø10/8", "Blue") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Yellow, "Conduit", "Ø10/8", "Yellow") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_White, "Conduit", "Ø10/8", "White") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Green, "Conduit", "Ø10/8", "Green") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Black, "Conduit", "Ø10/8", "Black") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Red, "Conduit", "Ø10/8", "Red") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Orange, "Conduit", "Ø10/8", "Orange") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Pink, "Conduit", "Ø10/8", "Pink") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Silver, "Conduit", "Ø10/8", "Silver") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Brown, "Conduit", "Ø10/8", "Brown") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Turquoise, "Conduit", "Ø10/8", "Turquoise") { OuterDiameter = 10, InnerDiameter = 8 });
            AddSpecification(new SpanStructureSpecification(Ø10_Violet, "Conduit", "Ø10/8", "Violet") { OuterDiameter = 10, InnerDiameter = 8 });

            AddSpecification(new SpanStructureSpecification(Ø12_Orange, "Conduit", "Ø12/10", "Orange") { OuterDiameter = 12, InnerDiameter = 10 });
            AddSpecification(new SpanStructureSpecification(Ø12_Red, "Conduit", "Ø12/10", "Red") { OuterDiameter = 12, InnerDiameter = 10 });

            AddSpecification(new SpanStructureSpecification(Ø32_Orange, "Conduit", "Ø32", "Orange") { OuterDiameter = 32 });
            AddSpecification(new SpanStructureSpecification(Ø40_Orange, "Conduit", "Ø40", "Orange") { OuterDiameter = 40 });
            AddSpecification(new SpanStructureSpecification(Ø40_Red, "Conduit", "Ø40", "Red") { OuterDiameter = 40 });
            AddSpecification(new SpanStructureSpecification(Ø50_Orange, "Conduit", "Ø50", "Orange") { OuterDiameter = 50 });
            AddSpecification(new SpanStructureSpecification(Ø110_Red, "Conduit", "Ø110", "Red") { OuterDiameter = 110 });
        }

        private void AddManufactures()
        {
            // Manufacturer
            AddManufacturer(new Manufacturer(Manu_GMPlast, "GM Plast"));
            AddManufacturer(new Manufacturer(Manu_Emtelle, "Emtelle"));
            AddManufacturer(new Manufacturer(Manu_Fiberpowertech, "Fiberpowertech"));
            AddManufacturer(new Manufacturer(Manu_Cubis, "Wavin"));
        }

        private void AddSpecification(SpanEquipmentSpecification spec)
        {
            var cmd = new AddSpanEquipmentSpecification(spec);
            var cmdResult = _commandDispatcher.HandleAsync<AddSpanEquipmentSpecification, Result>(cmd).Result;

            if (cmdResult.IsFailure)
                throw new ApplicationException(cmdResult.Error);
        }

        private void AddSpecification(SpanStructureSpecification spec)
        {
            var cmd = new AddSpanStructureSpecification(spec);
            var cmdResult = _commandDispatcher.HandleAsync<AddSpanStructureSpecification, Result>(cmd).Result;

            if (cmdResult.IsFailure)
                throw new ApplicationException(cmdResult.Error);
        }

        private void AddSpecification(NodeContainerSpecification spec)
        {
            var cmd = new AddNodeContainerSpecification(spec);
            var cmdResult = _commandDispatcher.HandleAsync<AddNodeContainerSpecification, Result>(cmd).Result;

            if (cmdResult.IsFailure)
                throw new ApplicationException(cmdResult.Error);
        }

        private void AddManufacturer(Manufacturer manufacturer)
        {
            var cmd = new AddManufacturer(manufacturer);
            var cmdResult = _commandDispatcher.HandleAsync<AddManufacturer, Result>(cmd).Result;

            if (cmdResult.IsFailure)
                throw new ApplicationException(cmdResult.Error);
        }
    }
}

