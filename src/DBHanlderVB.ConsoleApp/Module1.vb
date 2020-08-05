Imports DBHandlerVB.DBHandlerVB.Implementation

Module Module1

    Sub Main()
        Using dal As New DBWrapper("DBConnection")
            dal.CreateParameter("@is_deleted", 1, DbType.Int32)
            Using reader As IDataReader = dal.ExecuteDataReaderSQL("select top 10 * from asset_main where is_deleted = @is_deleted")
                While reader.Read
                    Console.WriteLine(String.Format("no chasis : {0}", reader("serial_chasis_no").ToString()))
                End While
            End Using
            dal.ClearParameters()
            Dim count As Integer = dal.GetScalarValue(Of Integer)("select count(asset_id) from asset_main where is_deleted = 1 ", CommandType.Text)
            Console.WriteLine("total " + count.ToString())
        End Using
    End Sub

End Module
