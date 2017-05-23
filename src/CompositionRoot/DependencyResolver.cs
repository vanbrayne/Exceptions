using Bll.Interfaces;
using Dal;

namespace CompositionRoot
{
    public static class DependencyResolver
    {
        public static IDalTicketClient GetDalTicketClient() => new DalTicketClient();
    }
}
