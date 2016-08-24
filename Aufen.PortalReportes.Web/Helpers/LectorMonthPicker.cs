using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Helpers
{
    public class LectorMonthPicker
    {
        private int Anno { get; set; }
        private int Mes { get; set; }
        private string NombreMes { get; set; }

        public LectorMonthPicker() { }

        public LectorMonthPicker(string MesAnno)
        {
            string[] Periodo = MesAnno.Split(' ');
            Anno = Convert.ToInt32(Periodo[1]);
            Mes = CaseMonth(Periodo[0]);
        }

        public string GetMonthNameFromInt(int Month)
        {
            string MonthName;
            switch (Month)
            {
                case 1:
                    MonthName = "Enero";
                    break;
                case 2:
                    MonthName = "Febrero";
                    break;
                case 3:
                    MonthName = "Marzo";
                    break;
                case 4:
                    MonthName = "Abril";
                    break;
                case 5:
                    MonthName = "Mayo";
                    break;
                case 6:
                    MonthName = "Junio";
                    break;
                case 7:
                    MonthName = "Julio";
                    break;
                case 8:
                    MonthName = "Agosto";
                    break;
                case 9:
                    MonthName = "Septiembre";
                    break;
                case 10:
                    MonthName = "Octubre";
                    break;
                case 11:
                    MonthName = "Noviembre";
                    break;
                case 12:
                    MonthName = "Diciembre";
                    break;
                default:
                    MonthName = "Error!";
                    break;
            }
            return MonthName;
        }

        private int CaseMonth(string Periodo)
        {
            int mes;
            switch (Periodo)
            {
                case "Enero":
                    mes = 1;
                    break;
                case "Febrero":
                    mes = 2;
                    break;
                case "Marzo":
                    mes = 3;
                    break;
                case "Abril":
                    mes = 4;
                    break;
                case "Mayo":
                    mes = 5;
                    break;
                case "Junio":
                    mes = 6;
                    break;
                case "Julio":
                    mes = 7;
                    break;
                case "Agosto":
                    mes = 8;
                    break;
                case "Septiembre":
                    mes = 9;
                    break;
                case "Octubre":
                    mes = 10;
                    break;
                case "Noviembre":
                    mes = 11;
                    break;
                case "Diciembre":
                    mes = 12;
                    break;
                case "January":
                    mes = 1;
                    break;
                case "February":
                    mes = 2;
                    break;
                case "March":
                    mes = 3;
                    break;
                case "April":
                    mes = 4;
                    break;
                case "May":
                    mes = 5;
                    break;
                case "June":
                    mes = 6;
                    break;
                case "July":
                    mes = 7;
                    break;
                case "August":
                    mes = 8;
                    break;
                case "September":
                    mes = 9;
                    break;
                case "October":
                    mes = 10;
                    break;
                case "November":
                    mes = 11;
                    break;
                case "December":
                    mes = 12;
                    break;
                default:
                    mes = 0;
                    break;
            }
            return mes;
        }

        public int GetAnno
        {
            get
            {
                return Anno;
            }
        }

        public int GetMes
        {
            get
            {
                return Mes;
            }
        }

        public string GetNombreMes
        {
            get
            {
                return NombreMes;
            }
        }
    }
}