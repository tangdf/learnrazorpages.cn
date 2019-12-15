using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using LearnRazorPages.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace LearnRazorPages
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
            services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/index", "{*url}");
            });

            services.AddResponseCaching(options => { options.UseCaseSensitivePaths = false; });

            services.Configure<List<PageInfo>>(Configuration.GetSection("SiteMap"));

            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

           
        }
 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/index/{0}");
            }

            app.UseStaticFiles(new StaticFileOptions {
                OnPrepareResponse = context =>
                {
                        const int durationInSeconds = 60 * 60;
                        context.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
                }
            });
            app.UseRouting();

            app.UseResponseCaching();

            app.UseEndpoints(builder=>builder.MapRazorPages());
        }
    }
}
