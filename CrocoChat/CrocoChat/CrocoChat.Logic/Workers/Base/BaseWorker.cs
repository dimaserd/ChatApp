using Croco.Core.Abstractions;
using Croco.Core.Logic.Workers;
using CrocoChat.Logic.Implementations;

namespace CrocoChat.Logic.Workers.Base
{
    public class BaseWorker : BaseCrocoWorker<MyCrocoWebApplication>
    {
        public BaseWorker(ICrocoAmbientContext ambientContext) : base(ambientContext)
        {

        }
    }
}