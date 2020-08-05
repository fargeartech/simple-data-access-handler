Namespace DBHandlerVB.Interfaces
    Public Interface IDBWrapper
        Inherits IDisposable
        Function CreateConnection() As IDbConnection
        Sub CloseConnection()
        Function CreateCommand(ByVal commandText As String, ByVal commandType As CommandType, ByVal connection As IDbConnection) As IDbCommand
        Function CreateAdapter() As IDataAdapter
        Function CreateParameter() As IDbDataParameter
        ReadOnly Property CurrentConnectionState As ConnectionState
        Function Begin() As IDbTransaction
        Sub Commit()
        Sub Rollback()
    End Interface
End Namespace