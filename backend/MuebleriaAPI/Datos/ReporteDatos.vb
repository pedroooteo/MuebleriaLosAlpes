Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.Collections.Generic

Namespace Datos
    Public Class ReporteDatos

        Private Function EjecutarReporte(nombreProc As String, parametros As List(Of OracleParameter)) As List(Of Dictionary(Of String, Object))
            Dim resultado As New List(Of Dictionary(Of String, Object))
            Using conn = ConexionReplica.ObtenerConexion()
                conn.Open()
                Using cmd As New OracleCommand(nombreProc, conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    For Each p In parametros
                        cmd.Parameters.Add(p)
                    Next
                    Dim pCursor As New OracleParameter("p_cursor", OracleDbType.RefCursor)
                    pCursor.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(pCursor)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim fila As New Dictionary(Of String, Object)
                            For i = 0 To reader.FieldCount - 1
                                fila(reader.GetName(i)) = If(IsDBNull(reader(i)), Nothing, reader(i))
                            Next
                            resultado.Add(fila)
                        End While
                    End Using
                End Using
            End Using
            Return resultado
        End Function

        Public Function VentasDiarias(fechaIni As Date, fechaFin As Date, idCiudad As Integer) As List(Of Dictionary(Of String, Object))
            Dim params As New List(Of OracleParameter) From {
                New OracleParameter("p_fecha_ini", OracleDbType.Date) With {.Value = fechaIni},
                New OracleParameter("p_fecha_fin", OracleDbType.Date) With {.Value = fechaFin},
                New OracleParameter("p_id_ciudad", OracleDbType.Int32) With {
                    .Value = If(idCiudad = 0, DBNull.Value, CObj(idCiudad))}
            }
            Return EjecutarReporte("PKG_REPORTES.PR_VENTAS_DIARIAS", params)
        End Function

        Public Function ProductoTop(fechaIni As Date, fechaFin As Date, idCiudad As Integer) As List(Of Dictionary(Of String, Object))
            Dim params As New List(Of OracleParameter) From {
                New OracleParameter("p_fecha_ini", OracleDbType.Date) With {.Value = fechaIni},
                New OracleParameter("p_fecha_fin", OracleDbType.Date) With {.Value = fechaFin},
                New OracleParameter("p_id_ciudad", OracleDbType.Int32) With {
                    .Value = If(idCiudad = 0, DBNull.Value, CObj(idCiudad))}
            }
            Return EjecutarReporte("PKG_REPORTES.PR_PRODUCTO_TOP", params)
        End Function

        Public Function HistorialClienteAdmin(idCliente As Integer) As List(Of Dictionary(Of String, Object))
            Dim params As New List(Of OracleParameter) From {
                New OracleParameter("p_id_cliente", OracleDbType.Int32) With {.Value = idCliente}
            }
            Return EjecutarReporte("PKG_REPORTES.PR_HISTORIAL_CLIENTE_ADMIN", params)
        End Function

        Public Function CierreCajas(fechaIni As Date, fechaFin As Date) As List(Of Dictionary(Of String, Object))
            Dim params As New List(Of OracleParameter) From {
                New OracleParameter("p_fecha_ini", OracleDbType.Date) With {.Value = fechaIni},
                New OracleParameter("p_fecha_fin", OracleDbType.Date) With {.Value = fechaFin}
            }
            Return EjecutarReporte("PKG_REPORTES.PR_CIERRE_CAJAS", params)
        End Function

        Public Function ReporteLTV() As List(Of Dictionary(Of String, Object))
            Return EjecutarReporte("PKG_REPORTES.PR_REPORTE_LTV", New List(Of OracleParameter))
        End Function

        Public Function ReporteActividad(fechaIni As Date, fechaFin As Date) As List(Of Dictionary(Of String, Object))
            Dim params As New List(Of OracleParameter) From {
                New OracleParameter("p_fecha_ini", OracleDbType.Date) With {.Value = fechaIni},
                New OracleParameter("p_fecha_fin", OracleDbType.Date) With {.Value = fechaFin}
            }
            Return EjecutarReporte("PKG_REPORTES.PR_REPORTE_ACTIVIDAD", params)
        End Function

        Public Function ReporteRetencion() As List(Of Dictionary(Of String, Object))
            Return EjecutarReporte("PKG_REPORTES.PR_REPORTE_RETENCION", New List(Of OracleParameter))
        End Function

        Public Function ReporteCohorte() As List(Of Dictionary(Of String, Object))
            Return EjecutarReporte("PKG_REPORTES.PR_REPORTE_COHORTE", New List(Of OracleParameter))
        End Function

        Public Function ReporteRemarketing() As List(Of Dictionary(Of String, Object))
            Return EjecutarReporte("PKG_REPORTES.PR_REPORTE_REMARKETING", New List(Of OracleParameter))
        End Function

    End Class
End Namespace
