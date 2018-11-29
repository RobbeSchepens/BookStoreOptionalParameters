using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Controllers;
using BookStore.Models;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;

namespace BookStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BookStoreContext>(opt => opt.UseInMemoryDatabase("BookLists"));
            services.AddOData();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseODataBatching();
            app.UseMvc(b =>
            {
                b.Select().Expand().Filter().OrderBy().MaxTop(100).Count();
                b.MapODataServiceRoute("odata", "odata", GetEdmModel());
            });
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Book>("Books");
            builder.EntitySet<Press>("Presses");

            var function = builder.EntitySet<Book>("Books").EntityType.Collection.Function(nameof(BooksController.GetBookByAuthorWithRequiredParameter));
            function.Parameter<string>("nameRequired");
            function.Parameter<string>("nameOptional").Optional();
            function.ReturnsFromEntitySet<Book>("Books");

            var function2 = builder.EntitySet<Book>("Books").EntityType.Collection.Function(nameof(BooksController.GetBookByAuthorWithoutRequiredParameter));
            function2.Parameter<string>("nameOptional").Optional();
            function2.ReturnsFromEntitySet<Book>("Books");

            var function3 = builder.EntitySet<Book>("Books").EntityType.Collection.Function(nameof(BooksController.GetBookByAuthorWithTwoOptionalParameters));
            function3.Parameter<string>("nameOptional1").Optional();
            function3.Parameter<string>("nameOptional2").Optional();
            function3.ReturnsFromEntitySet<Book>("Books");

            return builder.GetEdmModel();
        }
    }
}
