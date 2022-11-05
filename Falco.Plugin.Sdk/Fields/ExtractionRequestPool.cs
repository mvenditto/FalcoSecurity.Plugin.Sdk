using Microsoft.Extensions.ObjectPool;

namespace Falco.Plugin.Sdk.Fields
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
        // this SHOULD create an DisposableObjectPool
        private readonly DefaultObjectPoolProvider _provider;

        private readonly ObjectPool<ExtractionRequest> _pool;

        public ObjectPool<ExtractionRequest> Pool => _pool;

        public ExtractionRequestPool(int maxRetained)
        {
            _provider = new DefaultObjectPoolProvider
            {
                MaximumRetained = maxRetained
            };

            _pool = _provider.Create(
                new ExtractionRequestPooledObjectPolicy());
        }
    }
}
