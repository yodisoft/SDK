namespace ClusterSharding.Example.Node
{
    /// <summary>Message: go</summary>
    public class GoMsg2
    {
        private static GoMsg2 _instance;

        /// <summary>Message: go</summary>
        public static GoMsg2 Instance => _instance ?? (_instance = new GoMsg2());

        private GoMsg2()
        {
        }
    }
}



































