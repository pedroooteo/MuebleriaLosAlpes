Imports Oracle.ManagedDataAccess.Client

Namespace Datos
    Public Class Conexion
        Public Shared Function ObtenerConexion() As OracleConnection
            Dim cadena = If(
                Environment.GetEnvironmentVariable("ORACLE_CONNECTION"),
                "Data Source=localhost:1521/xe;User Id=SYSTEM;Password=123456;"
            )
            Return New OracleConnection(cadena)
        End Function
    End Class
End Namespace