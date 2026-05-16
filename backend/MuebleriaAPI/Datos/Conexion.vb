Imports Oracle.ManagedDataAccess.Client

Namespace Datos
    Public Class Conexion
        ' La cadena de conexión a Oracle
        Private Shared ReadOnly cadenaConexion As String = "Data Source=localhost:1521/xe;User Id=SYSTEM;Password=123456;"

        Public Shared Function ObtenerConexion() As OracleConnection
            Return New OracleConnection(cadenaConexion)
        End Function
    End Class
End Namespace