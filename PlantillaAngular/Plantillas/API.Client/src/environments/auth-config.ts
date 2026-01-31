/**
 * This file contains authentication parameters. Contents of this file
 * is roughly the same across other MSAL.js libraries. These parameters
 * are used to initialize Angular and MSAL Angular configurations in
 * in app.module.ts file.
 */

import { environment } from "./environment";


/**
 * Add here the endpoints and scopes when obtaining an access token for protected web APIs. For more information, see:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/resources-and-scopes.md
 */
export const protectedResources = {
  pruebaangularEndpoints: {
    endpoint: environment.ApiUrl +'api/v1/BlogPostBFFAsync/',// "https://mutuai-des-uks.azurewebsites.net/api/v1/AzureGpt",
    scopes: 
    {
      access_as_user: "api://94d757e5-46d4-4de7-8b48-91ab3d30f2d7/access_as_user"
    }
  },
  Conversaciones: {
    endpoint: environment.ApiUrl + 'api/v1/TareasComentariosAsync/',//"https://mutuai-des-uks.azurewebsites.net/api/v1/Conversaciones",
    scopes: 
    {
      access_as_user: "api://94d757e5-46d4-4de7-8b48-91ab3d30f2d7/access_as_user"
    }
  }
}

/**
 * Scopes you add here will be prompted for user consent during sign-in.
 * By default, MSAL.js will add OIDC scopes (openid, profile, email) to any login request.
 * For more information about OIDC scopes, visit:
 * https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent#openid-connect-scopes
 */
export const loginRequest = {
  scopes: ["openid", "profile", "api://94d757e5-46d4-4de7-8b48-91ab3d30f2d7/access_as_user"]//,"api://95b20ca2-0f96-4503-b3ab-e5e91a6a5e95/User.Read"]
};
