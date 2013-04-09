namespace ToileDeFond.ContentManagement.DataTransfer
{
    public abstract class Mapper<TType, TDto>
    {
        public abstract TDto ToDto(TType type);
        public abstract TType FromDto(TDto dto);
    }
}