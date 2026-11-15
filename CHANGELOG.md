# Changelog

All notable changes to the DNN Blog DNN10 Maintenance Branch are documented in this file.

The format is based on Keep a Changelog.

---

## [06.07.01-DNN10] - 2026-07-14

### Fixed

- Fixed ASP.NET ViewState exception when editing Blog settings after changing Blog permissions.
- Prevented `BlogPermissionsGrid` from restoring dynamically generated child-control ViewState.
- Continued to persist:
  - Blog ID
  - Current User ID
  - Blog permission collection.

### Validation

Successfully tested on:

- DNN Platform 10.3.2
- .NET Framework 4.8
- SQL Server

Verified by:

- Editing existing Blogs
- Creating a second Blog
- Editing Blog permissions repeatedly
- Changing Blog settings
- Creating posts
- Editing posts
- Uploading images
- Production deployment using the stock Blog 6.7.1 release

### Contributors

- Trevor Forrest (Forro-54)