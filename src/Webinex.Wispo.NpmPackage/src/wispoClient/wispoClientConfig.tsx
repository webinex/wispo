import { AxiosInstance } from 'axios';

/**
 * Wispo client configuration
 */
export interface WispoClientConfig {
  /**
   * SignalR configuration
   */
  signalR?: WispoSignalRConfig;

  /**
   * Http requests configuration
   */
  http?: WispoHttpConfig;
}

/**
 * Wispo client SignalR configuration
 */
export interface WispoSignalRConfig {
  /**
   * Hub URI
   * @default /api/wispo/hub
   */
  hubUri?: string;

  /**
   * Access token factory
   */
  accessTokenFactory?: () => Promise<string>;

  /**
   * Request headers factory
   */
  headersFactory?: () => Promise<{ [name: string]: string }>;
}

/**
 * Wispo client http requests configuration
 */
export interface WispoHttpConfig {
  /**
   * Axios instance for http requests
   * Useful for authorization and before/post processing
   * Default: empty axios instance with base uri /api/wispo
   */
  axios?: AxiosInstance;
}
