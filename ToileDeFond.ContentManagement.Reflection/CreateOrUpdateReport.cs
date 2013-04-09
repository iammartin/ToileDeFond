namespace ToileDeFond.ContentManagement.Reflection
{
    public class CreateOrUpdateReport<T>
    {
        public CreateOrUpdateActions Action { get; set; }
        public T Item { get; set; }
    }
}