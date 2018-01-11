using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Web.Util;

namespace Web
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
      services.Configure<AGLOptions>(Configuration.GetSection("AGL"));
      services.AddTransient<IPetOwnerService, PetOwnerService>();
      services.AddTransient<IHttpClient, CustomHttpClient>();

      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseExceptionHandler(
        options => {
          options.Run(
            async context =>
            {
              context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
              context.Response.ContentType = "text/html";
              var ex = context.Features.Get<IExceptionHandlerFeature>();
              if (ex != null)
              {
                var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace }";
                await context.Response.WriteAsync(err).ConfigureAwait(false);
              }
            });
        }
      );

      app.UseAngularRoute();

      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseMvc();
    }
  }
}
