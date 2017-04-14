using NomadCode.Azure;

namespace Agencies.AppService.DataObjects
{
    public class TodoItem : AzureEntity
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}