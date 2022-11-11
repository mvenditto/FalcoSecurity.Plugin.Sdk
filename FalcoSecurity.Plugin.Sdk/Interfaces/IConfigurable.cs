namespace FalcoSecurity.Plugin.Sdk
{
    public interface IConfigurable<T>
    {
        public T? Config { get; set; }
    }
}
