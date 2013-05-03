Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services

Namespace Integration
 Partial Public Class BlogModuleController
  Implements IUpgradeable

#Region " IUpgradeable Methods "
  Public Function UpgradeModule(ByVal Version As String) As String Implements IUpgradeable.UpgradeModule
   Dim message As String = ""

   Select Case Version
    Case "05.00.00"
     NotificationController.AddNotificationTypes()
   End Select
   Return message

  End Function
#End Region

 End Class
End Namespace