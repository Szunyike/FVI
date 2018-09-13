Imports System.Windows.Forms

''' <summary>
''' Wraps an OpenFileDialog to show a Vista-style Folder Dialog
''' This class based on the work of 'Bill' and posted on https://www.lyquidity.com/devblog/?p=136
''' We've re-written it for Visual Basic.
''' </summary>
Public Class FolderSelectDialog
    Implements IDisposable

    ' Wrapped dialog
    Private OFD As System.Windows.Forms.OpenFileDialog = Nothing

    ''' <summary>
    ''' Initialize our Open File Dialog object
    ''' </summary>
    Public Sub New()
        OFD = New System.Windows.Forms.OpenFileDialog()

        With OFD
            .Filter = "Folders|" & vbLf
            .AddExtension = False
            .CheckFileExists = False
            .DereferenceLinks = True
            .Multiselect = True
        End With
    End Sub

#Region "Properties"

    ''' <summary>
    ''' Gets/Sets the initial folder to be selected. A value of Nothing or Emplty selects the current directory.
    ''' </summary>
    Public Property InitialDirectory() As String
        Get
            Return OFD.InitialDirectory
        End Get
        Set(value As String)
            OFD.InitialDirectory = IIf(value Is Nothing OrElse value.Length = 0, Environment.CurrentDirectory, value)
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the title to show in the dialog. A value of Nothing or Empty defaults to 'Select a folder'
    ''' </summary>
    Public Property Title() As String
        Get
            Return OFD.Title
        End Get
        Set(value As String)
            OFD.Title = If(value Is Nothing, "Select a folder", value)
        End Set
    End Property

    ''' <summary>
    ''' Gets the selected folder
    ''' </summary>
    Public ReadOnly Property FolderName() As String
        Get
            Return OFD.FileName
        End Get
    End Property
    ''' <summary>
    ''' Gets the selected folder
    ''' </summary>
    Public Property FolderNames() As String()
        Get
            Return OFD.FileNames
        End Get
        Set(value As String())

        End Set
    End Property

#End Region

#Region "Methods"

    ''' <summary>
    ''' Shows the dialog
    ''' </summary>
    ''' <returns>True if folder selected, False if Cancel clicked</returns>
    Public Function ShowDialog() As DialogResult
        Return ShowDialog(IntPtr.Zero)
    End Function

    ''' <summary>
    ''' Shows the dialog. Returns True of folder selected, False if Cancel clicked.
    ''' </summary>
    ''' <param name="WinForm">Parent form</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ShowDialog(WinForm As Form) As DialogResult
        Return ShowDialog(WinForm.Handle)
    End Function

    ''' <summary>
    ''' Shows the dialog. Returns True of folder selected, False if Cancel clicked.
    ''' </summary>
    ''' <param name="hWndOwner">Handle of the Parent Form</param>
    ''' <returns>True if folder selected, False if Cancel clicked</returns>
    Public Function ShowDialog(hWndOwner As IntPtr) As DialogResult
        Dim flag As Boolean = False

        'See if our Windows version is post-Vista
        If Environment.OSVersion.Version.Major >= 6 Then
            Dim frm = New Reflector("System.Windows.Forms")

            Dim num As UInteger = 0
            Dim folderDialog As Type = frm.myGetType("FileDialogNative.IFileDialog")
            Dim dialog As Object = frm.[Call](OFD, "CreateVistaDialog")
            frm.Call(OFD, "OnBeforeVistaDialog", dialog)

            Dim options As UInteger = CUInt(frm.CallAs(GetType(System.Windows.Forms.FileDialog), OFD, "GetOptions"))
            options = options Or CUInt(frm.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS"))
            frm.CallAs(folderDialog, dialog, "SetOptions", options)

            Dim pfde As Object = frm.[New]("FileDialog.VistaDialogEvents", OFD)
            Dim parameters As Object() = New Object() {pfde, num}
            frm.CallAs2(folderDialog, dialog, "Advise", parameters)
            num = CUInt(parameters(1))
            Try
                Dim num2 As Integer = CInt(frm.CallAs(folderDialog, dialog, "Show", hWndOwner))
                flag = (0 = num2)
            Finally
                frm.CallAs(folderDialog, dialog, "Unadvise", num)
                GC.KeepAlive(pfde)
            End Try
        Else
            Dim fbd = New FolderBrowserDialog()
            fbd.Description = Me.Title
            fbd.SelectedPath = Me.InitialDirectory
            fbd.ShowNewFolderButton = False
            If fbd.ShowDialog(New WindowWrapper(hWndOwner)) <> DialogResult.OK Then
                Return DialogResult.Cancel
            End If
            OFD.FileName = fbd.SelectedPath
            flag = True
        End If

        If flag = True Then
            Return DialogResult.OK
        Else
            Return DialogResult.Cancel
        End If
    End Function

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        ofd.Dispose()
        Me.disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

''' <summary>
''' Creates IWin32Window around a Windows Form Handle
''' </summary>
Public Class WindowWrapper
    Implements System.Windows.Forms.IWin32Window
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="handle">Handle to wrap</param>
    Public Sub New(handle As IntPtr)
        _hwnd = handle
    End Sub

    ''' <summary>
    ''' Windows Form Handle
    ''' </summary>
    Public ReadOnly Property Handle() As IntPtr Implements IWin32Window.Handle
        Get
            Return _hwnd
        End Get
    End Property

    Private _hwnd As IntPtr
End Class

''' <summary>
''' This class is from the Front-End for Dosbox and is used to present a 'vista' dialog box to select folders.
''' Being able to use a vista style dialog box to select folders is much better then using the shell folder browser.
''' http://code.google.com/p/fed/
'''
''' Example:
''' var r = new Reflector("System.Windows.Forms");
''' </summary>
Public Class Reflector
#Region "variables"

    Private m_ns As String
    Private m_asmb As Reflection.Assembly

#End Region

#Region "Constructors"

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="ns">The namespace containing types to be used</param>
    Public Sub New(ns As String)
        Me.New(ns, ns)
    End Sub

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="an">A specific assembly name (used if the assembly name does not tie exactly with the namespace)</param>
    ''' <param name="ns">The namespace containing types to be used</param>
    Public Sub New(an As String, ns As String)
        m_ns = ns
        m_asmb = Nothing
        For Each an2 As Reflection.AssemblyName In Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies()
            If an2.FullName.StartsWith(an) Then
                m_asmb = Reflection.Assembly.Load(an2)
                Exit For
            End If
        Next
    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Return a Type instance for a type 'typeName'
    ''' </summary>
    ''' <param name="typeName">The name of the type</param>
    ''' <returns>A type instance</returns>
    Public Function MyGetType(typeName As String) As Type
        Dim type As Type = Nothing
        Dim names As String() = typeName.Split("."c)

        If names.Length > 0 Then
            type = m_asmb.[GetType]((m_ns & Convert.ToString(".")) + names(0))
        End If

        For i As Integer = 1 To names.Length - 1
            type = type.GetNestedType(names(i), Reflection.BindingFlags.NonPublic)
        Next
        Return type
    End Function

    ''' <summary>
    ''' Create a new object of a named type passing along any params
    ''' </summary>
    ''' <param name="name">The name of the type to create</param>
    ''' <param name="parameters"></param>
    ''' <returns>An instantiated type</returns>
    Public Function [New](name As String, ParamArray parameters As Object()) As Object
        Dim type As Type = MyGetType(name)

        Dim ctorInfos As Reflection.ConstructorInfo() = type.GetConstructors()
        For Each ci As Reflection.ConstructorInfo In ctorInfos
            Try
                Return ci.Invoke(parameters)
            Catch
            End Try
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' Calls method 'func' on object 'obj' passing parameters 'parameters'
    ''' </summary>
    ''' <param name="obj">The object on which to excute function 'func'</param>
    ''' <param name="func">The function to execute</param>
    ''' <param name="parameters">The parameters to pass to function 'func'</param>
    ''' <returns>The result of the function invocation</returns>
    Public Function [Call](obj As Object, func As String, ParamArray parameters As Object()) As Object
        Return Call2(obj, func, parameters)
    End Function

    ''' <summary>
    ''' Calls method 'func' on object 'obj' passing parameters 'parameters'
    ''' </summary>
    ''' <param name="obj">The object on which to excute function 'func'</param>
    ''' <param name="func">The function to execute</param>
    ''' <param name="parameters">The parameters to pass to function 'func'</param>
    ''' <returns>The result of the function invocation</returns>
    Public Function Call2(obj As Object, func As String, parameters As Object()) As Object
        Return CallAs2(obj.[GetType](), obj, func, parameters)
    End Function

    ''' <summary>
    ''' Calls method 'func' on object 'obj' which is of type 'type' passing parameters 'parameters'
    ''' </summary>
    ''' <param name="type">The type of 'obj'</param>
    ''' <param name="obj">The object on which to excute function 'func'</param>
    ''' <param name="func">The function to execute</param>
    ''' <param name="parameters">The parameters to pass to function 'func'</param>
    ''' <returns>The result of the function invocation</returns>
    Public Function CallAs(type As Type, obj As Object, func As String, ParamArray parameters As Object()) As Object
        Return CallAs2(type, obj, func, parameters)
    End Function

    ''' <summary>
    ''' Calls method 'func' on object 'obj' which is of type 'type' passing parameters 'parameters'
    ''' </summary>
    ''' <param name="type">The type of 'obj'</param>
    ''' <param name="obj">The object on which to excute function 'func'</param>
    ''' <param name="func">The function to execute</param>
    ''' <param name="parameters">The parameters to pass to function 'func'</param>
    ''' <returns>The result of the function invocation</returns>
    Public Function CallAs2(type As Type, obj As Object, func As String, parameters As Object()) As Object
        Dim methInfo As Reflection.MethodInfo = type.GetMethod(func, Reflection.BindingFlags.Instance Or Reflection.BindingFlags.[Public] Or Reflection.BindingFlags.NonPublic)
        Return methInfo.Invoke(obj, parameters)
    End Function

    ''' <summary>
    ''' Returns the value of property 'prop' of object 'obj'
    ''' </summary>
    ''' <param name="obj">The object containing 'prop'</param>
    ''' <param name="prop">The property name</param>
    ''' <returns>The property value</returns>
    Public Function [Get](obj As Object, prop As String) As Object
        Return GetAs(obj.[GetType](), obj, prop)
    End Function

    ''' <summary>
    ''' Returns the value of property 'prop' of object 'obj' which has type 'type'
    ''' </summary>
    ''' <param name="type">The type of 'obj'</param>
    ''' <param name="obj">The object containing 'prop'</param>
    ''' <param name="prop">The property name</param>
    ''' <returns>The property value</returns>
    Public Function GetAs(type As Type, obj As Object, prop As String) As Object
        Dim propInfo As Reflection.PropertyInfo = type.GetProperty(prop, Reflection.BindingFlags.Instance Or Reflection.BindingFlags.[Public] Or Reflection.BindingFlags.NonPublic)
        Return propInfo.GetValue(obj, Nothing)
    End Function

    ''' <summary>
    ''' Returns an enum value
    ''' </summary>
    ''' <param name="typeName">The name of enum type</param>
    ''' <param name="name">The name of the value</param>
    ''' <returns>The enum value</returns>
    Public Function GetEnum(typeName As String, name As String) As Object
        Dim type As Type = MyGetType(typeName)
        Dim fieldInfo As Reflection.FieldInfo = type.GetField(name)
        Return fieldInfo.GetValue(Nothing)
    End Function

#End Region

End Class
