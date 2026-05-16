Imports Oracle.ManagedDataAccess.Client

Namespace Datos
    Public Class ConexionReplica
        ' PRODUCCIÓN: apunta a la BD Standby (DataGuard Replica)
        ' En desarrollo puede apuntar a la misma BD
        Private Const CADENA_REPLICA As String =
            "Data Source=localhost:1521/xe;User Id=SYSTEM;Password=123456;"

        Public Shared Function ObtenerConexion() As OracleConnection
            Return New OracleConnection(CADENA_REPLICA)
        End Function
    End Class
End Namespace
