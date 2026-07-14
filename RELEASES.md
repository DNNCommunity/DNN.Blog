# Releases

## 06.07.01-DNN10

Maintained compatibility build of DNN Blog 6.7.1 for DNN Platform 10.

### Target environment
- DNN Platform 10.3.2
- .NET Framework 4.8
- SQL Server

### Changes
- Fixed the ViewState exception after Blog permission changes.
- Updated `BlogPermissionsGrid` to avoid restoring dynamically generated child-control ViewState.
- Preserved Blog ID, current user ID and Blog permission collection.

### Validation
- Existing Blogs
- New Blogs
- Permission changes
- Settings updates
- Post creation and editing
- Image uploads
- Production validation on the stock Blog 6.7.1 release

### Deployment note
The fix is contained in:

```text
DotNetNuke.Modules.Blog.Core.dll
```

Always back up the database and Blog assemblies before deployment.
