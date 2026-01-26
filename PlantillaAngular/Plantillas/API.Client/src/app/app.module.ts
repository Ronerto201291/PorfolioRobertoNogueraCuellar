import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from './app-routing.module';

import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { IonicModule } from '@ionic/angular';
import { AppComponent } from './app.component';
import { LayoutComponent } from './layout/layout.component';
import { MSAL_INSTANCE, MSAL_INTERCEPTOR_CONFIG, MsalBroadcastService, MsalGuard, MsalInterceptor, MsalInterceptorConfiguration, MsalService, ProtectedResourceScopes } from '@azure/msal-angular';
import { IPublicClientApplication, InteractionType, PublicClientApplication } from '@azure/msal-browser';
import * as jsonData from '../assets/msal.json';
import { protectedResources } from '../environments/auth-config';
import { ConfigService } from './config.service';
import { MutuMsalConfig } from './models/MutuMsalConfig';

/**
 * MSAL Angular will automatically retrieve tokens for resources
 * added to protectedResourceMap. For more info, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-angular/docs/v2-docs/initialization.md#get-tokens-for-web-api-calls
 */
export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  const protectedResourceMap = new Map<string, Array<string | ProtectedResourceScopes> | null>();

  protectedResourceMap.set(protectedResources.pruebaangularEndpoints.endpoint, [
    {
      httpMethod: 'GET',
      scopes: [protectedResources.pruebaangularEndpoints.scopes.access_as_user]
    },
    {
      httpMethod: 'POST',
      scopes: [protectedResources.pruebaangularEndpoints.scopes.access_as_user]
    },

    {
      httpMethod: 'PUT',
      scopes: [protectedResources.pruebaangularEndpoints.scopes.access_as_user]
    },
    {
      httpMethod: 'DELETE',
      scopes: [protectedResources.pruebaangularEndpoints.scopes.access_as_user]
    }
  ]);
  protectedResourceMap.set(protectedResources.Conversaciones.endpoint, [
    {
      httpMethod: 'GET',
      scopes: [protectedResources.Conversaciones.scopes.access_as_user]
    },
    {
      httpMethod: 'POST',
      scopes: [protectedResources.Conversaciones.scopes.access_as_user]
    },

    {
      httpMethod: 'PUT',
      scopes: [protectedResources.Conversaciones.scopes.access_as_user]
    },
    {
      httpMethod: 'DELETE',
      scopes: [protectedResources.Conversaciones.scopes.access_as_user]
    }
  ]);
  return {
    interactionType: InteractionType.Popup,
    protectedResourceMap,
  };
}

export function MSALInstanceFactory(): IPublicClientApplication {

  return new PublicClientApplication(jsonData as unknown as MutuMsalConfig);
}

@NgModule({
declarations: [
  AppComponent,
  LayoutComponent
],
imports: [
  BrowserModule,
  CommonModule,
  BrowserAnimationsModule,
  AppRoutingModule,
  FormsModule,
  IonicModule.forRoot(),
  HttpClientModule,
],
  providers: [Location,
    MsalService,
    MsalGuard,
    MsalBroadcastService,
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: MSALInterceptorConfigFactory,
      useExisting: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    {
      provide: ConfigService,
      deps: [HttpClient],
      multi: false,
      useClass: ConfigService
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor() {
  }
}
