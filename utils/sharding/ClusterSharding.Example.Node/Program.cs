using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Util;
using ClusterSharding.Example.Common;
using ClusterSharding.Example.Node.AutomaticJoin;

namespace ClusterSharding.Example.Node
{
    using ClusterSharding = Akka.Cluster.Sharding.ClusterSharding;

    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("sharded-cluster-system", Cch2.Cfg.WithFallback(ClusterSingletonManager.DefaultConfig())))
            {
                var automaticCluster = new AutomaticCluster(system);
                try
                {
                    automaticCluster.Join();
                    CounterHelper.CounterActor= system.ActorOf(Props.Create(() => new CounterActor()));


                    RunExample(system);

                    Console.WriteLine("## Is started");
                    Console.ReadLine();
                }
                finally
                {
                    //WARNING: you may need to remove SQLite database file from bin/Debug or bin/Release in case when unexpected crash happened
                    automaticCluster.Leave();
                }
            }
        }

        private static void RunExample(ActorSystem system)
        {
            var sharding = ClusterSharding.Get(system);
            var shardRegion = sharding.Start(
                typeName: "customer",
                entityProps: Props.Create<Customer>(),
                settings: ClusterShardingSettings.Create(system),
                messageExtractor: new MessageExtractor(10000));

//            Thread.Sleep(2000);
//            
//            Console.Write("Press enter to gen messages");
//            Console.ReadLine();
//
//            ProduceMessages(system, shardRegion);
        }

//        private static void ProduceMessages(ActorSystem system, IActorRef shardRegion)
//        {
//            var customers = new[] { "Yoda", "Obi-Wan", "Darth Vader", "Princess Leia", "Luke Skywalker", "R2D2", "Han Solo", "Chewbacca", "Jabba" };
//            var items = new[] { "Yoghurt", "Fruits", "Lightsaber", "Fluffy toy", "Dreamcatcher", "Candies", "Cigars", "Chicken nuggets", "French fries" };
//
//            system.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1), () =>
//            {
//                var customer = PickRandom(customers);
//                var item = PickRandom(items);
//                var message = new ShardEnvelope(customer, new PurchaseItem(item));
//
//                shardRegion.Tell(message);
//            });
//        }
//
//        private static T PickRandom<T>(T[] items) => items[ThreadLocalRandom.Current.Next(items.Length)];
    }
}

