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

        // Span Equipment
        public static Guid Ø32_Orange = Guid.Parse("e7dea74e-df7f-4a0d-a752-98f9e02be1a8");
        public static Guid Ø40_Orange = Guid.Parse("ac417fea-b6f6-4a5a-9c9e-10ee05ecbf56");
        public static Guid Ø40_Red = Guid.Parse("be4deb0f-8d15-49ba-bbeb-fafb4ed66de5");
        public static Guid Ø50_Orange = Guid.Parse("7960355a-4dab-4d60-b3a5-e20ac4301176");
        public static Guid Ø110_Orange = Guid.Parse("150fdd01-80d5-41fd-b0c5-97d2fbe91fa0");

        public static Guid Multi_Ø32_3x10 = Guid.Parse("b11a4fce-2116-4437-9108-3ca467124d99");
        public static Guid Multi_Ø40_5x10 = Guid.Parse("7ca9dcbb-524f-4d61-945c-16bf2679326e");
        public static Guid Multi_Ø40_6x10 = Guid.Parse("f8d15ef6-b07f-440b-8357-4c7a3f84f156");
        public static Guid Multi_Ø50_10x10 = Guid.Parse("1c2a1e9e-03e6-4eb9-ae89-e723fea1e59c");
        public static Guid Multi_Ø50_5x10_12_7_MultiColor = Guid.Parse("36f0deaf-0d77-4cae-be06-1e6e0cf84ae2");
        public static Guid Multi_Ø50_5x10_12_7_BlueYellow = Guid.Parse("dc83bdd3-142b-49ff-8a80-d8d7e1d794b3");
        public static Guid Multi_Ø50_5x10_12_7_GreenWhite = Guid.Parse("2fe1a566-6477-4f24-b7df-e242bd6c7d7d");

        public static Guid Flex_Ø40_Red = Guid.Parse("6df48525-ac10-4b02-b1cd-05283b549ab2");

        public TestSpecifications Run()
        {
            if (_specificationsCreated)
                return this;

            lock (_myLock)
            {
                // double-checked locking
                if (_specificationsCreated)
                    return this;

                var manufacturerQueryResult = _queryDispatcher.HandleAsync<GetManufacturer, Result<LookupCollection<Manufacturer>>>(new GetManufacturer()).Result;

                if (manufacturerQueryResult.Value.ContainsKey(Manu_GMPlast))
                    return this;

                AddManufactures();

                AddNodeContainerSpecifications();

                AddSpanStructureSpecifications();

                AddSpanEquipmentSpecifications();

                Thread.Sleep(100);

                _specificationsCreated = true;

                return this;
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
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle }
            });

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
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle }
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
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle }
            });

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
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle }
            });

            AddSpecification(new SpanEquipmentSpecification(Multi_Ø50_5x10_12_7_MultiColor, "Conduit", "Ø50 10x10",
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
                Description = "ø50 mm Multirør 5x10 + 12x7 color",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle }
            });

            AddSpecification(new SpanEquipmentSpecification(Flex_Ø40_Red, "Conduit", "Ø40 Flex", new SpanStructureTemplate(Ø40_Red, 1, 1, Array.Empty<SpanStructureTemplate>()))
            {
                Description = "ø40 mm Flexrør",
                ManufacturerRefs = new Guid[] { Manu_GMPlast, Manu_Emtelle }
            });
        }

        private void AddSpanStructureSpecifications()
        {
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

            AddSpecification(new SpanStructureSpecification(Ø32_Orange, "Conduit", "Ø32", "Orange") { OuterDiameter = 32 });
            AddSpecification(new SpanStructureSpecification(Ø40_Orange, "Conduit", "Ø40", "Orange") { OuterDiameter = 40 });
            AddSpecification(new SpanStructureSpecification(Ø40_Red, "Conduit", "Ø40", "Red") { OuterDiameter = 40 });
            AddSpecification(new SpanStructureSpecification(Ø50_Orange, "Conduit", "Ø50", "Orange") { OuterDiameter = 40 });
            AddSpecification(new SpanStructureSpecification(Ø110_Orange, "Conduit", "Ø110", "Orange") { OuterDiameter = 40 });
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

