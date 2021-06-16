using Akka.Actor;

namespace ClusterSharding.Example.Node
{
    public static class CounterHelper
    {
        public static IActorRef CounterActor { get; set; }
    }
}

