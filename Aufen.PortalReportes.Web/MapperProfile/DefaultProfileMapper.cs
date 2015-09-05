using AutoMapper;
using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.MapperProfile
{
    public class DefaultProfileMapper: Profile
    {
        public override string ProfileName
        {
            get
            {
                return "DefaultProfileMapper";
            }
        }

        protected override void Configure()
        {
            base.Configure();
            CreateMap<Rut, string>()
                .ConvertUsing(x => x.ToString());
            CreateMap<int, Rut>()
                .ConvertUsing(x => new Rut(x));
        }
    }
}