using Microsoft.Extensions.ObjectPool;

namespace FalcoSecurity.Plugin.Sdk.Fields
{
    internal class ExtractionRequestPooledObjectPolicy: IPooledObjectPolicy<ExtractionRequest>
    {
        public ExtractionRequest Create()
        {
            return new ExtractionRequest();
        }

        public bool Return(ExtractionRequest req)
        {
            return true;
        }
    }

    public class ExtractionRequestPool
    {
        private readonly ObjectPool<ExtractionRequest> _pool;

        public ObjectPool<ExtractionRequest> Pool => _pool;

        public ExtractionRequestPool(int maxRetained)
        {
            var provider = new DefaultObjectPoolProvider
            {
                MaximumRetained = maxRetained
            };

            _pool = provider.Create(
                new ExtractionRequestPooledObjectPolicy());
        }
    }
}
