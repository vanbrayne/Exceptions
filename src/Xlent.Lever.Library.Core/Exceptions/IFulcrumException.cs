namespace Xlent.Lever.Library.Core.Exceptions
{
    public interface IFulcrumException : IFulcrumError
    {
        void CopyFrom(IFulcrumError fulcrumError);
    }
}