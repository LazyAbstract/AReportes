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
    public class sp_LibroAtrasosResultsp_LibroInasistenciaDTO : Profile
    {
        public override string ToString()
        {
            return "sp_LibroAtrasosResult-> sp_LibroAtrasosResultDTO";
        }

        protected override void Configure()
        {
            base.Configure();
            DateTime? nulo = null;
            Mapper.CreateMap<sp_LibroInasistenciaResult, LibroInasistenciaDTO>()
                .ForMember(x => x.Fecha, prop=> prop.MapFrom(x => x.Fecha != null ? DateTime.ParseExact(x.Fecha,"yyyyMMdd", CultureInfo.CurrentCulture) : DateTime.Now))
                .ForMember(x => x.EntradaTeorica, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.EntradaTeorica1) && !String.IsNullOrWhiteSpace(x.EntradaTeorica1) ? DateTime.ParseExact(x.Fecha + x.EntradaTeorica1, "yyyyMMddHHmm", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.SalidaTeorica, prop => prop.MapFrom(x => !String.IsNullOrEmpty(x.SalidaTeorica1) && !String.IsNullOrWhiteSpace(x.SalidaTeorica1) ? DateTime.ParseExact(x.Fecha + x.SalidaTeorica1, "yyyyMMddHHmm", CultureInfo.CurrentCulture) : nulo))
                .ForMember(x => x.Rut, prop=> prop.MapFrom(x =>!String.IsNullOrEmpty(x.Rut) ? new Rut(x.Rut): null))
                .ForMember(x => x.Nombres, prop => prop.MapFrom(x => x.Nombre))
                ;
        }
    }
}