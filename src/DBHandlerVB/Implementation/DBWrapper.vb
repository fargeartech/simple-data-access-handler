Imports System.Runtime.InteropServices
Imports DBHandlerVB.DBHandler.DbProvider
Imports DBHandlerVB.DBHandlerVB.Interfaces

Namespace DBHandlerVB.Implementation
    Public Class DBWrapper
        Implements IDisposable

        Private dbFactory As DBWrapperFactory
        Private database As IDBWrapper
        Private providerName As String
        Private parameters As List(Of IDbDataParameter)

        Public Sub New(ByVal connectionStringName As String)
            dbFactory = New DBWrapperFactory(connectionStringName)
            database = dbFactory.CreateDatabase()
            providerName = dbFactory.GetProviderName()
            parameters = New List(Of IDbDataParameter)()
        End Sub

        Public Sub BeginTransaction()
            Try
                database.Begin()
            Catch ex As Exception
                Throw New ApplicationException("error " & ex.Message)
            End Try
        End Sub

        Public Sub Commit()
            Try
                database.Commit()
            Catch ex As Exception
                Throw New Exception("Error " & ex.Message)
            End Try
        End Sub

        Public Sub Rollback()
            Try
                database.Rollback()
            Catch ex As Exception
                Throw New Exception("error " & ex.Message)
            End Try
        End Sub

        Private Function GetDatabaseConnection() As IDbConnection
            Return database.CreateConnection()
        End Function

        Public Sub CreateParameter(ByVal name As String, ByVal value As Object, ByVal dbType As DbType)
            parameters.Add(ParameterManager.CreateParameter(providerName, name, value, dbType, ParameterDirection.Input))
        End Sub

        Public Sub CreateParameter(ByVal name As String, ByVal size As Integer, ByVal value As Object, ByVal dbType As DbType)
            parameters.Add(ParameterManager.CreateParameter(providerName, name, size, value, dbType, ParameterDirection.Input))
        End Sub

        Public Sub CreateParameter(ByVal name As String, ByVal size As Integer, ByVal value As Object, ByVal dbType As DbType, ByVal direction As ParameterDirection)
            parameters.Add(ParameterManager.CreateParameter(providerName, name, size, value, dbType, direction))
        End Sub

        Public Sub ClearParameters()
            parameters.Clear()
        End Sub

        Public Function GetDataTable(ByVal commandText As String, ByVal commandType As CommandType) As DataTable
            Using connection = GetDatabaseConnection()
                Using command = database.CreateCommand(commandText, commandType, connection)
                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Dim dataset = New DataSet()
                    Dim dataAdaper = database.CreateAdapter()
                    dataAdaper.Fill(dataset)
                    Return dataset.Tables(0)
                End Using
            End Using
        End Function

        Public Function GetDataSet(ByVal commandText As String, ByVal commandType As CommandType) As DataSet
            Using connection = GetDatabaseConnection()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Dim dataset = New DataSet()
                    Dim dataAdaper = database.CreateAdapter()
                    dataAdaper.Fill(dataset)
                    Return dataset
                End Using
            End Using
        End Function

        Public Function GetDataReader(ByVal commandText As String, ByVal commandType As CommandType) As IDataReader
            Dim reader As IDataReader = Nothing
            Dim connection = GetDatabaseConnection()
            Dim command = database.CreateCommand(commandText, commandType, connection)

            If parameters IsNot Nothing Then
                Parallel.ForEach(parameters, Sub(parameter)
                                                 command.Parameters.Add(parameter)
                                             End Sub)
            End If

            reader = command.ExecuteReader()
            Return reader
        End Function

        Public Sub Delete(ByVal commandText As String, ByVal commandType As CommandType)
            Using connection = GetDatabaseConnection()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub Insert(ByVal commandText As String, ByVal commandType As CommandType)
            Using connection = GetDatabaseConnection()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function Insert(ByVal commandText As String, ByVal commandType As CommandType, <Out> ByRef lastId As Integer) As Integer
            lastId = 0

            Using connection = GetDatabaseConnection()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Dim newId As Object = command.ExecuteScalar()
                    lastId = Convert.ToInt32(newId)
                End Using
            End Using

            Return lastId
        End Function

        Public Function Insert(ByVal commandText As String, ByVal commandType As CommandType, <Out> ByRef lastId As Long) As Long
            lastId = 0

            Using connection = GetDatabaseConnection()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Dim newId As Object = command.ExecuteScalar()
                    lastId = Convert.ToInt64(newId)
                End Using
            End Using

            Return lastId
        End Function

        Public Sub InsertWithTransaction(ByVal commandText As String, ByVal commandType As CommandType)
            Dim transactionScope As IDbTransaction = Nothing

            Using connection = GetDatabaseConnection()
                transactionScope = connection.BeginTransaction()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Try
                        command.ExecuteNonQuery()
                        transactionScope.Commit()
                    Catch __unusedException1__ As Exception
                        transactionScope.Rollback()
                    Finally
                        connection.Dispose()
                    End Try
                End Using
            End Using
        End Sub

        Public Sub InsertWithTransaction(ByVal commandText As String, ByVal commandType As CommandType, ByVal isolationLevel As IsolationLevel)
            Dim transactionScope As IDbTransaction = Nothing

            Using connection = GetDatabaseConnection()
                transactionScope = connection.BeginTransaction(isolationLevel)

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Try
                        command.ExecuteNonQuery()
                        transactionScope.Commit()
                    Catch __unusedException1__ As Exception
                        transactionScope.Rollback()
                    Finally
                        connection.Close()
                    End Try
                End Using
            End Using
        End Sub

        Public Sub Update(ByVal commandText As String, ByVal commandType As CommandType)
            Using connection = GetDatabaseConnection()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub UpdateWithTransaction(ByVal commandText As String, ByVal commandType As CommandType)
            Dim transactionScope As IDbTransaction = Nothing

            Using connection = GetDatabaseConnection()
                transactionScope = connection.BeginTransaction()

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Try
                        command.ExecuteNonQuery()
                        transactionScope.Commit()
                    Catch __unusedException1__ As Exception
                        transactionScope.Rollback()
                    Finally
                        connection.Close()
                    End Try
                End Using
            End Using
        End Sub

        Public Sub UpdateWithTransaction(ByVal commandText As String, ByVal commandType As CommandType, ByVal isolationLevel As IsolationLevel)
            Dim transactionScope As IDbTransaction = Nothing

            Using connection = GetDatabaseConnection()
                transactionScope = connection.BeginTransaction(isolationLevel)

                Using command = database.CreateCommand(commandText, commandType, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Try
                        command.ExecuteNonQuery()
                        transactionScope.Commit()
                    Catch __unusedException1__ As Exception
                        transactionScope.Rollback()
                    Finally
                        connection.Close()
                    End Try
                End Using
            End Using
        End Sub

        Public Function GetScalarValue(Of T)(ByVal commandText As String, ByVal commandType As CommandType) As T
            Dim connection = GetDatabaseConnection()

            Using command = database.CreateCommand(commandText, commandType, connection)

                If parameters.Count > 0 Then
                    Parallel.ForEach(parameters, Function(parameter)
                                                     command.Parameters.Add(parameter)
                                                 End Function)
                End If

                Dim result = command.ExecuteScalar()
                If Convert.IsDBNull(result) Then Return Nothing
                If TypeOf result Is T Then
                    Return CType(result, T)
                Else
                    Return CType(Convert.ChangeType(result, GetType(T)), T)
                End If
            End Using
        End Function

        Public Function ExecuteDatatableSQL(ByVal sql As String) As DataTable
            Using connection = GetDatabaseConnection()

                Using command = database.CreateCommand(sql, CommandType.Text, connection)

                    If parameters IsNot Nothing Then
                        Parallel.ForEach(parameters, Sub(parameter)
                                                         command.Parameters.Add(parameter)
                                                     End Sub)
                    End If

                    Dim dataset = New DataSet()
                    Dim dataAdaper = database.CreateAdapter()
                    dataAdaper.Fill(dataset)
                    Return dataset.Tables(0)
                End Using
            End Using
        End Function

        Public Function ExecuteDataReaderSQL(ByVal commandText As String) As IDataReader
            Dim reader As IDataReader = Nothing
            Dim connection = GetDatabaseConnection()
            Dim command = database.CreateCommand(commandText, CommandType.Text, connection)

            If parameters IsNot Nothing Then
                Parallel.ForEach(parameters, Sub(parameter)
                                                 command.Parameters.Add(parameter)
                                             End Sub)
            End If

            reader = command.ExecuteReader()
            Return reader
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            database.Dispose()
            dbFactory = Nothing
            database = Nothing
            parameters = Nothing
        End Sub
    End Class
End Namespace
