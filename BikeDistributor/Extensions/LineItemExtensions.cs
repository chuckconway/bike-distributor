namespace BikeDistributor.Extensions
{
    public static class LineItemExtensions
    {
        public static string Name(this Line line)
        {
            return string.Format("{0} - {1}", line.Item.Brand, line.Item.Model);
        }
    }
}
