import { Injectable } from '@angular/core';
import { Auth0Configuration, auth0Configuration } from './auth.config';

@Injectable({ providedIn: 'root' })
export class PortalAuthService {
  readonly configuration: Auth0Configuration = auth0Configuration;

  isConfigured(): boolean {
    return !Object.values(this.configuration).some((value) => value.startsWith('YOUR_'));
  }
}