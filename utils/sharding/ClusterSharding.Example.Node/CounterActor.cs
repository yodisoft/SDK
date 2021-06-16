using System;
using Akka.Actor;

namespace ClusterSharding.Example.Node
{
    internal class CounterActor : ReceiveActor
    {
        private int _cn;
        public CounterActor()
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(1),Self, GoMsg2.Instance, Self  );
            Receive<GoMsg>(s => _cn++);
            Receive<GoMsg2>(s =>
            {
                Console.WriteLine(">>" + _cn);
                _cn = 0;
            });
        }

    }
}



































