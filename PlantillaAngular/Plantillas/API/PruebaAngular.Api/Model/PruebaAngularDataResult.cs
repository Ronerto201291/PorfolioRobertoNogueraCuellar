

namespace PruebaAngular.Api.Model
{
    public class PruebaAngularDataResult<T> : ApiDataResult<T>
    {
        public new T Data { get; set; }

        public PruebaAngularDataResult() : base(default)
        {

        }
    }
}
