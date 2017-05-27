namespace Xlent.Lever.Library.Core.Exceptions
{
    public interface IFulcrumException
    {
        void CopyFrom(IFulcrumError fulcrumError);
    }
}