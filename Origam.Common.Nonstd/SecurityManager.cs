#region license
/*
Copyright 2005 - 2018 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using Microsoft.Practices.EnterpriseLibrary.Security;
using System.Threading;
using System.Security.Principal;

namespace Origam
{
	/// <summary>
	/// Summary description for SecurityManager.
	/// </summary>
	public class SecurityManager
	{
		public const string ROLE_SUFFIX_DIVIDER = "|";
		public const string READ_ONLY_ROLE_SUFFIX = "isReadOnly()";
        public const string BUILTIN_SUPER_USER_ROLE = "e0ad1a0b-3e05-4b97-be38-12ff63e7f2f2";

		private static ProfileProviderFactory _profileFactory = new ProfileProviderFactory();
		private static AuthorizationProviderFactory _authorizationFactory = new AuthorizationProviderFactory();

		private static IOrigamProfileProvider _profileProvider = null;
		private static IOrigamAuthorizationProvider _authorizationProvider = null;

		public static IOrigamAuthorizationProvider GetAuthorizationProvider()
		{
			if(_authorizationProvider == null)
			{
				_authorizationProvider = _authorizationFactory.GetAuthorizationProvider() as IOrigamAuthorizationProvider;
			}

			return _authorizationProvider;
		}

		
		public static IOrigamProfileProvider GetProfileProvider()
		{
			if(_profileProvider == null)
			{
				_profileProvider = _profileFactory.GetProfileProvider() as IOrigamProfileProvider;
			}

			return _profileProvider;
		}

		public static void Reset()
		{
			_profileProvider = null;
			_authorizationProvider = null;
			_profileFactory = new ProfileProviderFactory();
			_authorizationFactory = new AuthorizationProviderFactory();
		}

		public static string GetReadOnlyRoles(string roles)
		{
			string authContext = "";
			if(roles != null)
			{
				string[] roleList = roles.Split(";".ToCharArray());
				foreach(string role in roleList)
				{
					authContext += role + ROLE_SUFFIX_DIVIDER + READ_ONLY_ROLE_SUFFIX + ";";
				}
			}
			return authContext;
		}

        public static void SetServerIdentity()
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("origam_server"), null);
        }

        public static IPrincipal CurrentPrincipal
        {
            get
            {
                if (Thread.CurrentPrincipal == null)
                {
                    throw new UserNotLoggedInException();
                }
                else
                {
                    return Thread.CurrentPrincipal;
                }
            }
        }

        public static UserProfile CurrentUserProfile()
        {
            IOrigamProfileProvider profileProvider = GetProfileProvider();
            return (UserProfile)profileProvider.GetProfile(CurrentPrincipal.Identity);
        }
    }
}
