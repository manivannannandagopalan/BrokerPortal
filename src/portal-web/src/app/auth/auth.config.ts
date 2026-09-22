export interface Auth0Configuration {
  domain: string;
  clientId: string;
  audience: string;
}

export const auth0Configuration: Auth0Configuration = {
  domain: 'YOUR_AUTH0_DOMAIN',
  clientId: 'YOUR_AUTH0_CLIENT_ID',
  audience: 'YOUR_API_IDENTIFIER'
};