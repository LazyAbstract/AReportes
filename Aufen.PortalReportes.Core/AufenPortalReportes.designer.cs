﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aufen.PortalReportes.Core
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ZkTimeEnterprise")]
	public partial class AufenPortalReportesDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertEMPRESA(EMPRESA instance);
    partial void UpdateEMPRESA(EMPRESA instance);
    partial void DeleteEMPRESA(EMPRESA instance);
    #endregion
		
		public AufenPortalReportesDataContext() : 
				base(global::Aufen.PortalReportes.Core.Properties.Settings.Default.ZkTimeEnterpriseConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public AufenPortalReportesDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AufenPortalReportesDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AufenPortalReportesDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AufenPortalReportesDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<vw_Ubicacione> vw_Ubicaciones
		{
			get
			{
				return this.GetTable<vw_Ubicacione>();
			}
		}
		
		public System.Data.Linq.Table<EMPRESA> EMPRESAs
		{
			get
			{
				return this.GetTable<EMPRESA>();
			}
		}
		
		public System.Data.Linq.Table<vw_Empleado> vw_Empleados
		{
			get
			{
				return this.GetTable<vw_Empleado>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_LibroInasistencia")]
		public ISingleResult<sp_LibroInasistenciaResult> sp_LibroInasistencia([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(9)")] string fechaInicio, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(9)")] string fechaFin, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(2)")] string num, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(2)")] string depto, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(9)")] string rut)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), fechaInicio, fechaFin, num, depto, rut);
			return ((ISingleResult<sp_LibroInasistenciaResult>)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.sp_LibroAsistencia")]
		public ISingleResult<sp_LibroAsistenciaResult> sp_LibroAsistencia([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(8)")] string fechaInicio, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(8)")] string fechaFin, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(2)")] string num, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(2)")] string depto, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(9)")] string rut)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), fechaInicio, fechaFin, num, depto, rut);
			return ((ISingleResult<sp_LibroAsistenciaResult>)(result.ReturnValue));
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.vw_Ubicaciones")]
	public partial class vw_Ubicacione
	{
		
		private string _Codigo;
		
		private string _Descripcion;
		
		private string _IdEmpresa;
		
		public vw_Ubicacione()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Codigo", DbType="Char(2) NOT NULL", CanBeNull=false)]
		public string Codigo
		{
			get
			{
				return this._Codigo;
			}
			set
			{
				if ((this._Codigo != value))
				{
					this._Codigo = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Descripcion", DbType="Char(50)")]
		public string Descripcion
		{
			get
			{
				return this._Descripcion;
			}
			set
			{
				if ((this._Descripcion != value))
				{
					this._Descripcion = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdEmpresa", DbType="VarChar(2) NOT NULL", CanBeNull=false)]
		public string IdEmpresa
		{
			get
			{
				return this._IdEmpresa;
			}
			set
			{
				if ((this._IdEmpresa != value))
				{
					this._IdEmpresa = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.EMPRESAS")]
	public partial class EMPRESA : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Codigo;
		
		private string _Descripcion;
		
		private string _EtiquetaUbicacion;
		
		private string _EtiquetaCentro;
		
		private System.Nullable<byte> _FormatoNombre;
		
		private string _UltimaVersion;
		
		private string _Direccion;
		
		private string _Fono;
		
		private string _Fax;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnCodigoChanging(string value);
    partial void OnCodigoChanged();
    partial void OnDescripcionChanging(string value);
    partial void OnDescripcionChanged();
    partial void OnEtiquetaUbicacionChanging(string value);
    partial void OnEtiquetaUbicacionChanged();
    partial void OnEtiquetaCentroChanging(string value);
    partial void OnEtiquetaCentroChanged();
    partial void OnFormatoNombreChanging(System.Nullable<byte> value);
    partial void OnFormatoNombreChanged();
    partial void OnUltimaVersionChanging(string value);
    partial void OnUltimaVersionChanged();
    partial void OnDireccionChanging(string value);
    partial void OnDireccionChanged();
    partial void OnFonoChanging(string value);
    partial void OnFonoChanged();
    partial void OnFaxChanging(string value);
    partial void OnFaxChanged();
    #endregion
		
		public EMPRESA()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Codigo", DbType="Char(3) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string Codigo
		{
			get
			{
				return this._Codigo;
			}
			set
			{
				if ((this._Codigo != value))
				{
					this.OnCodigoChanging(value);
					this.SendPropertyChanging();
					this._Codigo = value;
					this.SendPropertyChanged("Codigo");
					this.OnCodigoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Descripcion", DbType="Char(60)")]
		public string Descripcion
		{
			get
			{
				return this._Descripcion;
			}
			set
			{
				if ((this._Descripcion != value))
				{
					this.OnDescripcionChanging(value);
					this.SendPropertyChanging();
					this._Descripcion = value;
					this.SendPropertyChanged("Descripcion");
					this.OnDescripcionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EtiquetaUbicacion", DbType="Char(15)")]
		public string EtiquetaUbicacion
		{
			get
			{
				return this._EtiquetaUbicacion;
			}
			set
			{
				if ((this._EtiquetaUbicacion != value))
				{
					this.OnEtiquetaUbicacionChanging(value);
					this.SendPropertyChanging();
					this._EtiquetaUbicacion = value;
					this.SendPropertyChanged("EtiquetaUbicacion");
					this.OnEtiquetaUbicacionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EtiquetaCentro", DbType="Char(15)")]
		public string EtiquetaCentro
		{
			get
			{
				return this._EtiquetaCentro;
			}
			set
			{
				if ((this._EtiquetaCentro != value))
				{
					this.OnEtiquetaCentroChanging(value);
					this.SendPropertyChanging();
					this._EtiquetaCentro = value;
					this.SendPropertyChanged("EtiquetaCentro");
					this.OnEtiquetaCentroChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FormatoNombre", DbType="TinyInt")]
		public System.Nullable<byte> FormatoNombre
		{
			get
			{
				return this._FormatoNombre;
			}
			set
			{
				if ((this._FormatoNombre != value))
				{
					this.OnFormatoNombreChanging(value);
					this.SendPropertyChanging();
					this._FormatoNombre = value;
					this.SendPropertyChanged("FormatoNombre");
					this.OnFormatoNombreChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UltimaVersion", DbType="Char(7) NOT NULL", CanBeNull=false)]
		public string UltimaVersion
		{
			get
			{
				return this._UltimaVersion;
			}
			set
			{
				if ((this._UltimaVersion != value))
				{
					this.OnUltimaVersionChanging(value);
					this.SendPropertyChanging();
					this._UltimaVersion = value;
					this.SendPropertyChanged("UltimaVersion");
					this.OnUltimaVersionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Direccion", DbType="VarChar(255)")]
		public string Direccion
		{
			get
			{
				return this._Direccion;
			}
			set
			{
				if ((this._Direccion != value))
				{
					this.OnDireccionChanging(value);
					this.SendPropertyChanging();
					this._Direccion = value;
					this.SendPropertyChanged("Direccion");
					this.OnDireccionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Fono", DbType="VarChar(255)")]
		public string Fono
		{
			get
			{
				return this._Fono;
			}
			set
			{
				if ((this._Fono != value))
				{
					this.OnFonoChanging(value);
					this.SendPropertyChanging();
					this._Fono = value;
					this.SendPropertyChanged("Fono");
					this.OnFonoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Fax", DbType="VarChar(255)")]
		public string Fax
		{
			get
			{
				return this._Fax;
			}
			set
			{
				if ((this._Fax != value))
				{
					this.OnFaxChanging(value);
					this.SendPropertyChanging();
					this._Fax = value;
					this.SendPropertyChanged("Fax");
					this.OnFaxChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.vw_Empleado")]
	public partial class vw_Empleado
	{
		
		private System.Nullable<int> _Codigo;
		
		private string _IdEmpresa;
		
		private string _Nombre;
		
		private string _Apellidos;
		
		private string _IdCentro;
		
		private string _NombreCentro;
		
		private string _IdUbicacion;
		
		private string _NombreUbicacion;
		
		private string _Tarjeta;
		
		public vw_Empleado()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Codigo", DbType="Int")]
		public System.Nullable<int> Codigo
		{
			get
			{
				return this._Codigo;
			}
			set
			{
				if ((this._Codigo != value))
				{
					this._Codigo = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdEmpresa", DbType="VarChar(2) NOT NULL", CanBeNull=false)]
		public string IdEmpresa
		{
			get
			{
				return this._IdEmpresa;
			}
			set
			{
				if ((this._IdEmpresa != value))
				{
					this._IdEmpresa = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Nombre", DbType="Char(40)")]
		public string Nombre
		{
			get
			{
				return this._Nombre;
			}
			set
			{
				if ((this._Nombre != value))
				{
					this._Nombre = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Apellidos", DbType="Char(60)")]
		public string Apellidos
		{
			get
			{
				return this._Apellidos;
			}
			set
			{
				if ((this._Apellidos != value))
				{
					this._Apellidos = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdCentro", DbType="Char(2) NOT NULL", CanBeNull=false)]
		public string IdCentro
		{
			get
			{
				return this._IdCentro;
			}
			set
			{
				if ((this._IdCentro != value))
				{
					this._IdCentro = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NombreCentro", DbType="Char(50)")]
		public string NombreCentro
		{
			get
			{
				return this._NombreCentro;
			}
			set
			{
				if ((this._NombreCentro != value))
				{
					this._NombreCentro = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdUbicacion", DbType="Char(2) NOT NULL", CanBeNull=false)]
		public string IdUbicacion
		{
			get
			{
				return this._IdUbicacion;
			}
			set
			{
				if ((this._IdUbicacion != value))
				{
					this._IdUbicacion = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NombreUbicacion", DbType="Char(50)")]
		public string NombreUbicacion
		{
			get
			{
				return this._NombreUbicacion;
			}
			set
			{
				if ((this._NombreUbicacion != value))
				{
					this._NombreUbicacion = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Tarjeta", DbType="Char(10)")]
		public string Tarjeta
		{
			get
			{
				return this._Tarjeta;
			}
			set
			{
				if ((this._Tarjeta != value))
				{
					this._Tarjeta = value;
				}
			}
		}
	}
	
	public partial class sp_LibroInasistenciaResult
	{
		
		private string _Fecha;
		
		private string _Rut;
		
		private string _Nombre;
		
		private string _Apellidos;
		
		private string _IdEmpresa;
		
		private string _IdDepartamento;
		
		private string _EntradaTeorica1;
		
		private string _SalidaTeorica1;
		
		private string _Observacion;
		
		public sp_LibroInasistenciaResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Fecha", DbType="VarChar(8) NOT NULL", CanBeNull=false)]
		public string Fecha
		{
			get
			{
				return this._Fecha;
			}
			set
			{
				if ((this._Fecha != value))
				{
					this._Fecha = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Rut", DbType="VarChar(9) NOT NULL", CanBeNull=false)]
		public string Rut
		{
			get
			{
				return this._Rut;
			}
			set
			{
				if ((this._Rut != value))
				{
					this._Rut = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Nombre", DbType="VarChar(7) NOT NULL", CanBeNull=false)]
		public string Nombre
		{
			get
			{
				return this._Nombre;
			}
			set
			{
				if ((this._Nombre != value))
				{
					this._Nombre = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Apellidos", DbType="VarChar(9) NOT NULL", CanBeNull=false)]
		public string Apellidos
		{
			get
			{
				return this._Apellidos;
			}
			set
			{
				if ((this._Apellidos != value))
				{
					this._Apellidos = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdEmpresa", DbType="VarChar(1) NOT NULL", CanBeNull=false)]
		public string IdEmpresa
		{
			get
			{
				return this._IdEmpresa;
			}
			set
			{
				if ((this._IdEmpresa != value))
				{
					this._IdEmpresa = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdDepartamento", DbType="VarChar(2) NOT NULL", CanBeNull=false)]
		public string IdDepartamento
		{
			get
			{
				return this._IdDepartamento;
			}
			set
			{
				if ((this._IdDepartamento != value))
				{
					this._IdDepartamento = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EntradaTeorica1", DbType="VarChar(4) NOT NULL", CanBeNull=false)]
		public string EntradaTeorica1
		{
			get
			{
				return this._EntradaTeorica1;
			}
			set
			{
				if ((this._EntradaTeorica1 != value))
				{
					this._EntradaTeorica1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalidaTeorica1", DbType="VarChar(4) NOT NULL", CanBeNull=false)]
		public string SalidaTeorica1
		{
			get
			{
				return this._SalidaTeorica1;
			}
			set
			{
				if ((this._SalidaTeorica1 != value))
				{
					this._SalidaTeorica1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Observacion", DbType="VarChar(10) NOT NULL", CanBeNull=false)]
		public string Observacion
		{
			get
			{
				return this._Observacion;
			}
			set
			{
				if ((this._Observacion != value))
				{
					this._Observacion = value;
				}
			}
		}
	}
	
	public partial class sp_LibroAsistenciaResult
	{
		
		private string _Fecha;
		
		private System.Nullable<int> _NumSemana;
		
		private string _Rut;
		
		private string _Nombre;
		
		private string _Apellidos;
		
		private string _IdHorario;
		
		private string _IdEmpresa;
		
		private string _IdDepartamento;
		
		private string _IdCalendario;
		
		private string _Entrada;
		
		private string _SalidaColacion;
		
		private string _EntradaColacion;
		
		private string _Salida;
		
		private string _EntradaTeorica1;
		
		private string _SalidaTeorica1;
		
		private System.Nullable<decimal> _Colacion;
		
		private string _Observacion;
		
		private System.Nullable<bool> _EsPermiso;
		
		public sp_LibroAsistenciaResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Fecha", DbType="VarChar(8)")]
		public string Fecha
		{
			get
			{
				return this._Fecha;
			}
			set
			{
				if ((this._Fecha != value))
				{
					this._Fecha = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NumSemana", DbType="Int")]
		public System.Nullable<int> NumSemana
		{
			get
			{
				return this._NumSemana;
			}
			set
			{
				if ((this._NumSemana != value))
				{
					this._NumSemana = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Rut", DbType="Char(9)")]
		public string Rut
		{
			get
			{
				return this._Rut;
			}
			set
			{
				if ((this._Rut != value))
				{
					this._Rut = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Nombre", DbType="Char(40)")]
		public string Nombre
		{
			get
			{
				return this._Nombre;
			}
			set
			{
				if ((this._Nombre != value))
				{
					this._Nombre = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Apellidos", DbType="Char(60)")]
		public string Apellidos
		{
			get
			{
				return this._Apellidos;
			}
			set
			{
				if ((this._Apellidos != value))
				{
					this._Apellidos = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdHorario", DbType="VarChar(255)")]
		public string IdHorario
		{
			get
			{
				return this._IdHorario;
			}
			set
			{
				if ((this._IdHorario != value))
				{
					this._IdHorario = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdEmpresa", DbType="VarChar(2) NOT NULL", CanBeNull=false)]
		public string IdEmpresa
		{
			get
			{
				return this._IdEmpresa;
			}
			set
			{
				if ((this._IdEmpresa != value))
				{
					this._IdEmpresa = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdDepartamento", DbType="VarChar(255)")]
		public string IdDepartamento
		{
			get
			{
				return this._IdDepartamento;
			}
			set
			{
				if ((this._IdDepartamento != value))
				{
					this._IdDepartamento = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdCalendario", DbType="Char(9)")]
		public string IdCalendario
		{
			get
			{
				return this._IdCalendario;
			}
			set
			{
				if ((this._IdCalendario != value))
				{
					this._IdCalendario = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Entrada", DbType="Char(6)")]
		public string Entrada
		{
			get
			{
				return this._Entrada;
			}
			set
			{
				if ((this._Entrada != value))
				{
					this._Entrada = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalidaColacion", DbType="VarChar(255)")]
		public string SalidaColacion
		{
			get
			{
				return this._SalidaColacion;
			}
			set
			{
				if ((this._SalidaColacion != value))
				{
					this._SalidaColacion = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EntradaColacion", DbType="VarChar(255)")]
		public string EntradaColacion
		{
			get
			{
				return this._EntradaColacion;
			}
			set
			{
				if ((this._EntradaColacion != value))
				{
					this._EntradaColacion = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Salida", DbType="Char(6)")]
		public string Salida
		{
			get
			{
				return this._Salida;
			}
			set
			{
				if ((this._Salida != value))
				{
					this._Salida = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EntradaTeorica1", DbType="Char(4)")]
		public string EntradaTeorica1
		{
			get
			{
				return this._EntradaTeorica1;
			}
			set
			{
				if ((this._EntradaTeorica1 != value))
				{
					this._EntradaTeorica1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalidaTeorica1", DbType="Char(4)")]
		public string SalidaTeorica1
		{
			get
			{
				return this._SalidaTeorica1;
			}
			set
			{
				if ((this._SalidaTeorica1 != value))
				{
					this._SalidaTeorica1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Colacion", DbType="Decimal(2,1)")]
		public System.Nullable<decimal> Colacion
		{
			get
			{
				return this._Colacion;
			}
			set
			{
				if ((this._Colacion != value))
				{
					this._Colacion = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Observacion", DbType="VarChar(32)")]
		public string Observacion
		{
			get
			{
				return this._Observacion;
			}
			set
			{
				if ((this._Observacion != value))
				{
					this._Observacion = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EsPermiso", DbType="Bit")]
		public System.Nullable<bool> EsPermiso
		{
			get
			{
				return this._EsPermiso;
			}
			set
			{
				if ((this._EsPermiso != value))
				{
					this._EsPermiso = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
