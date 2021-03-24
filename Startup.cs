using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.App.Databases;
using Project.App.Helpers;
using Project.App.Mappings;
using Project.App.Middlewares;
using Project.App.Schedules;
using Project.App.Schedules.Jobs;
using Project.Kafka;
using Project.Modules.FaceDetections.Models;
using Project.Modules.FaceDetections.Services;
using Project.Modules.UploadFiles.Services;
using Quartz;
using Repository;

namespace Project
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
            #region Add Cors Service
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(Configuration.GetSection($"AllowedOrigins").Get<string[]>())
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            #endregion

            #region Add Cron Job Service
            services.UseQuartz(typeof(SampleJob));
            #endregion

            #region Add Context Service
            //services.AddDbContextPool<MariaDBContext>(options => options.UseMySql(Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]));
            //services.AddSingleton<MongoDBContext>();
            //services.AddSingleton<RedisDBContext>();
            #endregion

            #region Add Helper Service
            //services.AddScoped<ISortHelper<Customer>, SortHelper<Customer>>();
            #endregion

            #region Add Module Services
            services.AddScoped<IUploadFileService, UploadFileService>();
            services.AddScoped<IFaceDetectionService, FaceDetectionService>();
            services.AddSingleton<KafkaClientHandle>();
            services.AddSingleton<KafkaDependentProducer<string, string>>();
            services.AddSingleton<HandleTask<RegisterFace>>();
            services.AddSingleton<HandleTask<DetectFace>>();
            services.AddSingleton<HandleTask<DeleteFace>>();
            services.AddHostedService<RequestTimeConsumer>();
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            #endregion

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            #region Start Job
            IScheduler scheduler = app.ApplicationServices.GetService<IScheduler>();
            // QuartzServicesUtilitie.StartJob<SampleJob>(scheduler, Configuration["SystemAction:SummarizeRatingJob"]);
            #endregion

            app.UseCors("AllowSpecificOrigin");

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
