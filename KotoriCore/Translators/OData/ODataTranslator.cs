using System.Diagnostics;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.OData.Core.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OData.UriParser;

namespace KotoriCore.Translators.OData
{
    public class ODataTranslator : ITranslator
    {
        static ODataQueryContext oDataQueryContext { get; set; }
        static IApplicationBuilder app { get; set; }
        static HttpRequest httpRequestMessage { get; set; }
        static HttpContext context;
        static ODataQueryOptions oDataQueryOptions;
        static ODataToSqlTranslator oDataToSqlTranslator;

        static HttpContext CreateHttpContext()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddOData();            
            services.AddMvcCore();
            services.AddOptions();
      
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<DiagnosticSource>(f => new DiagnosticListener("Microsoft.AspNetCore.Mvc"));
            services.AddSingleton<ODataUriResolver>( f => new UnqualifiedODataUriResolver());
            services.AddSingleton<Microsoft.AspNet.OData.Query.Validators.ODataQueryValidator>();
            services.AddSingleton<Microsoft.AspNet.OData.Query.Validators.OrderByQueryValidator>();
            services.AddSingleton<Microsoft.AspNet.OData.Query.Validators.SelectExpandQueryValidator>();
            services.AddSingleton<Microsoft.AspNet.OData.Query.Validators.FilterQueryValidator>();
            services.AddSingleton<Microsoft.AspNet.OData.Query.Validators.TopQueryValidator>();
            
            var sp = services.BuildServiceProvider();
            app = new ApplicationBuilder(sp);
            
            var serializerProvider = sp.GetService<DefaultODataSerializerProvider>();
            
            HttpContext context = new DefaultHttpContext
            {
                RequestServices = sp
            };

            context.Request.ODataFeature().RequestContainer = sp;
            return context;
        }

        public ODataTranslator()
        {
            context = CreateHttpContext();
            httpRequestMessage = context.Request;
            httpRequestMessage.Method = HttpMethods.Get;
            httpRequestMessage.Host = new HostString("http://localhost");
            httpRequestMessage.ContentType = "application/json";
            httpRequestMessage.Query =  new DefaultQueryCollection();
            httpRequestMessage.Path = new PathString("/");

            var type = typeof(MockOpenType);
            var builder = new ODataConventionModelBuilder(context.RequestServices);

            var entityTypeConfiguration = builder.EntitySet<MockOpenType>("MockOpenTypes").EntityType;
            entityTypeConfiguration.HasKey(k => k.Id);
            var edmModels = builder.GetEdmModel();
            app.UseMvc(b => b.MapODataServiceRoute("ODataRoute", "", edmModels));
            oDataQueryContext = new ODataQueryContext(edmModels, type, new Microsoft.AspNet.OData.Routing.ODataPath());
            oDataQueryOptions = new ODataQueryOptions(oDataQueryContext, httpRequestMessage);
            oDataToSqlTranslator = new ODataToSqlTranslator(new SQLQueryFormatter());
        }

        public string Translate(QueryString queryString, TranslateOptions translateOptions = TranslateOptions.ALL, string additionalWhereClause = null)
        {
            httpRequestMessage.QueryString = queryString;
            var oDataQueryOptions = new ODataQueryOptions(oDataQueryContext, httpRequestMessage);

            var oDataToSqlTranslator = new ODataToSqlTranslator(new SQLQueryFormatter());
            var sql = oDataToSqlTranslator.Translate(oDataQueryOptions, translateOptions, additionalWhereClause);

            // TODO: mega ugly hack - new version ADCCure.Azure.Documents.ODataCore.Sql should support $count=true, let's wait
            sql = sql.Replace("c.count(1)", "count(1)");

            return sql;
        }

        public string Translate(ComplexQuery query)
        {
            var q = new QueryString();

            if (query != null)
            {
                if (query.Select != null)
                {
                    if (query.Count)
                        q = q.Add("$select", "count(1)");
                    else
                        q = q.Add("$select", query.Select);
                }

                if (query.Filter != null)
                    q = q.Add("$filter", query.Filter);

                if (query.OrderBy != null)
                {
                    if (!query.Count)
                        q = q.Add("$orderby", query.OrderBy);
                }

                if (query.Top != null &&
                    !query.Count)
                    q = q.Add("$top", query.Top.ToString());
            }

            return Translate(q, TranslateOptions.ALL, query?.AdditionalFilter);
        }
    }
}