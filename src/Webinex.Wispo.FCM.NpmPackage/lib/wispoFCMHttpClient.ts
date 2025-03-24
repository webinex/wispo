import axios, { AxiosInstance } from "axios";

export interface WispoFCMHttpClientSettings {
  /**
   * Axios instance for http requests
   * Useful for authorization and before/post processing
   */
  axios?: AxiosInstance;
  routes?: {
    /**
     * Register device route
     * @default "/api/wispo/fcm/devices"
     */
    registerDevice?: string;

    /**
     * Get web config route
     * @default "/api/wispo/fcm/web/config"
     */
    getWebConfig?: string;
  };
}

export interface WispoFCMRegisterDeviceRequest {
  token: string;
  deviceType: string;
  extra?: object;
}

export interface WispoFCMGetConfigResponse {
  apiKey: string;
  authDomain: string;
  projectId: string;
  storageBucket: string;
  messagingSenderId: string;
  appId: string;
  vapidKey: string;
  measurementId?: string;
}

export class WispoFCMHttpClient {
  private readonly _axios: AxiosInstance;
  private readonly _registerDeviceRoute: string;
  private readonly _getWebConfig: string;

  constructor(args?: WispoFCMHttpClientSettings) {
    this._axios = args?.axios ?? axios.create();
    this._registerDeviceRoute =
      args?.routes?.registerDevice ?? "/api/wispo/fcm/devices";
    this._getWebConfig =
      args?.routes?.getWebConfig ?? "/api/wispo/fcm/web/config";
  }

  public registerDevice = async (request: WispoFCMRegisterDeviceRequest) => {
    await this._axios.put(this._registerDeviceRoute, request);
  };

  public getWebConfig = async () => {
    const { data } = await this._axios.get<WispoFCMGetConfigResponse>(
      this._getWebConfig
    );
    return data;
  };
}
