using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Mvc;
using OfficeOpenXml;
using ReporteCanalContacto.Models;


namespace ReporteCanalContacto.Controllers
{
    public class HomeController : Controller
    {
        public string ServerProd = ConfigurationManager.ConnectionStrings["CANALCONTACTOEntities"].ConnectionString;
        static FiltroBusqueda fechaReport = new FiltroBusqueda();
        List<Log> logs = new List<Log>();
        static List<Log> logsReporte;
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult GetList(FiltroBusqueda fecha = null)
        {
            return ObtenerTransacciones(fecha);
        }

        public ActionResult ObtenerTransacciones(FiltroBusqueda fecha = null)
        {
            string Consulta1 = "";
            fechaReport = new FiltroBusqueda();
            string dateDesde = fecha.fechaDesde.ToString("yyyyMMdd", new CultureInfo("en-US", true));
            string dateHasta = fecha.fechaHasta.ToString("yyyyMMdd", new CultureInfo("en-US", true));
            fechaReport = fecha;
            try
            {
                if (dateDesde != "00010101")
                {
                    using (SqlConnection connection = new SqlConnection(ServerProd))
                    {

                        if ((fecha.tipoCanal) == "NA" && (fecha.tipoConsulta) == "NA")
                        {
                            Consulta1 = "SELECT * " +
                           "FROM CANAL_CONTACTO WHERE (FECHA>=@desde AND  [FECHA] < dateadd(day, 1, @hasta) )  ORDER BY FECHA DESC";
                        }
                        else
                        {
                            if ((fecha.tipoCanal) != "NA" && (fecha.tipoConsulta) == "NA")
                            {
                                Consulta1 = "SELECT * " +
                                "FROM CANAL_CONTACTO  WHERE (FECHA>=@desde AND  [FECHA] < dateadd(day, 1, @hasta) and TIPO = '" + fecha.tipoCanal + "')   ORDER BY FECHA DESC";
                            }
                            else
                            {
                                if ((fecha.tipoCanal) == "NA" && (fecha.tipoConsulta) != "NA")
                                {
                                    Consulta1 = "SELECT * " +
                                    "FROM CANAL_CONTACTO WHERE (FECHA>=@desde AND  [FECHA] < dateadd(day, 1, @hasta) and CONSULTA = '" + fecha.tipoConsulta + "')   ORDER BY FECHA DESC";
                                }
                                else
                                {
                                    Consulta1 = "SELECT * " +
                                    "FROM CANAL_CONTACTO WHERE (FECHA>=@desde AND  [FECHA] < dateadd(day, 1, @hasta) and CONSULTA = '" + fecha.tipoConsulta + "'and TIPO = '" + fecha.tipoCanal + "')   ORDER BY FECHA DESC";
                                }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(Consulta1))
                        {
                            cmd.Connection = connection;
                            connection.Open();
                            cmd.Parameters.AddWithValue("desde", dateDesde);
                            cmd.Parameters.AddWithValue("hasta", dateHasta);

                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {

                                string fechaString;

                                while (sdr.Read())
                                {
                                    if (String.IsNullOrEmpty(sdr[4].ToString()))
                                    {
                                        fechaString = "2018-12-31 00:00:00.000";
                                    }
                                    else
                                    {
                                        fechaString = sdr[4].ToString();
                                    }

                                    var cont = 0;
                                    var posicion = 0;
                                    string[] transacciones = sdr[0].ToString().Split(',');

                                    foreach (var item in transacciones)
                                    {

                                        posicion++;
                                        if (item.Equals("INGRESO IDENTIFICACION"))
                                        {
                                            break;
                                        }
                                    }

                                    cont = (sdr[0].ToString().Split(',').Length - posicion);
                                    var contador = sdr[0].ToString().Split(',').Length;


                                    try
                                    {
                                        logs.Add(new Log
                                        {
                                            id = (Int32)sdr[0],
                                            cedula = sdr[1].ToString(),
                                            consulta = sdr[2].ToString(),
                                            tipo = sdr[3].ToString(),
                                            fecha = Convert.ToDateTime(fechaString)
                                            //totalAgendada=agendadas,
                                            //totalInconclusas= incompletas

                                        });

                                    }
                                    catch (Exception e)
                                    {
                                        ViewBag.Error = "Error: " + e.Message + e.Source;
                                        System.Diagnostics.Debug.WriteLine("Exception: " + e.Message);
                                    }
                                }
                            }
                            connection.Dispose();
                            connection.Close();
                        }
                        logsReporte = new List<Log>();
                        logsReporte.Clear();
                        logsReporte = logs;
                        return new JsonResult() { Data = logs, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
                    }
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(ServerProd))
                    {
                        DateTime fechaActual = DateTime.Now;
                        string dateActualDesde = fechaActual.ToString("yyyy-MM-dd", new CultureInfo("en-US", true));
                        //Consulta1 = "SELECT * FROM CANAL_CONTACTO  ORDER BY FECHA DESC";

                        Consulta1 = "SELECT * " +
                           "FROM CANAL_CONTACTO WHERE (([FECHA]>='" + dateActualDesde + "' AND  [FECHA] < dateadd(day, 1,'" + dateActualDesde + "') ) )  ORDER BY FECHA DESC";

                        using (SqlCommand cmd = new SqlCommand(Consulta1))
                        {
                            cmd.Connection = connection;
                            connection.Open();


                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {

                                string fechaString;
                                //string agendadas = GetTotalAgendada(fecha);
                                //string incompletas = GetCallsInconclusas(fecha);
                                while (sdr.Read())
                                {
                                    if (String.IsNullOrEmpty(sdr[4].ToString()))
                                    {
                                        fechaString = "2018-12-31 00:00:00.000";
                                    }
                                    else
                                    {
                                        fechaString = sdr[4].ToString();
                                    }


                                    try
                                    {
                                        logs.Add(new Log
                                        {
                                            id = (Int32)sdr[0],
                                            cedula = sdr[1].ToString(),
                                            consulta = sdr[2].ToString(),
                                            tipo = sdr[3].ToString(),
                                            fecha = Convert.ToDateTime(fechaString)

                                        });

                                    }
                                    catch (Exception e)
                                    {
                                        ViewBag.Error = "Error: " + e.Message + e.Source;
                                        System.Diagnostics.Debug.WriteLine("Exception: " + e.Message);
                                    }
                                }
                            }
                            connection.Dispose();
                            connection.Close();
                        }

                        logsReporte = new List<Log>();
                        logsReporte.Clear();
                        logsReporte = logs;

                        return new JsonResult() { Data = logs, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };

                    }
                }

            }
            catch (Exception e)
            {
                ViewBag.Error = "Error: " + e.Message + e.Source;
                System.Diagnostics.Debug.WriteLine("Exception: " + e.Message);
                return new JsonResult() { Data = logs, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
        }

        public void ReporteCanalContacto()
        {

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reporte Canal Contacto");
            var date = new DateTime(0001, 01, 01, 0, 0, 0);

            System.Diagnostics.Debug.WriteLine(date);

            ws.Cells["A1"].Value = "Reporte";
            ws.Cells["B1"].Value = "Reporte Canales de Contacto";
            ws.Cells["A2"].Value = "Fecha";
            ws.Cells["B2"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);
            ws.Cells["A1:A4"].Style.Font.Bold = true;


            if (fechaReport.fechaDesde == date)
            {
                System.Diagnostics.Debug.WriteLine(date);
                ws.Cells["A3"].Value = "Filtro Búsqueda Fecha Desde";
                ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy}", DateTimeOffset.Now);
                ws.Cells["A4"].Value = "Filtro Búsqueda Fecha Hasta";
                ws.Cells["B4"].Value = string.Format("{0:dd MMMM yyyy}", DateTimeOffset.Now);
            }
            else
            {
                ws.Cells["A3"].Value = "Filtro Búsqueda Fecha Desde";
                ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy}", fechaReport.fechaDesde);
                ws.Cells["A4"].Value = "Filtro Búsqueda Fecha Hasta";
                ws.Cells["B4"].Value = string.Format("{0:dd MMMM yyyy}", fechaReport.fechaHasta);
            }


            ws.Cells["A6"].Value = "CEDULA";
            ws.Cells["B6"].Value = "CONSULTA";
            ws.Cells["C6"].Value = "TIPO";
            ws.Cells["D6"].Value = "FECHA CREACIÓN";

            ws.Cells["A6:D6"].Style.Font.Bold = true;
            int rowStart = 7;


            foreach (var item in logsReporte)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.cedula;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.consulta;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.tipo;
                ws.Cells[string.Format("D{0}", rowStart)].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", item.fecha);

                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }

        public JsonResult AjaxMethod2(FiltroBusqueda filtroFecha = null)
        {
            string query = "";
            string dateDesde = filtroFecha.fechaDesde.ToString("yyyy-MM-dd", new CultureInfo("en-US", true));
            string dateHasta = filtroFecha.fechaHasta.ToString("yyyy-MM-dd", new CultureInfo("en-US", true));

            if ((filtroFecha.tipoCanal == "NA") && (filtroFecha.tipoConsulta == "NA") && (dateDesde == "0001-01-01"))
            {
                DateTime fechaActual = DateTime.Now;
                string dateActualDesde = fechaActual.ToString("yyyy-MM-dd", new CultureInfo("en-US", true));

                query = "SELECT [TIPO] , COUNT([TIPO])as CANTIDAD FROM CANAL_CONTACTO WHERE (([FECHA]>='" + dateActualDesde + "' AND  [FECHA] < dateadd(day, 1,'" + dateActualDesde + "') ) )group by [TIPO]";
            }
            else
            {
                if ((filtroFecha.tipoCanal) != "NA" && (filtroFecha.tipoConsulta) == "NA")
                {
                    query = "SELECT [TIPO] , COUNT([TIPO])as CANTIDAD FROM CANAL_CONTACTO WHERE (([FECHA]>='" + dateDesde + "' AND  [FECHA] < dateadd(day, 1,'" + dateHasta + "') ) ) and TIPO = '" + filtroFecha.tipoCanal + "' group by [TIPO]";

                }
                else
                {
                    if ((filtroFecha.tipoCanal) == "NA" && (filtroFecha.tipoConsulta) != "NA")
                    {
                        query = "SELECT [TIPO] , COUNT([TIPO])as CANTIDAD FROM CANAL_CONTACTO WHERE (([FECHA]>='" + dateDesde + "' AND  [FECHA] < dateadd(day, 1,'" + dateHasta + "') ) ) and CONSULTA = '" + filtroFecha.tipoConsulta + "' group by [TIPO]";
                    }
                    else
                    {
                        if ((filtroFecha.tipoCanal) == "NA" && (filtroFecha.tipoConsulta) == "NA")
                        {
                            query = "SELECT [TIPO] , COUNT([TIPO])as CANTIDAD FROM CANAL_CONTACTO WHERE (([FECHA]>='" + dateDesde + "' AND  [FECHA] < dateadd(day, 1,'" + dateHasta + "') ) ) group by [TIPO]";
                        }
                        else
                        {

                            query = "SELECT [TIPO] , COUNT([TIPO])as CANTIDAD FROM CANAL_CONTACTO WHERE (([FECHA]>='" + dateDesde + "' AND  [FECHA] < dateadd(day, 1,'" + dateHasta + "') ) ) and TIPO = '" + filtroFecha.tipoCanal + "' and CONSULTA = '" + filtroFecha.tipoConsulta + "' group by [TIPO]";
                        }
                    }
                }
            }

            string constr = ConfigurationManager.ConnectionStrings["CANALCONTACTOEntities"].ConnectionString;
            List<object> chartData = new List<object>();
            chartData.Add(new object[]
                            {
                            "TIPO", "CANTIDAD"
                            });
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            chartData.Add(new object[]
                            {
                            sdr["TIPO"], sdr["CANTIDAD"]
                            });
                        }
                    }

                    con.Close();
                }
            }

            return Json(chartData);
        }
    }
}