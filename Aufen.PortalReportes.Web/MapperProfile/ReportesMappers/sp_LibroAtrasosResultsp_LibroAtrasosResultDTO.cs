using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.ReportesModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.MapperProfile.ReportesMappers
{
    public class sp_LibroAtrasosResultsp_LibroAtrasosResultDTO : Profile
    {
        public override string ToString()
        {
            return "sp_LibroAtrasosResult-> sp_LibroAtrasosResultDTO";
        }

        protected override void Configure()
        {
            base.Configure();
            Mapper.CreateMap<sp_LibroAtrasosResult, sp_LibroAtrasosResultDTO>()
                .ForMember(x=>x.Fecha, prop=> prop.MapFrom(x=> DateTime.ParseExact(x.Fecha,"yyyyMMdd", CultureInfo.CurrentCulture)))
                .ForMember(x => x.Entrada, prop => prop.MapFrom(x => DateTime.ParseExact(x.Fecha + (!String.IsNullOrEmpty(x.Entrada) ? "080000" : x.Entrada), "yyyyMMddHHmmss", CultureInfo.CurrentCulture)))
                .ForMember(x => x.Salida, prop => prop.MapFrom(x => DateTime.ParseExact(x.Fecha + (!String.IsNullOrEmpty(x.Salida) ? "180000" : x.Entrada), "yyyyMMddHHmmss", CultureInfo.CurrentCulture)))
                .ForMember(x => x.EntradaTeorica, prop => prop.MapFrom(x => DateTime.ParseExact(x.Fecha + (!String.IsNullOrEmpty(x.EntradaTeorica1) ? "080000" : x.Entrada), "yyyyMMddHHmmss", CultureInfo.CurrentCulture)))
                .ForMember(x => x.SalidaTeorica, prop => prop.MapFrom(x => DateTime.ParseExact(x.Fecha + (!String.IsNullOrEmpty(x.SalidaTeorica1) ? "180000" : x.Entrada), "yyyyMMddHHmmss", CultureInfo.CurrentCulture)))
                .ForMember(x => x.TiempoColacion, prop => prop.MapFrom(x => new TimeSpan((long)x.Colacion*60*60*100)))
                .ForMember(x => x.Rut, prop=> prop.MapFrom(x =>new Rut(x.Rut)))
                ;

        }
    }
}