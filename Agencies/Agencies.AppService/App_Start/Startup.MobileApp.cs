using System.Configuration;
using System.Data.Entity;
using System.Web.Http;

using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Tables.Config;

using Agencies.AppService.Models;

using Owin;


namespace Agencies.AppService
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            //For more information on Web API tracing, see http://go.microsoft.com/fwlink/?LinkId=620686
            config.EnableSystemDiagnosticsTracing();

            //new MobileAppConfiguration()
            //    .UseDefaultConfiguration()
            //    .ApplyTo(config);

			new MobileAppConfiguration ()
                .AddMobileAppHomeController ()             // from the Home package
                .MapApiControllers ()
                .AddPushNotifications ()                   // from the Notifications package
                .AddTables (                               // from the Tables package
                    new MobileAppTableConfiguration ()
                        .MapTableControllers ()
                        .AddEntityFramework ()             // from the Entity package
                    )
                //.MapLegacyCrossDomainController ()         // from the CrossDomain package
                .ApplyTo (config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new AgenciesInitializer());

            // To prevent Entity Framework from modifying your database schema, use a null database initializer
            // Database.SetInitializer<AgenciesContext>(null);

            var settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);
        }
    }

    public class AgenciesInitializer : CreateDatabaseIfNotExists<AgenciesContext>
    {
        protected override void Seed(AgenciesContext context)
        {
            //List<TodoItem> todoItems = new List<TodoItem>
            //{
            //    new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
            //    new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            //};

            //foreach (TodoItem todoItem in todoItems)
            //{
            //    context.Set<TodoItem>().Add(todoItem);
            //}

            base.Seed(context);
        }
    }
}

