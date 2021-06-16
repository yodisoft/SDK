namespace ClusterSharding.Example.Common
{
    public sealed class ItemPurchased
    {
        public readonly string ItemName;

        public ItemPurchased(string itemName)
        {
            ItemName = itemName;
        }
    }
}

