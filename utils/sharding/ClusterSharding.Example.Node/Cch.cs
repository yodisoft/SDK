using Akka.Configuration;

namespace ClusterSharding.Example.Node
{
    public class Cch2
    {
        public static Config Cfg = @"
            #redis {
            #  host = ""localhost""
            #  port = 6379
            #  }
            akka {
              actor {
                provider = cluster
                serializers {
                  hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                }
                serialization-bindings {
                  ""System.Object"" = hyperion
                }
              }
              remote {
                dot-netty.tcp {
                        transport-class =""Akka.Remote.Transport.DotNetty.TcpTransport, Akka.Remote""
      applied-adapters=[]
      transport-protocol=tcp      

                  public-hostname = ""localhost""
                  hostname = ""localhost""
                  port = 0
                }
              }
              cluster {
                auto-down-unreachable-after = 5s
                sharding {
                  least-shard-allocation-strategy.rebalance-threshold = 1 #2 #3
                }
              }
              persistence {
                journal {


        plugin = ""akka.persistence.journal.redis""
        auto-start-journals = [""akka.persistence.journal.redis""]
        redis {
          class = ""Akka.Persistence.Redis.Journal.RedisJournal, Akka.Persistence.Redis""
          configuration-string = ""localhost:6379,abortConnect=false,syncTimeout=3000""
          plugin-dispatcher = ""akka.actor.default-dispatcher""
          key-prefix = ""akka:persistence:journal""
          }




                }
                snapshot-store {

        plugin = ""akka.persistence.snapshot-store.redis""
        auto-start-snapshot-stores = [""akka.persistence.snapshot-store.redis""]
        redis {
          class=""Akka.Persistence.Redis.Snapshot.RedisSnapshotStore, Akka.Persistence.Redis""
          configuration-string=""localhost:6379,abortConnect=false,syncTimeout=3000""
          plugin-dispatcher = ""akka.actor.default-dispatcher""
          key-prefix = ""akka:persistence:snapshots""
          }



                }
              }
            }";
    }
}

