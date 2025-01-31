using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2010 by DotNetNuke Corp. 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;

namespace DotNetNuke.Modules.Blog.Templating
{

  /// <summary>
 /// The TokenReplace class provides the option to replace tokens formatted 
 /// [object:property] or [object:property|format] or [custom:no] within a string
 /// with the appropriate current property/custom values.
 /// Example for Newsletter: 'Dear [user:Displayname],' ==> 'Dear Superuser Account,'
 /// Supported Token Sources: User, Host, Portal, Tab, Module, Membership, Profile, 
 ///                          Row, Date, Ticks, ArrayList (Custom), IDictionary
 /// </summary>
 /// <remarks></remarks>
  public class TokenReplace : BaseCustomTokenReplace
  {

    #region Private Fields 

    private PortalSettings _PortalSettings;
    private Dictionary<string, string> _Hostsettings;
    private ModuleInfo _ModuleInfo;
    private UserInfo _User;
    private DotNetNuke.Entities.Tabs.TabInfo _Tab;
    private int _ModuleId = int.MinValue;

    #endregion

    #region Public Properties 

    /// <summary>
  /// Gets the Host settings from Portal
  /// </summary>
  /// <value>A hashtable with all settings</value>
    private Dictionary<string, string> HostSettings
    {
      get
      {
        if (_Hostsettings is null)
        {
          _Hostsettings = HostController.Instance.GetSettings().Where(c => !c.Value.IsSecure).ToDictionary(c => c.Key, c => c.Value.Value);

        }
        return _Hostsettings;
      }
    }

    /// <summary>
  /// Gets/sets the current ModuleID to be used for 'User:' token replacement
  /// </summary>
  /// <value>ModuleID (Integer)</value>
    public int ModuleId
    {
      get
      {
        return _ModuleId;
      }
      set
      {

        _ModuleId = value;
      }
    }

    /// <summary>
  /// Gets/sets the module settings object to use for 'Module:' token replacement
  /// </summary>
    public ModuleInfo ModuleInfo
    {
      get
      {
        if (ModuleId > int.MinValue && (_ModuleInfo is null || _ModuleInfo.ModuleID != ModuleId))
        {
          var mc = new ModuleController();
          if (PortalSettings is not null && PortalSettings.ActiveTab is not null)
          {
            _ModuleInfo = mc.GetModule(ModuleId, PortalSettings.ActiveTab.TabID, false);
          }
          else
          {
            _ModuleInfo = mc.GetModule(ModuleId);
          }
        }
        return _ModuleInfo;
      }
      set
      {
        _ModuleInfo = value;
      }
    }

    /// <summary>
  /// Gets/sets the portal settings object to use for 'Portal:' token replacement
  /// </summary>
  /// <value>PortalSettings oject</value>
    public PortalSettings PortalSettings
    {
      get
      {
        return _PortalSettings;
      }
      set
      {
        _PortalSettings = value;
      }
    }

    /// <summary>
  /// Gets/sets the user object to use for 'User:' token replacement
  /// </summary>
  /// <value>UserInfo oject</value>
    public UserInfo User
    {
      get
      {
        return _User;
      }
      set
      {
        _User = value;
      }
    }

    #endregion

    #region Constructor

    /// <summary>
  /// creates a new TokenReplace object for default context
  /// </summary>
  /// <history>
  /// 08/10/2007 sLeupold  documented
  /// </history>
    public TokenReplace() : this(Scope.DefaultSettings, null, null, null, Null.NullInteger)
    {
    }

    /// <summary>
  /// creates a new TokenReplace object for default context and the current module
  /// </summary>
  /// <param name="ModuleID">ID of the current module</param>
  /// <history>
  /// 10/19/2007 sLeupold  added
  /// </history>
    public TokenReplace(int ModuleID) : this(Scope.DefaultSettings, null, null, null, ModuleID)
    {
    }

    /// <summary>
  /// creates a new TokenReplace object for custom context
  /// </summary>
  /// <param name="AccessLevel">Security level granted by the calling object</param>
  /// <history>
  /// 08/10/2007 sLeupold  documented
  /// </history>
    public TokenReplace(Scope AccessLevel) : this(AccessLevel, null, null, null, Null.NullInteger)
    {
    }

    /// <summary>
  /// creates a new TokenReplace object for custom context
  /// </summary>
  /// <param name="AccessLevel">Security level granted by the calling object</param>
  /// <param name="ModuleID">ID of the current module</param>
  /// <history>
  /// 08/10/2007 sLeupold  documented
  /// 10/19/2007 sLeupold  added
  /// </history>
    public TokenReplace(Scope AccessLevel, int ModuleID) : this(AccessLevel, null, null, null, ModuleID)
    {
    }


    /// <summary>
  /// creates a new TokenReplace object for custom context
  /// </summary>
  /// <param name="AccessLevel">Security level granted by the calling object</param>
  /// <param name="Language">Locale to be used for formatting etc.</param>
  /// <param name="PortalSettings">PortalSettings to be used</param>
  /// <param name="User">user, for which the properties shall be returned</param>
  /// <history>
  /// 08/10/2007 sLeupold  documented
  /// </history>
    public TokenReplace(Scope AccessLevel, string Language, PortalSettings PortalSettings, UserInfo User) : this(AccessLevel, Language, PortalSettings, User, Null.NullInteger)
    {
    }

    /// <summary>
  /// creates a new TokenReplace object for custom context
  /// </summary>
  /// <param name="AccessLevel">Security level granted by the calling object</param>
  /// <param name="Language">Locale to be used for formatting etc.</param>
  /// <param name="PortalSettings">PortalSettings to be used</param>
  /// <param name="User">user, for which the properties shall be returned</param>
  /// <param name="ModuleID">ID of the current module</param>
  /// <history>
  ///     08/10/2007    sleupold  documented
  ///     10/19/2007    sleupold  ModuleID added
  /// </history>
    public TokenReplace(Scope AccessLevel, string Language, PortalSettings PortalSettings, UserInfo User, int ModuleID)
    {
      CurrentAccessLevel = AccessLevel;
      if (AccessLevel != Scope.NoSettings)
      {
        if (PortalSettings is null)
        {
          if (HttpContext.Current is not null)
            this.PortalSettings = Framework.ServiceLocator<IPortalController, PortalController>.Instance.GetCurrentPortalSettings();
        }
        else
        {
          this.PortalSettings = PortalSettings;
        }
        if (User is null)
        {
          if (HttpContext.Current is not null)
          {
            this.User = (UserInfo)HttpContext.Current.Items["UserInfo"];
          }
          else
          {
            this.User = new UserInfo();
          }
          AccessingUser = this.User;
        }
        else
        {
          this.User = User;
          if (HttpContext.Current is not null)
          {
            AccessingUser = (UserInfo)HttpContext.Current.Items["UserInfo"];
          }
          else
          {
            AccessingUser = new UserInfo();
          }
        }
        if (string.IsNullOrEmpty(Language))
        {
          this.Language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
        }
        else
        {
          this.Language = Language;
        }
        if (ModuleID != Null.NullInteger)
        {
          ModuleId = ModuleID;
        }
      }
      PropertySource["date"] = new DateTimePropertyAccess();
      PropertySource["datetime"] = new DateTimePropertyAccess();
      PropertySource["ticks"] = new TicksPropertyAccess();
      PropertySource["culture"] = new CulturePropertyAccess();
    }

    #endregion

    #region Public Replace Methods

    /// <summary>
  /// Replaces tokens in strSourceText parameter with the property values
  /// </summary>
  /// <param name="strSourceText">String with [Object:Property] tokens</param>
  /// <returns>string containing replaced values</returns>
    public string ReplaceEnvironmentTokens(string strSourceText)
    {
      return ReplaceTokens(strSourceText);
    }

    /// <summary>
  /// Replaces tokens in strSourceText parameter with the property values
  /// </summary>
  /// <param name="strSourceText">String with [Object:Property] tokens</param>
  /// <param name="row"></param>
  /// <returns>string containing replaced values</returns>
    public string ReplaceEnvironmentTokens(string strSourceText, DataRow row)
    {
      var rowProperties = new DataRowPropertyAccess(row);
      PropertySource["field"] = rowProperties;
      PropertySource["row"] = rowProperties;
      return ReplaceTokens(strSourceText);
    }

    /// <summary>
  /// Replaces tokens in strSourceText parameter with the property values
  /// </summary>
  /// <param name="strSourceText">String with [Object:Property] tokens</param>
  /// <param name="Custom"></param>
  /// <param name="CustomCaption"></param>
  /// <returns>string containing replaced values</returns>
    public string ReplaceEnvironmentTokens(string strSourceText, ArrayList Custom, string CustomCaption)
    {
      PropertySource[CustomCaption.ToLower()] = new ArrayListPropertyAccess(Custom);
      return ReplaceTokens(strSourceText);
    }

    /// <summary>
  /// Replaces tokens in strSourceText parameter with the property values
  /// </summary>
  /// <param name="strSourceText">String with [Object:Property] tokens</param>
  /// <param name="Custom">NameValueList for replacing [custom:name] tokens, where 'custom' is specified in next param and name is either thekey or the index number in the string </param>
  /// <param name="CustomCaption">Token name to be used inside token  [custom:name]</param>
  /// <returns>string containing replaced values</returns>
  /// <history>
  /// 08/10/2007 sLeupold created
  /// </history>
    public string ReplaceEnvironmentTokens(string strSourceText, IDictionary Custom, string CustomCaption)
    {
      PropertySource[CustomCaption.ToLower()] = new DictionaryPropertyAccess(Custom);
      return ReplaceTokens(strSourceText);
    }

    /// <summary>
  /// Replaces tokens in strSourceText parameter with the property values
  /// </summary>
  /// <param name="strSourceText">String with [Object:Property] tokens</param>
  /// <param name="Custom">NameValueList for replacing [custom:name] tokens, where 'custom' is specified in next param and name is either thekey or the index number in the string </param>
  /// <param name="CustomCaption">Token name to be used inside token  [custom:name]</param>
  /// <param name="Row">DataRow, from which field values shall be used for replacement</param>
  /// <returns>string containing replaced values</returns>
  /// <history>
  /// 08/10/2007 sLeupold created
  /// </history>
    public string ReplaceEnvironmentTokens(string strSourceText, ArrayList Custom, string CustomCaption, DataRow Row)
    {
      var rowProperties = new DataRowPropertyAccess(Row);
      PropertySource["field"] = rowProperties;
      PropertySource["row"] = rowProperties;
      PropertySource[CustomCaption.ToLower()] = new ArrayListPropertyAccess(Custom);
      return ReplaceTokens(strSourceText);
    }

    /// <summary>
  /// Replaces tokens in strSourceText parameter with the property values, skipping environment objects
  /// </summary>
  /// <param name="strSourceText">String with [Object:Property] tokens</param>
  /// <returns>string containing replaced values</returns>
  /// <history>
  /// 08/10/2007 sLeupold created
  /// </history>
    protected override string ReplaceTokens(string strSourceText)
    {
      InitializePropertySources();
      return base.ReplaceTokens(strSourceText);
    }

    #endregion

    #region Private methods

    /// <summary>
  /// setup context by creating appropriate objects
  /// </summary>
  /// <history>
  /// /08/10/2007 sCullmann created
  /// </history>
  /// <remarks >
  /// security is not the purpose of the initialization, this is in the responsibility of each property access class
  /// </remarks>
    private void InitializePropertySources()
    {

      // Cleanup, by default "" is returned for these objects and any property
      IPropertyAccess DefaultPropertyAccess = new EmptyPropertyAccess();
      PropertySource["portal"] = DefaultPropertyAccess;
      PropertySource["tab"] = DefaultPropertyAccess;
      PropertySource["host"] = DefaultPropertyAccess;
      PropertySource["module"] = DefaultPropertyAccess;
      PropertySource["user"] = DefaultPropertyAccess;
      PropertySource["membership"] = DefaultPropertyAccess;
      PropertySource["profile"] = DefaultPropertyAccess;

      // initialization
      if (CurrentAccessLevel >= Scope.Configuration)
      {
        if (PortalSettings is not null)
        {
          PropertySource["portal"] = PortalSettings;
          PropertySource["tab"] = PortalSettings.ActiveTab;

        }
        PropertySource["host"] = new HostPropertyAccess();
        if (ModuleInfo is not null)
        {
          PropertySource["module"] = ModuleInfo;
        }
      }

      if (CurrentAccessLevel >= Scope.DefaultSettings && !(User is null || User.UserID == -1))
      {
        PropertySource["user"] = User;
        PropertySource["membership"] = new MembershipPropertyAccess(User);
        PropertySource["profile"] = new ProfilePropertyAccess(User);
      }

    }

    #endregion

  }

}