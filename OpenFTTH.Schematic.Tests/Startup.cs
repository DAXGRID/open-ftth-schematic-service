﻿using DAX.EventProcessing;
using Microsoft.Extensions.DependencyInjection;
using OpenFTTH.Address.Business;
using OpenFTTH.Address.Business.Repository;
using OpenFTTH.CQRS;
using OpenFTTH.EventSourcing;
using OpenFTTH.EventSourcing.InMem;
using OpenFTTH.EventSourcing.Postgres;
using OpenFTTH.RouteNetwork.Business.RouteElements.Projection;
using OpenFTTH.RouteNetwork.Business.RouteElements.StateHandling;
using OpenFTTH.TestData;
using System;
using System.Linq;
using System.Reflection;

namespace OpenFTTH.Schematic.Tests
{
    public class Startup
    {
        private static string _connectionString = Environment.GetEnvironmentVariable("test_event_store_connection");

        public void ConfigureServices(IServiceCollection services)
        {
            // Event producer
            services.AddSingleton<IExternalEventProducer, FakeExternalEventProducer>();

            // Route Network State and Repository
            services.AddSingleton<IRouteNetworkState, InMemRouteNetworkState>();
            services.AddSingleton<IRouteNetworkRepository, InMemRouteNetworkRepository>();

            // ES and CQRS
            if (_connectionString == null)
            {
                services.AddSingleton<IEventStore, InMemEventStore>();
            }
            else
            {
                services.AddSingleton<IEventStore>(e =>
                    new PostgresEventStore(e.GetRequiredService<IServiceProvider>(), _connectionString, "schematic_test", true) as IEventStore
                );
            }

            services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

            var businessAssemblies = new Assembly[] {
                AppDomain.CurrentDomain.Load("OpenFTTH.RouteNetwork.Business"),
                AppDomain.CurrentDomain.Load("OpenFTTH.UtilityGraphService.Business"),
                AppDomain.CurrentDomain.Load("OpenFTTH.Schematic.Business"),
                AppDomain.CurrentDomain.Load("OpenFTTH.Address.Business")
            };

            services.AddCQRS(businessAssemblies);

            services.AddProjections(businessAssemblies);

            var routeNetworkProjectionImpl = services
                .First(descriptor =>
                       descriptor.ImplementationType == typeof(RouteNetworkProjection));

            // We remove this because it times out doing testing.
            services.Remove(routeNetworkProjectionImpl);

            // In-mem address service for testing
            services.AddSingleton<IAddressRepository>(x =>
                new InMemAddressRepository(TestAddressData.AccessAddresses)
            );

            // Test Route Network Data
            services.AddSingleton<ITestRouteNetworkData, TestRouteNetwork>();
        }
    }
}
