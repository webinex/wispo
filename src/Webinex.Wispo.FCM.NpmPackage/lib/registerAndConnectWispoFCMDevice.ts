import { initializeApp } from 'firebase/app';
import { getMessaging, getToken, deleteToken } from 'firebase/messaging';
import { WispoFCMGetConfigResponse, WispoFCMHttpClient } from './wispoFCMHttpClient';
import { WISPO_FCM_MESSAGE_CONFIG_TYPE } from './constants';

interface RegisterAndConnectWispoFCMDeviceOptions {
  wispoClient?: WispoFCMHttpClient;
  sw: ServiceWorkerRegistration;
  deviceType?: string;
  extraDetails?: object;
}

/**
 * Register device and connect it to wispo fcm
 * @param options
 * @returns unregister callback
 */
export const registerAndConnectWispoFCMDevice = async ({
  wispoClient,
  sw,
  deviceType,
  extraDetails,
}: RegisterAndConnectWispoFCMDeviceOptions) => {
  wispoClient = wispoClient ?? new WispoFCMHttpClient();
  const config = await wispoClient.getWebConfig();

  connectWispoFCMWithSw(sw, config);

  const app = initializeApp(config);
  const messaging = getMessaging(app);
  const token = await getToken(messaging, {
    vapidKey: config.vapidKey,
    serviceWorkerRegistration: sw,
  });

  await wispoClient.registerDevice({
    token,
    deviceType: deviceType ?? 'web',
    extra: extraDetails,
  });

  return async () => {
    await deleteToken(messaging);
  };
};

const connectWispoFCMWithSw = (sw: ServiceWorkerRegistration, config: WispoFCMGetConfigResponse) => {
  if (sw.active) {
    sw.active.postMessage({
      type: WISPO_FCM_MESSAGE_CONFIG_TYPE,
      payload: config,
    });
  } else {
    sw.addEventListener('activate', () => {
      sw.active!.postMessage({
        type: WISPO_FCM_MESSAGE_CONFIG_TYPE,
        payload: config,
      });
    });
  }
};
