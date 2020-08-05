Imports System.Data.SqlClient
Imports DBHandlerVB.DBHandlerVB.Interfaces

Namespace DBHandlerVB.DbProvider
    Public Class MsSqlDataProvider
        Implements IDBWrapper
        Private Property _ConnectionString As String
        Private _disposed As Boolean
        Private _dbConnection As IDbConnection
        Private _dbTransaction As IDbTransaction
        Private sqlcmd As SqlCommand

        Public ReadOnly Property CurrentConnectionState As ConnectionState Implements IDBWrapper.CurrentConnectionState
            Get
                If _dbConnection Is Nothing Then
                    Return ConnectionState.Closed
                End If
                Return _dbConnection.State
            End Get
        End Property

        Public Sub CloseConnection() Implements IDBWrapper.CloseConnection
            _dbConnection.Close()
            _dbConnection.Dispose()
        End Sub

        Public Sub New(ByVal connectionString As String)
            _ConnectionString = connectionString
            _dbConnection = Nothing
            _dbTransaction = Nothing
            sqlcmd = New SqlCommand()
        End Sub

        Public Function CreateConnection() As IDbConnection Implements IDBWrapper.CreateConnection
            If _dbConnection Is Nothing OrElse (CurrentConnectionState = ConnectionState.Broken OrElse CurrentConnectionState = ConnectionState.Closed) Then
                _dbConnection = New SqlConnection(_ConnectionString)
                _dbConnection.Open()
                Return _dbConnection
            End If

            Return _dbConnection
        End Function

        Public Function CreateCommand(ByVal commandText As String, ByVal commandType As CommandType, ByVal connection As IDbConnection) As IDbCommand Implements IDBWrapper.CreateCommand
            sqlcmd.CommandText = commandText
            sqlcmd.Connection = CType(connection, SqlConnection)
            sqlcmd.CommandType = commandType
            Return sqlcmd
        End Function

        Public Function CreateAdapter() As IDataAdapter Implements IDBWrapper.CreateAdapter
            Return New SqlDataAdapter(sqlcmd)
        End Function

        Public Function CreateParameter() As IDbDataParameter Implements IDBWrapper.CreateParameter
            Return sqlcmd.CreateParameter()
        End Function

        Public Function Begin() As IDbTransaction Implements IDBWrapper.Begin
            _dbTransaction = CreateConnection().BeginTransaction()
            sqlcmd.Transaction = CType(_dbTransaction, SqlTransaction)
            Return _dbTransaction
        End Function

        Public Sub Commit() Implements IDBWrapper.Commit
            If _dbTransaction IsNot Nothing Then
                _dbTransaction.Commit()
                sqlcmd.Parameters.Clear()
                _dbTransaction.Dispose()
            Else
                Throw New ApplicationException("Transaction Not Even Open.")
            End If
        End Sub

        Public Sub Rollback() Implements IDBWrapper.Rollback
            If _dbTransaction IsNot Nothing Then
                _dbTransaction.Rollback()
                sqlcmd.Parameters.Clear()
                _dbTransaction.Dispose()
            Else
                Throw New ApplicationException("Transaction Not Even Open.")
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Private Sub dispose(ByVal disposing As Boolean)
            If Not _disposed Then
                If disposing Then
                    If _dbConnection IsNot Nothing AndAlso CurrentConnectionState = ConnectionState.Open Then CloseConnection()
                    sqlcmd.Dispose()
                    _dbTransaction = Nothing
                    _dbConnection = Nothing
                    sqlcmd = Nothing
                End If
                _disposed = True
            End If
        End Sub

        Protected Overrides Sub Finalize()
            dispose(False)
        End Sub
    End Class
End Namespace
