using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Util;
using ClusterSharding.Example.Common;
using ClusterSharding.Example.Gen.AutomaticJoin;

namespace ClusterSharding.Example.Gen
{
    using ClusterSharding = Akka.Cluster.Sharding.ClusterSharding;

    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("sharded-cluster-system", Cch.Cfg.WithFallback(ClusterSingletonManager.DefaultConfig())))
            {
                var automaticCluster = new AutomaticCluster(system);
                try
                {
                    automaticCluster.Join();

                    RunExample(system);

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
            var proxy = ClusterSharding.Get(system).StartProxy(
                typeName: "customer",
                role: null,
                messageExtractor: new MessageExtractor(10000));

            Thread.Sleep(2000);
//            Console.Write("Press enter to gen messages");
//            Console.ReadLine();

            //var shardRegion2 = ClusterSharding.Get(system).ShardRegion("customer");

            ProduceMessages(system, proxy);
            //Console.ReadLine();
        }

        private static void ProduceMessages(ActorSystem system, IActorRef shardRegion)
        {
            var customers = new[] { "Yoda", "Obi-Wan", "Darth Vader", "Princess Leia", "Luke Skywalker", "R2D2", "Han Solo", "Chewbacca", "Jabba", "Yodisoft" };
            var items = new[] { "Yoghurt", "Fruits", "Lightsaber", "Fluffy toy", "Dreamcatcher", "Candies", "Cigars", "Chicken nuggets", "French fries" };

            system.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10), () =>
            {
                var j = 0;
                while (j < 100)
                {
                    var customer = PickRandom(customers);
                    var item = PickRandom(items);
                    var message = new ShardEnvelope(customer, new PurchaseItem(item));

                    shardRegion.Tell(message);
                    j++;
                }


            });
        }

        private static T PickRandom<T>(T[] items) => items[ThreadLocalRandom.Current.Next(items.Length)];
    }
}

