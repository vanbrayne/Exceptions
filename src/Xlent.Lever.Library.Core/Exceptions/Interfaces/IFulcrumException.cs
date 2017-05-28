namespace Xlent.Lever.Library.Core.Exceptions.Interfaces
{
    public interface IFulcrumException : IFulcrumError
    {
        void CopyFrom(IFulcrumError fulcrumError);
    }
}