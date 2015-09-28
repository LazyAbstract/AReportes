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
            DateTime? nulo = null;
            Mapper.CreateMap<sp_LibroAsistenciaResult, LibroAsistenciaDTO>()
                .ForMember(x=>x.Fecha, prop=> prop.MapFrom(x=> DateTime.ParseExact(x.Fecha,"yyyyMMdd", CultureInfo.CurrentCulture)))
                .ForMember(x => x.Entrada, prop => {
                    prop.MapFrom(x => !String.IsNullOrWhiteSpace(x.Entrada) ? DateTime.ParseExact(x.Fecha + (x.Entrada), "yyyyMMddHHmmss", CultureInfo.CurrentCulture) : nulo);
                })
                .ForMember(x => x.Salida, prop =>
                    {
                        prop.MapFrom(x => !String.IsNullOrWhiteSpace(x.Salida) ?  DateTime.ParseExact(x.Fecha + x.Salida, "yyyyMMddHHmmss", CultureInfo.CurrentCulture): nulo);
                    })
                .ForMember(x => x.EntradaTeorica, prop => {
                    prop.MapFrom(x => !String.IsNullOrWhiteSpace(x.EntradaTeorica1) ? DateTime.ParseExact(x.Fecha + x.EntradaTeorica1, "yyyyMMddHHmm", CultureInfo.CurrentCulture) : nulo);
                })
                .ForMember(x => x.SalidaTeorica, prop => {
                    prop.MapFrom(x => !String.IsNullOrWhiteSpace(x.SalidaTeorica1) ? DateTime.ParseExact(x.Fecha + x.SalidaTeorica1, "yyyyMMddHHmm", CultureInfo.CurrentCulture) : nulo);
                })
                .ForMember(x => x.TiempoColacion, prop => prop.MapFrom(x =>  new TimeSpan(x.Colacion.HasValue ? (long)x.Colacion*60*60*100 : 0)))
                .ForMember(x => x.Rut, prop=> prop.MapFrom(x =>!String.IsNullOrEmpty(x.Rut) ?  new Rut(x.Rut) : new Rut()))
                ;

        }
    }
}