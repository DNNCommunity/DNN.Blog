Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Entities.Portals

Namespace Controls

 Public MustInherit Class LocalizedTextEdit
  Inherits System.Web.UI.WebControls.Panel

#Region " Private Members "
  Private _localizedTexts As LocalizedText = Nothing
  Friend pnlBox As System.Web.UI.WebControls.Panel
  Private _defaultText As String = String.Empty
  Private _supportedLocales As LocaleCollection = Nothing
  Private _DefaultLanguage As String = Nothing
#End Region

#Region " Protected Methods "
  Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

   If Not _PreRendered AndAlso (RebindOnPostback Or (Not Me.Page.IsPostBack)) Then
    Me.DataBind()
   End If
   _PreRendered = True

  End Sub

  Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
   If Not _PreRendered AndAlso (RebindOnPostback Or (Not Me.Page.IsPostBack)) Then
    Me.DataBind()
   End If
   _PreRendered = True
   MyBase.Render(writer)
  End Sub

  Public Overrides Sub DataBind()
   Me.EnsureChildControls()
   If Me.Page.IsPostBack And (Not ManualUpdate) Then
    Update()
   End If
   Rebind()
  End Sub

  Protected Overrides Sub LoadViewState(ByVal savedState As Object)

   _localizedTexts = New LocalizedText
   If Not (savedState Is Nothing) Then
    Dim myState As Object() = CType(savedState, Object())
    If Not (myState(0) Is Nothing) Then
     DefaultText = CStr(myState(0))
    End If
    If Not (myState(1) Is Nothing) Then
     _localizedTexts.Deserialize(CStr(myState(1)))
    End If
    If Not (myState(2) Is Nothing) Then
     MyBase.LoadViewState(myState(2))
    End If
   End If

  End Sub

  Protected Overrides Function SaveViewState() As Object

   Me.EnsureChildControls()
   If Not ManualUpdate Then
    Update()
   End If
   Dim allStates(2) As Object
   allStates(0) = DefaultText
   If _localizedTexts Is Nothing Then
    allStates(1) = Nothing
   Else
    allStates(1) = _localizedTexts.ToString
   End If
   allStates(2) = MyBase.SaveViewState()
   Return allStates

  End Function
#End Region

#Region " Events "
  Private Sub LocalizedTextEdit_Load(sender As Object, e As System.EventArgs) Handles Me.Load
   Me.EnsureChildControls()
  End Sub
#End Region

#Region " Public Methods "
  Public MustOverride Sub Update()
  Public MustOverride Sub Rebind()

  Public Function GetLocalizedTexts() As LocalizedText
   If Not ManualUpdate Then
    Update()
   End If
   Return LocalizedTexts
  End Function

  Public Sub InitialBind()
   Dim mu As Boolean = ManualUpdate
   ManualUpdate = True
   DataBind()
   ManualUpdate = mu
  End Sub
#End Region

#Region " Properties "
  Public Property TextBoxWidth As Unit = Unit.Pixel(0)
  Public Property TextBoxHeight As Unit = Unit.Pixel(0)
  Public Property MaxImage As String = "~/images/max.gif"
  Public Property MinImage As String = "~/images/min.gif"
  Public Property StartMaximized As Boolean = False
  Public Property UseFlags As Boolean = True
  Public Property RebindOnPostback As Boolean = True
  Public Property ManualUpdate As Boolean = False
  Public Property PreRendered As Boolean = False
  Public Property JustUpdated As Boolean = False
  Public Property CssPrefix As String = ""

  Public Property LocalizedTexts() As LocalizedText
   Get
    If _localizedTexts Is Nothing Then
     _localizedTexts = New LocalizedText()
    End If
    Return _localizedTexts
   End Get
   Set(ByVal value As LocalizedText)
    _localizedTexts = value
   End Set
  End Property

  Public Property DefaultText() As String
   Get
    If Not ManualUpdate Then
     Update()
    End If
    Return _defaultText
   End Get
   Set(ByVal value As String)
    _defaultText = value
   End Set
  End Property

  Public Property SupportedLocales() As LocaleCollection
   Get
    If _supportedLocales Is Nothing Then
     _supportedLocales = New LocaleCollection
     Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
     For Each kvp As KeyValuePair(Of String, Locale) In LocaleController.Instance.GetLocales(objPortalSettings.PortalId)
      _supportedLocales.Add(kvp.Key, kvp.Value)
     Next
    End If
    Return _supportedLocales
   End Get
   Set(ByVal value As LocaleCollection)
    _supportedLocales = value
   End Set
  End Property

  Public Property DefaultLanguage() As String
   Get
    If _DefaultLanguage Is Nothing Then
     _DefaultLanguage = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings.DefaultLanguage
    End If
    Return _DefaultLanguage
   End Get
   Set(ByVal value As String)
    _DefaultLanguage = value
   End Set
  End Property
#End Region

 End Class
End Namespace
