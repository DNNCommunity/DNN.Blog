Imports DotNetNuke.Entities.Modules

Namespace Common
 Public Class BlogModuleController
  Implements IUpgradeable

#Region "IUpgradeable"

  Public Function UpgradeModule(ByVal Version As String) As String Implements IUpgradeable.UpgradeModule
   Dim message As String = ""

   Select Case Version
    Case "05.00.00"
     'message = message & " Migrating taxonomy/folksonomy to core in " & Version & " :" & vbCrLf & vbCrLf
     'Dim _CustomUpgrade As New Upgrade.ModuleUpgrade
     'message += _CustomUpgrade.MigrateTaxonomyFolksonomy()
     Integration.NotificationController.AddNotificationTypes()
   End Select
   Return message

  End Function

#End Region

 End Class
End Namespace