Imports MuebleriaAPI.Datos
Imports System.Collections.Generic

Namespace Negocio
    Public Class ReporteNegocio
        Private ReadOnly _datos As New ReporteDatos()

        Public Function VentasDiarias(fechaIni As Date, fechaFin As Date, idCiudad As Integer) As List(Of Dictionary(Of String, Object))
            Return _datos.VentasDiarias(fechaIni, fechaFin, idCiudad)
        End Function

        Public Function ProductoTop(fechaIni As Date, fechaFin As Date, idCiudad As Integer) As List(Of Dictionary(Of String, Object))
            Return _datos.ProductoTop(fechaIni, fechaFin, idCiudad)
        End Function

        Public Function HistorialClienteAdmin(idCliente As Integer) As List(Of Dictionary(Of String, Object))
            Return _datos.HistorialClienteAdmin(idCliente)
        End Function

        Public Function CierreCajas(fechaIni As Date, fechaFin As Date) As List(Of Dictionary(Of String, Object))
            Return _datos.CierreCajas(fechaIni, fechaFin)
        End Function

        Public Function ReporteLTV() As List(Of Dictionary(Of String, Object))
            Return _datos.ReporteLTV()
        End Function

        Public Function ReporteActividad(fechaIni As Date, fechaFin As Date) As List(Of Dictionary(Of String, Object))
            Return _datos.ReporteActividad(fechaIni, fechaFin)
        End Function

        Public Function ReporteRetencion() As List(Of Dictionary(Of String, Object))
            Return _datos.ReporteRetencion()
        End Function

        Public Function ReporteCohorte() As List(Of Dictionary(Of String, Object))
            Return _datos.ReporteCohorte()
        End Function

        Public Function ReporteRemarketing() As List(Of Dictionary(Of String, Object))
            Return _datos.ReporteRemarketing()
        End Function
    End Class
End Namespace
