using System.Linq;
using Reusables.Cqrs;
using Reusables.EventSourcing;

namespace CqrsEventSourcingDemo.Web.Scenarios.Class.ListAllClasses
{
    public class ListAllClassesQueryHandler : IQueryHandler<ListAllClassesQuery, ClassView[]>
    {
        private readonly IViewDatabase _viewDatabase;

        public ListAllClassesQueryHandler(IViewDatabase viewDatabase)
        {
            _viewDatabase = viewDatabase;
        }

        public ClassView[] Handle(ListAllClassesQuery query)
        {
            return _viewDatabase.Set<ClassView>().ToArray();
        }
    }
}
