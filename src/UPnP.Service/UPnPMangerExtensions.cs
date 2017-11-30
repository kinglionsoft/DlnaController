using Microsoft.AspNetCore.Builder;
using System;
using System.Linq;
using UPnP.Service;
using AutoMapper;
using DlnaController.Abstractions;
using DlnaController.Domain;
using SV.UPnPLite.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UPnPMangerExtensions
    {
        public static IServiceCollection AddUPnPManager(this IServiceCollection services)
        {
            services.AddSingleton<UpnpManager>();
            return services;
        }
        
        public static IApplicationBuilder UseUPnPManager(this IApplicationBuilder app)
        {
            CreateDtoMap();

            app.ApplicationServices.GetService<UpnpManager>().Start();
            return app;
        }

        private static void CreateDtoMap()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<VideoItem, VideoItemDto>()
                    .ForMember(dto => dto.Duration,
                        opt => opt.MapFrom(video => video.Resources.First().Duration.ToString(@"hh\:mm\:ss")))
                    .ForMember(d => d.Size, 
                        opt => opt.MapFrom(v => v.Resources.First().Size.ToHumanString()))
                    .ForMember(d => d.Resolution, 
                        opt => opt.MapFrom(v => v.Resources.First().Resolution.ToString()));

                config.CreateMap<MediaRenderer, UpnpServerDto>();
                config.CreateMap<MediaServer, UpnpServerDto>();
            });
        }
    }
}
