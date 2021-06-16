using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Persistence;
using ClusterSharding.Example.Common;

namespace ClusterSharding.Example.Node
{
    public class Customer : ReceivePersistentActor
    {
        public override string PersistenceId { get; } = Context.Parent.Path.Name + "/" + Context.Self.Path.Name;

        //public ICollection<string> _purchasedItems = new List<string>();

        public Customer()
        {
            SetReceiveTimeout(TimeSpan.FromSeconds(60));
//            var region = Akka.Cluster.Sharding.ClusterSharding.Get(Context.System).ShardRegion("customer");


            Recover<ItemPurchased>(purchased =>
            {
//                _purchasedItems.Add(purchased.ItemName);
            });

            Command<PurchaseItem>(purchase =>
            {
                //CounterHelper.CounterActor.Tell(GoMsg.Instance);
                PersistAsync(new ItemPurchased(purchase.ItemName), purchased =>
                {
//                    _purchasedItems.Add(purchased.ItemName);
                    var name = Uri.UnescapeDataString(Self.Path.Name);
                    //Console.WriteLine($"'{name}' purchased '{purchased.ItemName}'.\nAll items: [{string.Join(", ", _purchasedItems)}]\n--------------------------");
                    //Console.WriteLine($"'{name}' purchased '{purchased.ItemName}' items: [{_purchasedItems.Count}]");



//                    var region = Akka.Cluster.Sharding.ClusterSharding.Get(Context.System).ShardRegion("customer");
//                            var state = await region.Ask<CurrentShardRegionState>(GetShardRegionState.Instance);
//                    foreach (var shard in state.Shards) 
//                    foreach(var entityId in shard.EntityIds)
//                        Console.WriteLine($"customer/{shard.ShardId}/{entityId}");
//                    Console.WriteLine($"({state.Shards.Count}) >> '{name}' purchased '{purchased.ItemName}' items: [{_purchasedItems.Count}]");

                    CounterHelper.CounterActor.Tell(GoMsg.Instance);
                });
            });
        }
    }
}

