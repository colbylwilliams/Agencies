
using Agencies.AppService.DataObjects;
using Agencies.AppService.Models;

using NomadCode.Azure.Controllers;

namespace Agencies.AppService.Controllers
{
    public class TodoItemController : AzureEntityController<TodoItem, AgenciesContext>
    {
    }
}