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
                .ForMember(x => x.Fecha, prop=> prop.MapFrom(x => x.Fecha != null ? DateTime.ParseExact(x.Fecha,"yyyyMMdd", CultureInfo.CurrentCulture) : DateTime.Now))
                .ForMember(x => x.Entrada, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.Entrada) && !String.IsNullOrWhiteSpace(x.Entrada) ? DateTime.ParseExact(x.Fecha + (x.Entrada), "yyyyMMddHHmmss", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.EntradaColacion, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.EntradaColacion) && !String.IsNullOrWhiteSpace(x.EntradaColacion) ? DateTime.ParseExact(x.Fecha + (x.EntradaColacion), "yyyyMMddHHmmss", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.Salida, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.Salida) && !String.IsNullOrWhiteSpace(x.Salida) ? DateTime.ParseExact(x.Fecha + x.Salida, "yyyyMMddHHmmss", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.SalidaColacion, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.SalidaColacion) && !String.IsNullOrWhiteSpace(x.SalidaColacion) ? DateTime.ParseExact(x.Fecha + x.SalidaColacion, "yyyyMMddHHmmss", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.EntradaTeorica, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.EntradaTeorica1) && !String.IsNullOrWhiteSpace(x.EntradaTeorica1) ? DateTime.ParseExact(x.Fecha + x.EntradaTeorica1, "yyyyMMddHHmm", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.SalidaTeorica, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.SalidaTeorica1) && !String.IsNullOrWhiteSpace(x.SalidaTeorica1) ? DateTime.ParseExact(x.Fecha + x.SalidaTeorica1, "yyyyMMddHHmm", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.TiempoColacion, prop => prop.MapFrom(x => TimeSpan.FromHours((double)x.Colacion.GetValueOrDefault(0))))
                .ForMember(x => x.Rut, prop=> prop.MapFrom(x =>x.Rut))
                .ForMember(x => x.Nombres, prop => prop.MapFrom(x => x.Nombre))
                .ForMember(x=> x.EsPermiso, prop => prop.MapFrom(x=> x.EsPermiso))
                ;
        }
    }
}