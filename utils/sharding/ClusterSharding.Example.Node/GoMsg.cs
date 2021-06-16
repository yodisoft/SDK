namespace ClusterSharding.Example.Node
{
    /// <summary>Message: go</summary>
    public class GoMsg
    {
        private static GoMsg _instance;

        /// <summary>Message: go</summary>
        public static GoMsg Instance => _instance ?? (_instance = new GoMsg());

        private GoMsg()
        {
        }
    }
}



































