//#pragma warning disable CS1702 // Assuming assembly reference matches identity
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;

using Microsoft.Azure.Mobile.Server;

using NomadCode.Azure;

using Agencies.AppService.Models;

namespace Agencies.AppService.Controllers
{
	public class AzureEntityController<T> : TableController<T>
		where T : AzureEntity
	{
		protected override void Initialize (HttpControllerContext controllerContext)
		{
			base.Initialize (controllerContext);

			var context = new AgenciesContext ();

			DomainManager = new EntityDomainManager<T> (context, Request);
		}

		// GET tables/{T}
		public IQueryable<T> GetAll () => Query ();


		// GET tables/{T}/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public SingleResult<T> Get (string id) => Lookup (id);


		// PATCH tables/{T}/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task<T> Patch (string id, Delta<T> patch) => UpdateAsync (id, patch);


		// POST tables/{T}
		public async Task<IHttpActionResult> Post (T item)
		{
			T current = await InsertAsync (item);
			return CreatedAtRoute ("Tables", new { id = current.Id }, current);
		}

		// DELETE tables/{T}/48D68C86-6EA6-4C25-AA33-223FC9A27959
		public Task Delete (string id) => DeleteAsync (id);
	}
}
//#pragma warning restore CS1702 // Assuming assembly reference matches identity